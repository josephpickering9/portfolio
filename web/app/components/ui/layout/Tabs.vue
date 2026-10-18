<template>
  <div role="tablist" class="flex gap-1 overflow-x-auto border-b border-line p-2">
    <button
      v-for="(tab, index) in tabs"
      :id="tabId(index)"
      :key="tab.label"
      type="button"
      role="tab"
      :aria-selected="index === activeTab"
      :aria-controls="tabPanelId(index)"
      class="flex items-center gap-2 rounded-[10px] px-4 py-2.5 text-sm font-medium whitespace-nowrap"
      :class="index === activeTab ? 'bg-surface2 text-text' : 'text-muted hover:text-text'"
      @click="emit('update:activeTab', index)"
    >
      {{ tab.label }}
      <span v-if="tab.hasError" class="size-1.75 rounded-full bg-danger" aria-label="Has errors" />
    </button>
  </div>
</template>

<script setup lang="ts">
import type { TabItem } from '~/types/TabItem'
import { tabId, tabPanelId } from '~/utils/tab-helper'

interface Props {
  tabs: TabItem[]
  activeTab: number
}

defineProps<Props>()

const emit = defineEmits<{ (e: 'update:activeTab', value: number): void }>()
</script>
