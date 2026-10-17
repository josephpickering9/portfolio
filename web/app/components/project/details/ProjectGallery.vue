<template>
  <div class="flex flex-col gap-6">
    <div v-if="desktopImages.length" class="grid grid-cols-[repeat(auto-fit,minmax(min(100%,420px),1fr))] gap-5">
      <button
        v-for="(image, index) in desktopImages"
        :key="image"
        type="button"
        class="block cursor-zoom-in text-left transition-transform duration-300 hover:-translate-y-1"
        :class="{ 'overflow-hidden rounded-xl border border-line bg-surface': !framed }"
        :aria-label="`View ${title} screenshot ${index + 1} full size`"
        data-testid="gallery-image"
        @click="openAt(index)"
      >
        <component :is="framed ? MockupBrowser : 'div'" v-bind="framed ? { size: 'sm' } : {}">
          <img
            :src="getImageUrl(image)"
            :alt="`${title} screenshot ${index + 1}`"
            loading="lazy"
            class="block aspect-16/10 w-full object-cover object-left-top"
          >
        </component>
      </button>
    </div>
    <div v-if="mobileImages.length" class="flex gap-4 overflow-x-auto pb-2">
      <button
        v-for="(image, index) in mobileImages"
        :key="image"
        type="button"
        class="block shrink-0 cursor-zoom-in rounded-3xl"
        :aria-label="`View ${title} mobile screenshot ${index + 1} full size`"
        data-testid="gallery-image"
        @click="openAt(desktopImages.length + index)"
      >
        <MockupPhone :src="getImageUrl(image)" :alt="`${title} mobile screenshot ${index + 1}`" :framed="framed" />
      </button>
    </div>
    <ProjectLightbox v-model:index="openIndex" :images="lightboxImages" />
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { getImageUrl } from '~/utils/image-helper'
import MockupBrowser from '~/components/mockup/MockupBrowser.vue'
import MockupPhone from '~/components/mockup/MockupPhone.vue'
import type { LightboxImage } from '~/types/LightboxImage'
import ProjectLightbox from './ProjectLightbox.vue'

interface Props {
  title: string
  desktopImages: string[]
  mobileImages: string[]
  framed: boolean
}

const props = defineProps<Props>()

const openIndex = ref<number | null>(null)

const lightboxImages = computed<LightboxImage[]>(() => [
  ...props.desktopImages.map((image, i) => ({
    src: getImageUrl(image),
    alt: `${props.title} screenshot ${i + 1}`,
    kind: 'desktop' as const,
  })),
  ...props.mobileImages.map((image, i) => ({
    src: getImageUrl(image),
    alt: `${props.title} mobile screenshot ${i + 1}`,
    kind: 'mobile' as const,
  })),
])

function openAt(index: number) {
  openIndex.value = index
}
</script>
