<template>
  <div class="flex flex-wrap gap-2">
    <FilterChip
      label="All"
      icon="heroicons:squares-2x2"
      :active="!modelValue"
      data-testid="type-filter-all"
      @select="emit('update:modelValue', undefined)"
    />
    <FilterChip
      v-for="type in visibleTypes"
      :key="type.value"
      :label="type.label"
      :icon="type.icon"
      :active="modelValue === type.value"
      :data-testid="`type-filter-${type.value}`"
      @select="emit('update:modelValue', type.value)"
    />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { TagType } from '@api'
import { tagTypes } from '~/data/TagTypes'
import FilterChip from '~/components/ui/filter/FilterChip.vue'

interface Props {
  modelValue?: TagType
  available: Set<TagType>
}

const props = defineProps<Props>()

const emit = defineEmits<{ (e: 'update:modelValue', value: TagType | undefined): void }>()

const visibleTypes = computed(() => tagTypes.filter((type) => props.available.has(type.value)))
</script>
