<template>
  <div class="flex items-center gap-1">
    <Icon v-if="icon" :name="icon" />
    <span>{{ label }}</span>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'FormButtonInner',
  props: {
    label: {
      type: String,
      default: null,
    },
    icon: {
      type: String,
      default: null,
    },
  },
})
</script>
