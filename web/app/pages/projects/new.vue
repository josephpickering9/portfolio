<template>
  <ProjectForm />
</template>

<script setup lang="ts">
import ProjectForm from '~/components/project/forms/ProjectForm.vue'
import useMeta from '~/composables/useMeta'

definePageMeta({ middleware: 'auth' })

const { updateMeta } = useMeta()
updateMeta({ title: 'New project' })
</script>
