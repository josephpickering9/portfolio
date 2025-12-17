<template>
  <div class="flex mx-auto flex-col items-center justify-center min-h-[85vh] transition-all duration-500 ease-in-out px-4">
    <AiSearch />
  </div>
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import AiSearch from '~/components/search/AiSearch.vue'

const { updateMeta } = useMeta()
updateMeta({ title: 'AI Search', description: "Ask usage questions about my work experience." })
</script>
