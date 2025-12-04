<template>
  <div v-if="form" class="flex flex-col gap-4 pb-4">
    <FormGroup :errors="v$.form?.repositories?.$errors" name="Repositories">
      <RepositoryInput v-model="form.repositories" />
    </FormGroup>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useVuelidate } from '@vuelidate/core'
import { cloneDeep } from 'lodash-es'
import type { CreateProject, Project } from '~/api'
import { useProjectStore } from '~/app/store/ProjectStore'
import { defaultProjectForm } from '~/mocks/Defaults'
import FormGroup from '~/app/components/forms/elements/FormGroup.vue'
import RepositoryInput from '~/app/components/forms/repository/RepositoryInput.vue'

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
  return true
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
