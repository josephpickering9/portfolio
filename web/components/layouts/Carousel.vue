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
          />
          <button v-if="showHoverButton" type="button" class="carousel-hover-button" @click="$emit('hoverButton', element)">
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
        />
        <button v-if="showHoverButton" type="button" class="carousel-hover-button" @click="$emit('hoverButton', image)">
          <Icon :name="hoverButtonIcon" size="2em" />
        </button>
      </div>
    </div>

    <button v-if="showArrows" type="button" class="carousel-arrow-left" @click="scrollLeft">
      <Icon name="material-symbols:chevron-left" size="2em" />
    </button>
    <button v-if="showArrows" type="button" class="carousel-arrow-right" @click="scrollRight">
      <Icon name="material-symbols:chevron-right" size="2em" />
    </button>
  </div>
</template>

<script lang="ts">
import type { PropType } from 'vue'
import { defineComponent } from 'vue'
import Draggable from 'vuedraggable'
import { getImageUrl } from '../../utils/image-helper'
import { Icon } from '#components'

export default defineComponent({
  name: 'Carousel',
  components: { Draggable, Icon },
  props: {
    modelValue: {
      type: Array as PropType<string[]>,
      default: () => [],
    },
    title: {
      type: String,
      default: null,
    },
    width: {
      type: String,
      default: '100%',
    },
    height: {
      type: String,
      default: 'auto',
    },
    showHoverButton: {
      type: Boolean,
      default: false,
    },
    hoverButtonIcon: {
      type: String,
      default: 'material-symbols:delete',
    },
    draggable: {
      type: Boolean,
      default: false,
    },
    showArrows: {
      type: Boolean,
      default: false,
    },
  },
  emits: ['hoverButton', 'update:modelValue'],
  computed: {
    images: {
      get(): string[] {
        return this.modelValue
      },
      set(value: string[]) {
        this.$emit('update:modelValue', value)
      },
    },
  },
  methods: {
    getImageUrl,
    getCarouselElement() {
      const carouselComponent: any = this.$refs.carousel
      return carouselComponent.$el ? carouselComponent.$el : carouselComponent
    },
    scrollLeft() {
      const carouselElement = this.getCarouselElement()
      carouselElement.scrollBy({ left: -100, behavior: 'smooth' })
    },
    scrollRight() {
      const carouselElement = this.getCarouselElement()
      carouselElement.scrollBy({ left: 100, behavior: 'smooth' })
    },
  },
})
</script>

<style scoped>
.carousel-hover-button {
  @apply absolute left-0 top-0 flex h-full w-full items-center justify-center bg-black bg-opacity-20;
  @apply opacity-0 transition-opacity duration-300 hover:opacity-100;
}

.carousel-arrow-left,
.carousel-arrow-right {
  @apply absolute top-1/2 -translate-y-1/2 transform;
  @apply bg-black bg-opacity-50 text-white;
  @apply z-50 rounded-full p-2;
}

.carousel-arrow-left {
  @apply -left-4 md:-left-6;
}

.carousel-arrow-right {
  @apply -right-4 md:-right-6;
}
</style>
