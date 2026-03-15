<template>
  <div class="relative -mx-4 flex flex-col gap-3 rounded-box bg-neutral p-4 md:mx-0">
    <div v-if="title" class="flex items-center justify-between">
      <h3 class="m-0">{{ title }}</h3>
      <span v-if="!draggable && images.length > 1" class="text-sm text-base-content/50">
        {{ currentIndex + 1 }} / {{ images.length }}
      </span>
    </div>

    <Draggable
      v-if="draggable"
      ref="carousel"
      v-model="images"
      class="carousel carousel-center w-full space-x-4"
      item-key="index"
    >
      <template #item="{ element, index }">
        <div :key="index" class="carousel-item relative w-4/5 sm:w-auto">
          <NuxtImg
            :src="getImageUrl(element)"
            :alt="`Slide ${index + 1}`"
            placeholder
            format="webp"
            class="h-full w-full object-cover transition-transform duration-500 hover:scale-105"
          />
          <button v-if="showHoverButton" type="button" class="absolute left-0 top-0 flex h-full w-full items-center justify-center bg-black bg-opacity-20 opacity-0 transition-opacity duration-300 hover:opacity-100" @click="$emit('hoverButton', element)">
            <Icon :name="hoverButtonIcon" size="2em" />
          </button>
        </div>
      </template>
    </Draggable>

    <div
      v-else
      ref="carousel"
      class="carousel carousel-center w-full space-x-4 scroll-smooth"
    >
      <div
        v-for="(image, index) in images"
        :key="index"
        :ref="el => setItemRef(el as HTMLElement, index)"
        class="carousel-item relative w-4/5 sm:w-auto cursor-pointer overflow-hidden rounded-lg"
        @click="openLightbox(index)"
      >
        <NuxtImg
          :src="getImageUrl(image)"
          :alt="`Slide ${index + 1}`"
          placeholder
          format="webp"
          class="h-full w-full object-cover transition-transform duration-500 hover:scale-105"
          :width="width"
          :height="height"
        />
        <div class="absolute inset-0 flex items-center justify-center bg-black/0 transition-colors duration-300 hover:bg-black/20">
          <Icon name="material-symbols:zoom-in" size="2em" class="text-white opacity-0 transition-opacity duration-300 drop-shadow-lg group-hover:opacity-100" />
        </div>
        <button v-if="showHoverButton" type="button" class="absolute left-0 top-0 flex h-full w-full items-center justify-center bg-black bg-opacity-20 opacity-0 transition-opacity duration-300 hover:opacity-100" @click.stop="$emit('hoverButton', image)">
          <Icon :name="hoverButtonIcon" size="2em" />
        </button>
      </div>
    </div>

    <button
      v-if="showArrows && images.length > 1"
      type="button"
      class="absolute cursor-pointer -left-4 top-1/2 z-50 -translate-y-1/2 transform rounded-full bg-black/60 p-2 text-white shadow-lg transition-all duration-200 hover:bg-black/80 hover:scale-110 md:-left-6 flex items-center justify-center"
      @click="scrollLeft"
    >
      <Icon name="material-symbols:chevron-left" size="2em" />
    </button>
    <button
      v-if="showArrows && images.length > 1"
      type="button"
      class="absolute cursor-pointer -right-4 top-1/2 z-50 -translate-y-1/2 transform rounded-full bg-black/60 p-2 text-white shadow-lg transition-all duration-200 hover:bg-black/80 hover:scale-110 md:-right-6 flex items-center justify-center"
      @click="scrollRight"
    >
      <Icon name="material-symbols:chevron-right" size="2em" />
    </button>

    <div v-if="showArrows && images.length > 1" class="flex justify-center gap-2 pt-1">
      <button
        v-for="(_, i) in images"
        :key="i"
        type="button"
        :aria-label="`Go to slide ${i + 1}`"
        class="rounded-full transition-all duration-300"
        :class="currentIndex === i ? 'w-5 h-2 bg-primary' : 'w-2 h-2 bg-base-content/30 hover:bg-base-content/60'"
        @click="goTo(i)"
      />
    </div>
  </div>

  <Teleport to="body">
    <Transition name="lightbox">
      <div
        v-if="lightboxOpen"
        class="fixed inset-0 z-[200] flex flex-col items-center justify-center bg-black/95"
        @click.self="closeLightbox"
      >
        <div class="absolute top-0 left-0 right-0 flex items-center justify-between px-6 py-4 bg-gradient-to-b from-black/60 to-transparent">
          <span v-if="title" class="text-white/70 text-sm font-medium">{{ title }}</span>
          <span class="text-white/70 text-sm ml-auto mr-4">{{ lightboxIndex + 1 }} / {{ images.length }}</span>
          <button
            type="button"
            class="text-white/70 hover:text-white transition-colors"
            @click="closeLightbox"
          >
            <Icon name="material-symbols:close" size="1.75em" />
          </button>
        </div>

        <Transition :name="lightboxTransition" mode="out-in">
          <NuxtImg
            :key="lightboxIndex"
            :src="getImageUrl(images[lightboxIndex] ?? '')"
            :alt="`Image ${lightboxIndex + 1}`"
            format="webp"
            class="max-h-[85vh] max-w-[90vw] rounded-lg object-contain shadow-2xl"
          />
        </Transition>

        <button
          v-if="images.length > 1"
          type="button"
          class="absolute left-4 top-1/2 -translate-y-1/2 rounded-full bg-white/10 p-3 text-white backdrop-blur-sm transition-all hover:bg-white/25 hover:scale-110 flex items-center justify-center cursor-pointer"
          @click="lightboxPrev"
        >
          <Icon name="material-symbols:chevron-left" size="2em" />
        </button>
        <button
          v-if="images.length > 1"
          type="button"
          class="absolute right-4 top-1/2 -translate-y-1/2 rounded-full bg-white/10 p-3 text-white backdrop-blur-sm transition-all hover:bg-white/25 hover:scale-110 flex items-center justify-center cursor-pointer"
          @click="lightboxNext"
        >
          <Icon name="material-symbols:chevron-right" size="2em" />
        </button>

        <div v-if="images.length > 1" class="absolute bottom-0 left-0 right-0 flex justify-center gap-2 px-4 py-4 bg-gradient-to-t from-black/60 to-transparent overflow-x-auto">
          <button
            v-for="(image, i) in images"
            :key="i"
            type="button"
            class="flex-shrink-0 rounded transition-all duration-200"
            :class="lightboxIndex === i ? 'ring-2 ring-primary opacity-100 scale-110' : 'opacity-50 hover:opacity-80'"
            @click="lightboxIndex = i"
          >
            <NuxtImg
              :src="getImageUrl(image)"
              :alt="`Thumbnail ${i + 1}`"
              format="webp"
              width="64"
              height="48"
              class="h-12 w-16 rounded object-cover"
            />
          </button>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { ref, computed, nextTick, onMounted, onBeforeUnmount } from 'vue'
