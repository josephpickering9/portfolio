<template>
  <div v-if="url" class="mockup-phone m-0 min-w-80 border-primary">
    <div class="camera"></div>
    <div class="display">
      <div class="artboard phone-1 flex !w-auto border-t-[12px] border-black">
        <iframe :src="url" class="w-full" />
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'MockupPhone',
  props: {
    url: {
      type: String,
      required: true,
    },
  },
})
</script>
