<template>
  <ProjectForm />
</template>

<script setup lang="ts">
import { definePageMeta } from '#imports'
import ProjectForm from '~/components/project/forms/ProjectForm.vue'
import useMeta from '~/composables/useMeta'

definePageMeta({ middleware: 'auth' })

const { updateMeta } = useMeta()
updateMeta({ title: 'New project' })
</script>
