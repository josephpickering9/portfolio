<template>
  <div class="flex mx-auto flex-col items-center justify-start gap-16 px-4 py-16 min-h-[85vh] bg-base-100">
    <AiSearch />
  </div>
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import AiSearch from '~/components/home/AiSearch.vue'

const { updateMeta } = useMeta()
updateMeta({ title: 'AI Search', description: "Ask usage questions about my work experience." })
</script>
