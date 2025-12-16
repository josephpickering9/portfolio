<template>
  <div class="flex flex-col gap-8">
    <div 
      v-if="project.bannerUrl"
      class="h-64 w-full rounded-xl bg-cover bg-center md:h-96 shadow-sm"
      :style="bannerStyle"
    />

    <div class="flex flex-col gap-4 px-6 md:px-0">
      <div class="flex flex-col gap-x-4 gap-y-2 md:flex-row md:items-center justify-between">
        <div class="flex items-center gap-4">
          <NuxtImg
            v-if="project.logoUrl"
            :src="getImageUrl(project.logoUrl)"
            :alt="`${project.title} Logo`"
            placeholder
            format="webp"
            class="h-16 w-16 rounded-full shadow-sm ring-1 ring-base-content/10"
          />
          <div class="flex flex-col gap-2">
            <h1 class="text-3xl font-extrabold tracking-tight md:text-4xl lg:text-5xl">{{ project.title }}</h1>
            <p class="text-lg font-medium text-base-content/60">{{ dateRange }}</p>
          </div>
        </div>

        <div class="flex items-center gap-4">
          <ProjectLinks :project="project" :company="company" />

          <ClientOnly>
            <NuxtLink
              v-if="isAuthenticated"
              :to="`/projects/${project.slug}/update`"
              class="btn btn-sm btn-ghost"
          >
            Edit Project
          </NuxtLink>
        </ClientOnly>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { format, parseISO } from 'date-fns'
import type { Company, Project } from '@api'
import { getImageUrl } from '~/utils/image-helper'
import useAuth from '~/composables/useAuth'
import ProjectLinks from './ProjectLinks.vue';

const props = defineProps<{
  project: Project
  company?: Company
}>()

const { isAuthenticated } = useAuth()

const bannerStyle = computed(() => {
  if (!props.project.bannerUrl) return {}
  return {
    backgroundImage: `url(${getImageUrl(props.project.bannerUrl)})`,
  }
})

const dateRange = computed(() => {
  if (!props.project.startDate) return ''
  
  const start = format(parseISO(props.project.startDate), 'MMM yyyy')
  const end = props.project.endDate 
    ? format(parseISO(props.project.endDate), 'MMM yyyy') 
    : 'Present'
    
  return `${start} - ${end}`
})
</script>
