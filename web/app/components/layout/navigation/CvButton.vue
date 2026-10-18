<template>
  <a
    :href="CV_PATH"
    download
    class="flex items-center gap-2 rounded-full bg-text px-4 py-2.25 text-sm font-medium whitespace-nowrap text-bg hover:text-bg hover:opacity-90"
  >
    <Icon name="mdi:file-document-outline" />
    Download CV
  </a>
</template>

<script setup lang="ts">
import { CV_PATH } from '~/data/NavItems'
</script>
