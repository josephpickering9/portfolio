<template>
  <NuxtLink
    v-if="project"
    :to="`/projects/${project.slug}`"
    class="group card bg-base-100 hover:bg-base-200/50 shadow-lg hover:shadow-2xl transition-all duration-300 border border-base-200 hover:border-primary/30 flex flex-col h-full overflow-hidden hover:-translate-y-1"
  >
    <figure class="relative h-48 w-full overflow-hidden bg-base-200">
      <NuxtImg 
        v-if="project.cardUrl" 
        :src="getImageUrl(project.cardUrl)" 
        :alt="project.title"
        placeholder
        format="webp"
        class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110"
      />
      <div v-else class="w-full h-full flex items-center justify-center bg-base-300 text-base-content/20">
         <Icon name="heroicons:photo" class="w-16 h-16" />
      </div>
      <div class="absolute inset-0 bg-gradient-to-t from-base-100 to-transparent opacity-60" />
      
      <div v-if="project.logoUrl" class="absolute bottom-2 right-2 p-1 bg-base-100/90 rounded-lg shadow-sm backdrop-blur-sm">
         <NuxtImg 
          :src="getImageUrl(project.logoUrl)" 
          :alt="`${project.title} Logo`" 
          placeholder
          format="webp"
          class="h-8 w-8 object-contain" 
         />
      </div>
    </figure>
    
    <div class="card-body p-6 flex-grow">
      <div class="flex flex-wrap gap-2 mb-3">
         <Tag 
          v-for="(tag, index) in project.tags?.slice(0, 3)" 
          :key="tag.id || index" 
          :tag="tag" 
          class="badge badge-sm badge-outline border-primary/50 text-primary/80"
        />
        <span v-if="project.tags?.length > 3" class="text-xs text-base-content/50 self-center">+{{ project.tags.length - 3 }}</span>
      </div>

      <h3 class="card-title text-xl font-bold group-hover:text-primary transition-colors">
        {{ project.title }}
      </h3>
      
      <p class="text-base-content/70 text-sm line-clamp-3 mt-2">
        {{ project.shortDescription }}
      </p>

      <div class="card-actions justify-end mt-4 pt-4 border-t border-base-200">
         <span class="text-xs uppercase font-bold tracking-wider text-base-content/50 group-hover:text-primary transition-colors flex items-center gap-1">
            View Details <Icon name="heroicons:arrow-right" class="w-3 h-3 transition-transform group-hover:translate-x-1" />
         </span>
      </div>
    </div>
  </NuxtLink>
</template>

<script setup lang="ts">
import type { Project } from '@api'
import { getImageUrl } from '~/utils/image-helper'
import Tag from '~/components/tag/Tag.vue'

interface Props {
  project: Project
}

const props = defineProps<Props>()
</script>
