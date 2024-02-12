<template>
  <label v-if="label" class="form-control w-full">
    <div class="label">
      <span class="label-text">{{ label }}</span>
    </div>
    <slot />
  </label>
  <div v-else class="relative w-full">
    <slot />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'FormElementContainer',
  props: {
    label: {
      type: String,
      default: null,
    },
  },
})
</script>
