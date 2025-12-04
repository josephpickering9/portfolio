<template>
  <div class="flex w-full flex-col items-center px-4">
    <TagList />
  </div>
</template>

<script setup lang="ts">
import useMeta from '~/app/composables/useMeta'
import TagList from '~/app/components/lists/tag/TagList.vue'

const { updateMeta } = useMeta()

updateMeta({ title: 'Tags', description: 'View all of the tags associated with projects' })
</script>
