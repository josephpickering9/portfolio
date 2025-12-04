<template>
  <div class="flex w-full flex-col items-center space-y-4 px-4">
    <ProjectForm />
  </div>
</template>

<script setup lang="ts">
import ProjectForm from '~/app/components/forms/project/ProjectForm.vue'
import useMeta from '~/app/composables/useMeta'

definePageMeta({ middleware: 'auth' })

const { updateMeta } = useMeta()

updateMeta({ title: 'New Project', description: 'Create a new project' })
</script>
