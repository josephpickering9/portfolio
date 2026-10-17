<template>
  <NuxtLink
    :to="`/projects/${project.slug}`"
    class="flex flex-col gap-4 text-text transition-transform duration-350 ease-lift hover:-translate-y-1.5 hover:text-text"
  >
    <MockupBrowser size="sm">
      <NuxtImg
v-if="getCardImage(project)"
        format="webp"
        :src="getImageUrl(getCardImage(project))"
        :alt="project.title"
        width="358"
        height="224"
        fit="cover"
        :modifiers="{ position: 'left top' }"
        sizes="100vw md:50vw lg:358px"
        loading="lazy"
        class="block aspect-16/10 w-full object-cover object-left-top"
      />
      <div v-else class="aspect-16/10" />
    </MockupBrowser>
    <div class="flex flex-col gap-2">
      <h3 class="m-0 text-[21px] font-semibold tracking-[-0.01em]">{{ project.title }}</h3>
      <p class="m-0 text-[15px] leading-[1.55] text-muted">{{ project.shortDescription }}</p>
      <span class="font-mono text-xs text-muted">{{ getTagSummary(project) }}</span>
    </div>
  </NuxtLink>
</template>

<script setup lang="ts">
import type { Project } from '@api'
import { getImageUrl } from '~/utils/image-helper'
import { getCardImage, getTagSummary } from '~/utils/project-helper'
import MockupBrowser from '~/components/mockup/MockupBrowser.vue'

interface Props {
  project: Project
}

defineProps<Props>()
</script>
