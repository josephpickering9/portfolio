<template>
  <div
    v-if="websiteUrl"
    class="-mx-4 flex flex-col gap-4 rounded-box bg-neutral p-4 md:mx-0"
  >
    <h3 class="m-0">Mockup</h3>
    <div class="flex flex-col items-center gap-4 md:flex-row">
      <MockupBrowser class="hidden md:block" :url="websiteUrl" />
      <MockupPhone :url="websiteUrl" />
    </div>
  </div>
</template>

<script setup lang="ts">
import MockupBrowser from '~/components/mockup/MockupBrowser.vue'
import MockupPhone from '~/components/mockup/MockupPhone.vue'

defineProps<{
  websiteUrl?: string
}>()
</script>
