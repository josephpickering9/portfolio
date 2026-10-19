<template>
  <div class="flex flex-col gap-6">
    <div v-if="desktopShots.length" class="grid grid-cols-[repeat(auto-fit,minmax(min(100%,420px),1fr))] gap-5">
      <button
        v-for="(shot, index) in desktopShots"
        :key="shot.src"
        type="button"
        class="block cursor-zoom-in text-left transition-transform duration-300 hover:-translate-y-1"
        :class="{ 'overflow-hidden rounded-xl border border-line bg-surface': !framed }"
        :aria-label="`View ${shot.alt} full size`"
        data-testid="gallery-image"
        @click="openAt(index)"
      >
        <component :is="framed ? MockupBrowser : 'div'" v-bind="framed ? { size: 'sm' } : {}">
          <NuxtImg
            format="webp"
            :src="shot.src"
            :alt="shot.alt"
            v-bind="shotSize"
            fit="cover"
            :modifiers="{ position: 'top' }"
            loading="lazy"
            class="block aspect-16/10 w-full object-cover object-top"
          />
        </component>
      </button>
    </div>
    <div v-if="mobileShots.length" class="flex gap-4 overflow-x-auto pb-2">
      <button
        v-for="(shot, index) in mobileShots"
        :key="shot.src"
        type="button"
        class="block shrink-0 cursor-zoom-in rounded-3xl"
        :aria-label="`View ${shot.alt} full size`"
        data-testid="gallery-image"
        @click="openAt(desktopShots.length + index)"
      >
        <MockupPhone :src="shot.src" :alt="shot.alt" :framed="framed" />
      </button>
    </div>
    <ProjectLightbox v-model:index="openIndex" :images="lightboxImages" />
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useImageUrl } from '~/composables/useImageUrl'
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

const { getImageUrl } = useImageUrl()

const openIndex = ref<number | null>(null)

const desktopShots = computed<LightboxImage[]>(() =>
  props.desktopImages.map((image, index) => ({
    src: getImageUrl(image),
    alt: `${props.title} screenshot ${index + 1}`,
    kind: 'desktop',
  })),
)
const mobileShots = computed<LightboxImage[]>(() =>
  props.mobileImages.map((image, index) => ({
    src: getImageUrl(image),
    alt: `${props.title} mobile screenshot ${index + 1}`,
    kind: 'mobile',
  })),
)
const shotSize = computed(() =>
  desktopShots.value.length === 1
    ? { width: 1142, height: 714, sizes: 'sm:100vw xl:1142px' }
    : { width: 560, height: 350, sizes: 'sm:100vw lg:560px' },
)
const lightboxImages = computed(() => [...desktopShots.value, ...mobileShots.value])

function openAt(index: number) {
  openIndex.value = index
}
</script>
