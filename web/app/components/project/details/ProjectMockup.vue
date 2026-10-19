<template>
  <MockupBrowser size="lg" :host="getHost(project.website) || project.slug">
    <NuxtImg
      v-if="heroImage"
      format="webp"
      :src="getImageUrl(heroImage)"
      :alt="project.title"
      width="1142"
      height="643"
      fit="cover"
      :modifiers="{ position: 'left top' }"
      sizes="100vw xl:1142px"
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
import { useImageUrl } from '~/composables/useImageUrl'
import { getHeroImage } from '~/utils/project-helper'
import { getHost } from '~/utils/string-helper'
import MockupBrowser from '~/components/mockup/MockupBrowser.vue'

interface Props {
  project: Project
}

const props = defineProps<Props>()

const { getImageUrl } = useImageUrl()

const heroImage = computed(() => getHeroImage(props.project))
</script>
