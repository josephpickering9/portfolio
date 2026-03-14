<template>
  <div v-if="url" class="mockup-phone m-0 min-w-80 border-primary">
    <div class="mockup-phone-camera"/>
    <div class="mockup-phone-display">
      <iframe :src="url" name="Mockup Phone" class="w-full h-full" />
    </div>
  </div>
</template>

<script setup lang="ts">
interface Props {
  url: string
}

defineProps<Props>()
</script>
