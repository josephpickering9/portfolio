<template>
  <div class="flex w-full flex-col items-center px-4">
    <ProjectForm :slug="slug" />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import useMeta from '~/app/composables/useMeta'
import ProjectForm from '~/app/components/forms/project/ProjectForm.vue'

definePageMeta({ middleware: 'auth' })

const { updateMeta } = useMeta()
const route = useRoute()

const slug = computed(() => {
  const param = route.params.slug
  return Array.isArray(param) ? param[0] : param
})

updateMeta({ title: 'Update Project', description: 'Update an existing project' })
</script>
