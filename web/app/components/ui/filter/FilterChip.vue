<template>
  <div class="badge badge-lg gap-1.5 pr-1 border transition-all hover:scale-[1.02]" :class="badgeColorClass">
    <Icon v-if="filter.icon" :name="filter.icon" size="1.1em" class="opacity-90" />
    <NuxtImg v-if="filter.logo" :src="getImageUrl(filter.logo)" placeholder format="webp" class="opacity-90 h-4 w-4" />
    <span class="opacity-90 font-normal">{{ displayValue }}</span>
    <button
      type="button"
      class="btn btn-circle btn-ghost btn-xs ml-0.5 opacity-60 hover:opacity-100 transition-opacity"
      @click="$emit('remove')"
    >
      <Icon name="heroicons:x-mark" size="1em" />
    </button>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { Icon } from '#components'
import type { Filter } from '~/types/Filter'
import { FilterType } from '~/types/FilterType'
import { getImageUrl } from '~/utils/image-helper'

interface Props {
  filter: Filter
}

const props = defineProps<Props>()

defineEmits<{
  'remove': []
}>()

const displayValue = computed(() => {
  return props.filter.displayValue || props.filter.value
})

const badgeColorClass = computed(() => {
  switch (props.filter.type) {
    case FilterType.SEARCH:
      return 'bg-info/10 text-info border-info/20 hover:bg-info/20'
    case FilterType.COMPANY:
      return 'bg-success/10 text-success border-success/20 hover:bg-success/20'
    case FilterType.TAG_TYPE:
      return 'bg-warning/10 text-warning border-warning/20 hover:bg-warning/20'
    case FilterType.TAG:
      return 'bg-secondary/10 text-secondary border-secondary/20 hover:bg-secondary/20'
    default:
      return 'bg-primary/10 text-primary border-primary/20 hover:bg-primary/20'
  }
})
</script>
