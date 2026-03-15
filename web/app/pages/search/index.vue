<template>
  <div class="w-full flex-grow flex flex-col items-center px-4">
    <AiSearch />
  </div>
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import AiSearch from '~/components/search/AiSearch.vue'

const { updateMeta } = useMeta()
updateMeta({ title: 'AI Search', description: "Ask questions about my work experience." })
</script>
