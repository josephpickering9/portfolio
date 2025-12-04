<template>
  <div class="mx-auto mt-2 flex w-full max-w-7xl flex-col justify-center gap-4 overflow-hidden px-8">
    <div v-if="projects.length" class="-mx-4 flex flex-col gap-4 rounded-box bg-neutral p-4 md:mx-0">
      <div class="prose"><h3 class="m-0">Related Projects</h3></div>
      <div class="mx-auto flex items-center justify-center">
        <ProjectList :set-projects="projects" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Project } from '@api'
import ProjectList from '~/components/project/list/ProjectList.vue'

defineProps<{
  projects: Project[]
}>()
</script>
