<template>
  <AiSearch />
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import AiSearch from '~/components/search/AiSearch.vue'

const { updateMeta } = useMeta()

updateMeta({ title: 'AI Search', description: 'Ask questions about my work experience, projects and skills.' })
</script>
