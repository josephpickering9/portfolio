<template>
  <div v-if="url" class="mockup-phone m-0 min-w-80 border-primary">
    <div class="camera"/>
    <div class="display">
      <div class="artboard phone-1 flex !w-auto border-t-[12px] border-black">
        <iframe :src="url" name="Mockup Phone" class="w-full" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
interface Props {
  url: string
}

defineProps<Props>()
</script>
