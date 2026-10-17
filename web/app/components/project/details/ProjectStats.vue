<template>
  <dl v-if="sortedStats.length" class="m-0 flex flex-wrap gap-x-10 gap-y-6 border-t border-line pt-6" data-testid="project-stats">
    <div v-for="stat in sortedStats" :key="stat.id" class="flex flex-col gap-1" data-testid="project-stat">
      <dt class="eyebrow order-2 text-muted">{{ stat.label }}</dt>
      <dd class="order-1 m-0 font-display text-[44px] leading-none text-text">{{ stat.value }}</dd>
    </div>
  </dl>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { sortBy } from 'lodash-es'
import type { ProjectStat } from '@api'

const props = defineProps<{
  stats: ProjectStat[]
}>()

const sortedStats = computed(() => sortBy(props.stats, (stat) => stat.order ?? 0))
</script>
