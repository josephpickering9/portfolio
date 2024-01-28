<template>
  <div v-if="url" class="mockup-browser h-full w-full border bg-base-300">
    <div class="mockup-browser-toolbar">
      <div class="input">{{ url }}</div>
    </div>
    <div class="flex h-full min-h-96 w-full bg-base-200">
      <iframe :src="url" class="w-full" />
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'MockupBrowser',
  props: {
    url: {
      type: String,
      required: true,
    },
  },
})
</script>
