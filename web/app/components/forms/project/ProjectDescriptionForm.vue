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
import type { CreateProject, Project } from '~/api'
import { useProjectStore } from '~/app/store/ProjectStore'
import useValidation from '~/app/composables/useValidation'
import { defaultProjectForm } from '~/mocks/Defaults'
import FormGroup from '~/app/components/forms/elements/FormGroup.vue'
import TextEditor from '~/app/components/forms/elements/TextEditor.vue'

interface Props {
  modelValue?: CreateProject
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => cloneDeep(defaultProjectForm),
})

// Emits
const emit = defineEmits<{
  'update:modelValue': [value: CreateProject]
}>()

const projectStore = useProjectStore()
const validation = useValidation()

const form = ref<CreateProject>(props.modelValue)

// Validation
const v$ = useVuelidate()

const project = computed((): Project | undefined => {
  return projectStore.project
})

const loading = computed((): boolean => {
  return projectStore.projectCreating || projectStore.projectLoading
})

async function validate(): Promise<boolean> {
  return await validation.validate(v$)
}

// Expose methods for parent component
defineExpose({
  validate,
})

// Watch methods
watch(() => props.modelValue, (newValue) => {
  form.value = newValue
}, { immediate: true })

watch(form, (newValue) => {
  emit('update:modelValue', newValue)
}, { deep: true })
</script>
