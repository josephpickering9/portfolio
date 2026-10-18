<template>
  <div class="flex flex-col gap-5.5">
    <div class="flex flex-wrap items-center justify-between gap-4">
      <NuxtLink :to="backTo" class="text-sm text-muted">← {{ backLabel }}</NuxtLink>
      <ClientOnly>
        <NuxtLink v-if="isAuthenticated" :to="`/projects/${project.slug}/update`" class="flex items-center gap-1.5 text-sm text-muted">
          <Icon name="heroicons:pencil-square" />
          Edit project
        </NuxtLink>
      </ClientOnly>
    </div>
    <span class="eyebrow text-accent">{{ eyebrow }}</span>
    <h1 class="m-0 font-display text-[clamp(48px,7vw,88px)] leading-[0.95] font-normal tracking-[-0.02em]">{{ project.title }}</h1>
    <p class="m-0 max-w-180 text-xl leading-[1.55] text-muted">{{ project.shortDescription }}</p>
    <div v-if="project.website || project.repositories.length" class="flex flex-wrap gap-3">
      <a
        v-if="project.website"
        :href="project.website"
        target="_blank"
        rel="noopener noreferrer"
        class="flex items-center gap-2 rounded-full bg-accent px-4.5 py-2.5 text-sm font-medium text-accent-ink hover:text-accent-ink hover:opacity-90"
      >
        <Icon name="mdi:open-in-new" />
        Live site
      </a>
      <a
        v-for="repository in project.repositories"
        :key="repository.id"
        :href="repository.url"
        target="_blank"
        rel="noopener noreferrer"
        class="flex items-center gap-2 rounded-full border border-line px-4.5 py-2.5 text-sm font-medium text-text"
      >
        <Icon name="mdi:github" />
        {{ repository.title }}
      </a>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Project } from '@api'
import useAuth from '~/composables/useAuth'

interface Props {
  project: Project
  eyebrow: string
  backTo?: string
  backLabel?: string
}

withDefaults(defineProps<Props>(), {
  backTo: '/projects',
  backLabel: 'All projects',
})

const { isAuthenticated } = useAuth()
</script>
