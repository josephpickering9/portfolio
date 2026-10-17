<template>
  <MockupBrowser size="lg" :host="getHost(project.website) || project.slug">
    <NuxtImg
v-if="heroImage"
      format="webp"
      :src="getImageUrl(heroImage)"
      :alt="project.title"
      sizes="100vw xl:1144px"
      loading="eager"
      fetchpriority="high"
      class="block aspect-video w-full object-cover object-left-top"
    />
    <div v-else class="aspect-video" />
  </MockupBrowser>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { Project } from '@api'
import { getImageUrl } from '~/utils/image-helper'
import { getHeroImage, getHost } from '~/utils/project-helper'
import MockupBrowser from '~/components/mockup/MockupBrowser.vue'

interface Props {
  project: Project
}

const props = defineProps<Props>()

const heroImage = computed(() => getHeroImage(props.project))
</script>
