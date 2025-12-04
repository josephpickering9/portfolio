<template>
  <div class="relative -mx-4 flex flex-col gap-4 rounded-box bg-neutral p-4 md:mx-0">
    <h3 v-if="title" class="m-0">{{ title }}</h3>
    <Draggable
      v-if="draggable"
      ref="carousel"
      v-model="images"
      class="carousel carousel-center w-full space-x-4"
      item-key="index"
    >
      <template #item="{ element, index }">
        <div :key="index" class="carousel-item relative w-4/5 sm:w-auto">
          <img
            :src="getImageUrl(element)"
            :alt="`Carousel Image ${index + 1}`"
            :width="width"
            :height="height"
            class="m-0 rounded-box"
          >
          <button v-if="showHoverButton" type="button" class="absolute left-0 top-0 flex h-full w-full items-center justify-center bg-black bg-opacity-20 opacity-0 transition-opacity duration-300 hover:opacity-100" @click="$emit('hoverButton', element)">
            <Icon :name="hoverButtonIcon" size="2em" />
          </button>
        </div>
      </template>
    </Draggable>
    <div v-else ref="carousel" class="carousel carousel-center w-full space-x-4">
      <div v-for="(image, index) in images" :key="index" class="carousel-item relative w-4/5 sm:w-auto">
        <img
          :src="getImageUrl(image)"
          :alt="`Carousel Image ${index + 1}`"
          :width="width"
          :height="height"
          class="m-0 rounded-box"
        >
        <button v-if="showHoverButton" type="button" class="absolute left-0 top-0 flex h-full w-full items-center justify-center bg-black bg-opacity-20 opacity-0 transition-opacity duration-300 hover:opacity-100" @click="$emit('hoverButton', image)">
          <Icon :name="hoverButtonIcon" size="2em" />
        </button>
      </div>
    </div>

    <button v-if="showArrows" type="button" class="absolute -left-4 top-1/2 z-50 -translate-y-1/2 transform rounded-full bg-black bg-opacity-50 p-2 text-white md:-left-6" @click="scrollLeft">
      <Icon name="material-symbols:chevron-left" size="2em" />
    </button>
    <button v-if="showArrows" type="button" class="absolute -right-4 top-1/2 z-50 -translate-y-1/2 transform rounded-full bg-black bg-opacity-50 p-2 text-white md:-right-6" @click="scrollRight">
      <Icon name="material-symbols:chevron-right" size="2em" />
    </button>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import Draggable from 'vuedraggable'
import { getImageUrl } from '~/app/utils/image-helper'

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

// Emits
const emit = defineEmits<{
  'hoverButton': [value: string]
  'update:modelValue': [value: string[]]
}>()

const carousel = ref<any>(null)

const images = computed({
  get(): string[] {
    return props.modelValue
  },
  set(value: string[]) {
    emit('update:modelValue', value)
  },
})

function getCarouselElement() {
  const carouselComponent = carousel.value
  return carouselComponent?.$el ? carouselComponent.$el : carouselComponent
}

function scrollLeft() {
  const carouselElement = getCarouselElement()
  carouselElement?.scrollBy({ left: -100, behavior: 'smooth' })
}

function scrollRight() {
  const carouselElement = getCarouselElement()
  carouselElement?.scrollBy({ left: 100, behavior: 'smooth' })
}
</script>

<style scoped>
</style>
