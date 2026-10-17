<template>
  <TagList />
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import TagList from '~/components/tag/list/TagList.vue'

const { updateMeta } = useMeta()

updateMeta({ title: 'Technologies', description: 'The languages, frameworks and services behind my projects.' })
</script>
