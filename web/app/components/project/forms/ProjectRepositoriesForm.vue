<template>
  <div v-if="form" class="flex flex-col gap-4 pb-4">
    <FormGroup :errors="v$.form?.repositories?.$errors" name="Repositories">
      <RepositoryInput v-model="form.repositories" />
    </FormGroup>
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { cloneDeep } from 'lodash-es'
import type { CreateProject } from '@api'
import { defaultProjectForm } from '~/utils/default-helper'
import FormGroup from '~/components/ui/form/FormGroup.vue'
import RepositoryInput from '~/components/repository/RepositoryInput.vue'

interface Props {
  modelValue?: CreateProject
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => cloneDeep(defaultProjectForm),
})


const emit = defineEmits<{
  'update:modelValue': [value: CreateProject]
}>()

const form = ref<CreateProject>(props.modelValue)

const v$ = useVuelidate()

async function validate(): Promise<boolean> {
  return true
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
