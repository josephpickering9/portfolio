<template>
  <FormGroup label="Repositories" label-tag="span">
    <RepositoryInput v-model="repositories" :errors="errors" />
  </FormGroup>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import type { CreateProject, CreateProjectRepository } from '@api'
import { validateRepositories } from '~/utils/validation-helper'
import FormGroup from '~/components/ui/form/FormGroup.vue'
import RepositoryInput from '~/components/repository/RepositoryInput.vue'

const form = defineModel<CreateProject>({ required: true })

const submitted = ref(false)

const repositories = computed({
  get: () => form.value.repositories ?? [],
  set: (value: CreateProjectRepository[]) => {
    form.value.repositories = value
  },
})

const errors = computed(() => (submitted.value ? validateRepositories(repositories.value) : {}))
const hasErrors = computed(() => Object.keys(errors.value).length > 0)

async function validate(): Promise<boolean> {
  submitted.value = true
  return Object.keys(validateRepositories(repositories.value)).length === 0
}

defineExpose({
  validate,
  hasErrors,
})
</script>
