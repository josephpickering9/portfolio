<template>
  <div class="flex flex-col gap-4">
    <Carousel
      v-if="desktopImages.length"
      :model-value="desktopImages"
      title="Desktop"
      width="1007"
      height="508"
      :show-arrows="true"
      @update:model-value="$emit('update:desktopImages', $event)"
    />
    <Carousel
      v-if="mobileImages.length"
      :model-value="mobileImages"
      title="Mobile"
      width="335"
      height="673"
      :show-arrows="true"
      @update:model-value="$emit('update:mobileImages', $event)"
    />
  </div>
</template>

<script setup lang="ts">
import Carousel from '~/components/layout/Carousel.vue'

defineProps<{
  desktopImages: string[]
  mobileImages: string[]
}>()

defineEmits<{
  (e: 'update:desktopImages' | 'update:mobileImages', value: string[]): void
}>()
</script>
