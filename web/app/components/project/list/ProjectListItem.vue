<template>
  <NuxtLink
    v-if="project"
    :to="`/projects/${project.slug}`"
    class="group card card-compact w-full bg-base-100 shadow-sm transition-all duration-300 hover:-translate-y-1 hover:shadow-xl"
    @mousedown.stop.prevent="(event) => event.preventDefault()"
  >
    <figure class="relative overflow-hidden">
      <img 
        :src="backgroundImage" 
        :alt="`${project.title} Background Image`" 
        class="h-[200px] w-full object-cover transition-transform duration-500 group-hover:scale-105" 
      >
      <div class="absolute inset-0 bg-gradient-to-t from-base-100/90 via-base-100/20 to-transparent opacity-80" />
    </figure>
    
    <div class="card-body relative -mt-8 justify-between">
      <div>
        <div class="mb-2 flex items-center gap-3">
          <div class="flex h-10 w-10 flex-shrink-0 items-center justify-center rounded-lg bg-base-100 p-1 shadow-md ring-1 ring-base-200">
            <img 
              v-if="project.logoUrl" 
              :src="image" 
              :alt="`${project.title} Logo`" 
              class="h-full w-full object-contain" 
            >
            <Icon v-else name="heroicons:cube" class="h-6 w-6 text-base-content/40" />
          </div>
          <div>
            <h2 class="card-title text-lg font-bold leading-tight mt-2">{{ project.title }}</h2>
             <span v-if="project.company" class="text-xs font-medium text-primary/80">
              {{ project.company.name }}
            </span>
          </div>
        </div>

        <p class="line-clamp-2 text-sm text-base-content/70">
          {{ project.shortDescription }}
        </p>
      </div>

      <div class="card-actions mt-4 items-center gap-2">
        <Tag 
          v-for="(tag, index) in project.tags.slice(0, 2)" 
          :key="index" 
          :tag="tag" 
          class="bg-base-200/50"
        />
        <span 
          v-if="project.tags.length > 2" 
          class="flex h-6 items-center px-1.5 text-xs font-medium text-base-content/50"
        >
          +{{ project.tags.length - 2 }}
        </span>
      </div>
    </div>
  </NuxtLink>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { Project } from '@api/models/Project'
import { getImageUrl } from '~/utils/image-helper'
import Tag from '~/components/tag/Tag.vue'

interface Props {
  project: Project
}

const props = defineProps<Props>()

const image = computed((): string => {
  if (!props.project?.logoUrl) return 'https://via.placeholder.com/320x200'

  return getImageUrl(props.project.logoUrl)
})

const backgroundImage = computed((): string => {
  if (!props.project?.cardUrl) return 'https://via.placeholder.com/320x200'

  return getImageUrl(props.project.cardUrl)
})
</script>