import Draggable from 'vuedraggable'
import { getImageUrl } from '~/utils/image-helper'

interface Props {
  modelValue?: string[]
  title?: string | null
  width?: string
  height?: string
  showHoverButton?: boolean
  hoverButtonIcon?: string
  draggable?: boolean
  showArrows?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: () => [],
  title: null,
  width: '100%',
  height: 'auto',
  showHoverButton: false,
  hoverButtonIcon: 'material-symbols:delete',
  draggable: false,
  showArrows: false,
})

const emit = defineEmits<{
  'hoverButton': [value: string]
  'update:modelValue': [value: string[]]
}>()

const carousel = ref<HTMLElement | any>(null)
const itemRefs = ref<HTMLElement[]>([])
const currentIndex = ref(0)
const lightboxOpen = ref(false)
const lightboxIndex = ref(0)
const lightboxTransition = ref('slide-next')
let observer: IntersectionObserver | null = null

const images = computed({
  get(): string[] {
    return props.modelValue
  },
  set(value: string[]) {
    emit('update:modelValue', value)
  },
})

function setItemRef(el: HTMLElement | null, index: number) {
  if (el) itemRefs.value[index] = el
}

function getCarouselElement(): HTMLElement | null {
  const el = carousel.value
  return el?.$el ? el.$el : el
}

function scrollLeft() {
  const idx = Math.max(0, currentIndex.value - 1)
  goTo(idx)
}

function scrollRight() {
  const idx = Math.min(images.value.length - 1, currentIndex.value + 1)
  goTo(idx)
}

function goTo(index: number) {
  const item = itemRefs.value[index]
  if (item) {
    item.scrollIntoView({ behavior: 'smooth', block: 'nearest', inline: 'nearest' })
    currentIndex.value = index
  }
}

function setupObserver() {
  if (observer) observer.disconnect()
  const carouselEl = getCarouselElement()
  if (!carouselEl) return

  observer = new IntersectionObserver(
    (entries) => {
      const visible = entries
        .filter(e => e.isIntersecting)
        .sort((a, b) => b.intersectionRatio - a.intersectionRatio)
      const first = visible[0]
      if (first) {
        const idx = itemRefs.value.indexOf(first.target as HTMLElement)
        if (idx !== -1) currentIndex.value = idx
      }
    },
    { root: carouselEl, threshold: 0.6 },
  )

  itemRefs.value.forEach(el => { if (el) observer!.observe(el) })
}

onMounted(async () => {
  await nextTick()
  setupObserver()
  window.addEventListener('keydown', handleKeydown)
})

onBeforeUnmount(() => {
  observer?.disconnect()
  window.removeEventListener('keydown', handleKeydown)
})

function openLightbox(index: number) {
  lightboxIndex.value = index
  lightboxOpen.value = true
  document.body.style.overflow = 'hidden'
}

function closeLightbox() {
  lightboxOpen.value = false
  document.body.style.overflow = ''
}

function lightboxPrev() {
  lightboxTransition.value = 'slide-prev'
  lightboxIndex.value = (lightboxIndex.value - 1 + images.value.length) % images.value.length
}

function lightboxNext() {
  lightboxTransition.value = 'slide-next'
  lightboxIndex.value = (lightboxIndex.value + 1) % images.value.length
}

function handleKeydown(e: KeyboardEvent) {
  if (!lightboxOpen.value) return
  if (e.key === 'Escape') closeLightbox()
  if (e.key === 'ArrowLeft') lightboxPrev()
  if (e.key === 'ArrowRight') lightboxNext()
}
</script>

<style scoped>
.lightbox-enter-active,
.lightbox-leave-active {
  transition: opacity 0.2s ease;
}
.lightbox-enter-from,
.lightbox-leave-to {
  opacity: 0;
}

.slide-next-enter-active,
.slide-next-leave-active,
.slide-prev-enter-active,
.slide-prev-leave-active {
  transition: opacity 0.2s ease, transform 0.2s ease;
}

.slide-next-enter-from {
  opacity: 0;
  transform: translateX(30px);
}
.slide-next-leave-to {
  opacity: 0;
  transform: translateX(-30px);
}

.slide-prev-enter-from {
  opacity: 0;
  transform: translateX(-30px);
}
.slide-prev-leave-to {
  opacity: 0;
  transform: translateX(30px);
}
</style>
