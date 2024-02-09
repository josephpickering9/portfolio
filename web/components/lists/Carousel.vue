<template>
  <div class="-mx-4 flex flex-col gap-4 rounded-box bg-neutral p-4 md:mx-0">
    <h3 v-if="title" class="m-0">{{ title }}</h3>
    <div class="carousel carousel-center w-full space-x-4">
      <div v-for="(image, index) in images" :key="index" class="carousel-item w-4/5 sm:w-auto">
        <img :src="getImageUrl(image)" :width="width" :height="height" class="m-0 rounded-box" />
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import type { PropType } from 'vue'
import { defineComponent } from 'vue'
import { getImageUrl } from '../../utils/image-helper'

export default defineComponent({
  name: 'Carousel',
  props: {
    images: {
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
  },
  methods: {
    getImageUrl,
  },
})
</script>
