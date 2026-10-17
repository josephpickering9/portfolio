<template>
  <div class="flex max-w-180 flex-col gap-3.5">
    <form
      class="flex items-center gap-3 rounded-2xl border border-line bg-surface py-2 pr-2 pl-5 shadow-[0_20px_60px_-20px_var(--glow)]"
      role="search"
      @submit.prevent="ask(query)"
    >
      <Icon name="mdi:robot-happy-outline" size="22" class="shrink-0 text-accent" />
      <input
        v-model="query"
        type="text"
        aria-label="Ask about my work"
        placeholder="Ask about my work, projects or skills…"
        class="min-w-0 flex-1 border-0 bg-transparent py-2.5 text-[17px] text-text outline-none"
      >
      <button type="submit" class="rounded-[10px] bg-accent px-5 py-3 text-[15px] font-medium text-accent-ink">Ask</button>
    </form>
    <div class="flex min-h-8.25 flex-wrap gap-2">
      <button
        v-for="prompt in prompts"
        :key="prompt.text"
        type="button"
        class="flex items-center gap-1.5 rounded-full border border-line px-3.25 py-1.75 text-left text-[13px] text-muted transition-colors duration-200 hover:border-accent hover:text-text"
        @click="ask(prompt.text)"
      >
        <Icon :name="prompt.icon" class="shrink-0" />
        {{ prompt.text }}
      </button>
      <button
        type="button"
        class="flex items-center gap-1.5 rounded-full border border-dashed border-line px-3.25 py-1.75 text-[13px] text-muted transition-colors duration-200 hover:text-text"
        aria-label="More prompts"
        @click="shufflePrompts"
      >
        <Icon name="mdi:shuffle-variant" />
        More
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from 'vue'
import { sampleSize } from 'lodash-es'
import { aiPrompts, SUGGESTED_PROMPT_COUNT } from '~/data/AiPrompts'
import type { AiPrompt } from '~/data/AiPrompts'

const query = ref('')
const prompts = ref<AiPrompt[]>([])

function ask(text: string) {
  const question = text.trim()
  if (!question) return
  navigateTo({ path: '/search', query: { q: question } })
}

function shufflePrompts() {
  prompts.value = sampleSize(aiPrompts, SUGGESTED_PROMPT_COUNT)
}

onMounted(shufflePrompts)
</script>
