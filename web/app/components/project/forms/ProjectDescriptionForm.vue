<template>
  <div v-if="form" class="flex flex-col gap-4 pb-4">
    <FormGroup :errors="v$.form?.description?.$errors" name="Description">
      <TextEditor v-model="form.description" label="Description" :disabled="loading" />
    </FormGroup>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { cloneDeep } from 'lodash-es'
import type { CreateProject } from '@api'
import { useProjectStore } from '~/store/ProjectStore'
import useValidation from '~/composables/useValidation'
import { defaultProjectForm } from '~/utils/default-helper'
import FormGroup from '~/components/ui/form/FormGroup.vue'
import TextEditor from '~/components/ui/input/TextEditor.vue'

interface Props {
  modelValue?: CreateProject
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => cloneDeep(defaultProjectForm),
})


const emit = defineEmits<{
  'update:modelValue': [value: CreateProject]
}>()

const projectStore = useProjectStore()
const validation = useValidation()

const form = ref<CreateProject>(props.modelValue)

const v$ = useVuelidate()

const loading = computed((): boolean => {
  return projectStore.projectCreating || projectStore.projectLoading
})

async function validate(): Promise<boolean> {
  return await validation.validate(v$.value)
}

defineExpose({
  validate,
})

watch(() => props.modelValue, (newValue) => {
  form.value = newValue
}, { immediate: true })

watch(form, (newValue) => {
  emit('update:modelValue', newValue)
}, { deep: true })
</script>
