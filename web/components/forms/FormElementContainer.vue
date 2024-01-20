<template>
  <label class="form-control w-full">
    <div v-if="label" class="label">
      <span class="label-text">{{ label }}</span>
    </div>
    <slot />
  </label>
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
