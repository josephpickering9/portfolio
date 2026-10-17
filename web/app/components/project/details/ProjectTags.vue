<template>
  <ul class="m-0 grid list-none grid-cols-[repeat(auto-fit,minmax(min(100%,200px),1fr))] gap-x-6 gap-y-0.5 p-0 text-[15px]">
    <li v-for="row in rows" :key="row.id" class="flex justify-between gap-3 border-b border-line py-3">
      <span class="flex items-center gap-2">
        <Icon v-if="row.icon" :name="row.icon" />
        {{ row.title }}
      </span>
      <span class="pt-0.5 font-mono text-xs tracking-[0.05em] text-muted uppercase">{{ row.type }}</span>
    </li>
  </ul>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { Tag } from '@api'
import { tagTypes } from '~/data/TagTypes'

const props = defineProps<{
  tags: Tag[]
}>()

const rows = computed(() =>
  props.tags.map((tag) => ({
    id: tag.id,
    title: tag.title,
    type: tag.type,
    icon: tag.icon || tagTypes.find((type) => type.value === tag.type)?.icon,
  })),
)
</script>
