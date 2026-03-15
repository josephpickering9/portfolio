<template>
  <div v-if="Object.keys(groupedTags).length > 0" class="project-tags flex flex-wrap items-center gap-x-4 gap-y-1">
    <div v-for="(typeTags, type) in groupedTags" :key="type" class="flex flex-col flex-wrap gap-2">
      <small class="block font-mono text-xs font-bold uppercase tracking-wider text-base-content/50">{{ type }}</small>

      <div class="flex flex-wrap gap-2">
        <Tag v-for="(tag, index) in typeTags" :key="index" :tag="tag" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { Tag as TagModel } from '@api'
import Tag from '~/components/tag/Tag.vue'

const props = defineProps<{
  tags: TagModel[]
}>()

const groupedTags = computed(() => {
  const tags = props.tags
  const groupedAndSorted = tags.reduce((acc: Record<string, TagModel[]>, tag: TagModel) => {
    acc[tag.type] = acc[tag.type] ?? []
    acc[tag.type].push(tag)
    acc[tag.type].sort((a: TagModel, b: TagModel) => a.title.localeCompare(b.title))
    return acc
  }, {})
  const sortedGroupTitles = Object.keys(groupedAndSorted).sort((a, b) => a.localeCompare(b))
  return sortedGroupTitles.reduce((acc: Record<string, TagModel[]>, title: string) => {
    acc[title] = groupedAndSorted[title] ?? []
    return acc
  }, {})
})
</script>
