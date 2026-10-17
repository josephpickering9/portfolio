<template>
  <div class="flex rounded-xl border border-line p-1" role="group" aria-label="View">
    <button
      v-for="option in options"
      :key="option.value"
      type="button"
      class="flex size-10 items-center justify-center rounded-lg text-lg"
      :class="modelValue === option.value ? 'bg-surface2 text-text' : 'text-muted hover:text-text'"
      :aria-label="option.label"
      :aria-pressed="modelValue === option.value"
      @click="emit('update:modelValue', option.value)"
    >
      <Icon :name="option.icon" />
    </button>
  </div>
</template>

<script setup lang="ts">
import { ViewMode } from '~/types/ViewMode'

interface Props {
  modelValue: ViewMode
}

defineProps<Props>()

const emit = defineEmits<{ (e: 'update:modelValue', value: ViewMode): void }>()

const options = [
  { value: ViewMode.CARD, label: 'Grid view', icon: 'heroicons:squares-2x2' },
  { value: ViewMode.TABLE, label: 'List view', icon: 'heroicons:list-bullet' },
]
</script>
