<template>
  <NuxtLink
    :to="`/projects/${project.slug}`"
    class="flex flex-wrap items-baseline justify-between gap-4 border-y border-line py-7 text-text transition-colors duration-200 hover:text-accent"
  >
    <span class="eyebrow text-muted">Next project</span>
    <span class="font-display text-4xl">{{ project.title }} →</span>
  </NuxtLink>
</template>

<script setup lang="ts">
import type { Project } from '@api'

defineProps<{
  project: Project
}>()
</script>
