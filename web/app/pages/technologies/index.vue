<template>
  <div class="flex w-full flex-col items-center px-4">
    <TagList />
  </div>
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import TagList from '~/components/tag/list/TagList.vue'

const { updateMeta } = useMeta()

updateMeta({ title: 'Technologies', description: 'View all of the technologies associated with projects' })
</script>
