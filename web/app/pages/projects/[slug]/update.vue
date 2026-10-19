<template>
  <ProjectForm :slug="slug" />
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router'
import { definePageMeta } from '#imports'
import useMeta from '~/composables/useMeta'
import ProjectForm from '~/components/project/forms/ProjectForm.vue'

definePageMeta({ middleware: 'auth' })

const route = useRoute()
const slug = String(route.params['slug'])

const { updateMeta } = useMeta()
updateMeta({ title: 'Update project' })
</script>
