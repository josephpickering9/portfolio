<template>
  <div class="relative flex min-h-10 flex-wrap justify-center gap-2 [view-transition-name:ai-search-prompts]">
    <button
      v-for="prompt in prompts"
      :key="prompt.text"
      type="button"
      class="flex items-center gap-2 rounded-full border border-line bg-surface px-3.5 py-2.25 text-left text-sm text-text transition-colors duration-200 hover:border-accent"
      data-testid="suggested-prompt"
      @click="emit('select', prompt.text)"
    >
      <Icon :name="prompt.icon" class="shrink-0" />
      {{ prompt.text }}
    </button>
    <button
      type="button"
      class="flex items-center gap-1.5 rounded-full border border-dashed border-line px-3.5 py-2.25 text-sm text-muted hover:text-text"
      aria-label="More prompts"
      @click="emit('shuffle')"
    >
      <Icon name="mdi:shuffle-variant" />
      More
    </button>
  </div>
</template>

<script setup lang="ts">
import type { AiPrompt } from '~/data/AiPrompts'

interface Props {
  prompts: AiPrompt[]
}

defineProps<Props>()

const emit = defineEmits<{
  (e: 'select', value: string): void
  (e: 'shuffle'): void
}>()
</script>
