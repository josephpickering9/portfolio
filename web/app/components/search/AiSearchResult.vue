<template>
  <div
    class="relative flex flex-col gap-5.5 rounded-[18px] border border-line bg-surface p-6.5 [view-transition-name:ai-search-answer]"
    data-testid="ai-search-result"
  >
    <div class="flex flex-wrap items-baseline justify-between gap-3">
      <span class="flex-[1_1_300px] font-display text-[28px] leading-[1.15] text-balance" data-testid="ai-search-question">
        {{ result.question }}
      </span>
      <button
        type="button"
        class="rounded-full border border-line px-3 py-1.75 text-[13px] text-muted hover:text-text"
        data-testid="ai-search-reset"
        @click="emit('reset')"
      >
        New search
      </button>
    </div>

    <div v-if="result.thinking" class="flex flex-col gap-3.5" aria-live="polite" data-testid="ai-search-thinking">
      <div class="flex items-center gap-3 text-sm text-muted">
        <span class="flex gap-1.5" aria-hidden="true">
          <span v-for="delay in DOT_DELAY_CLASSES" :key="delay" class="pf-dot size-2 rounded-full bg-accent" :class="delay" />
        </span>
        <span>Searching my projects…</span>
      </div>
      <div v-for="width in SHIMMER_WIDTH_CLASSES" :key="width" class="skeleton-pulse h-3.5 rounded-md bg-surface2" :class="width" />
    </div>

    <div
      v-else-if="result.failed"
      class="flex flex-wrap items-center justify-between gap-3.5 rounded-xl border border-line bg-surface2 px-4.5 py-4"
      role="alert"
      data-testid="ai-search-error"
    >
      <div class="flex items-center gap-2.5">
        <Icon name="heroicons:exclamation-triangle" size="20" class="text-accent" />
        <span>AI search is unavailable right now.</span>
      </div>
      <div class="flex gap-2">
        <button
          type="button"
          class="rounded-full bg-accent px-3.5 py-2 text-sm font-medium text-accent-ink"
          data-testid="ai-search-retry"
          @click="emit('retry')"
        >
          Try again
        </button>
        <NuxtLink to="/projects" class="rounded-full border border-line px-3.5 py-2 text-sm">Browse projects</NuxtLink>
      </div>
    </div>

    <template v-else-if="result.answer">
      <div class="rich-text text-[17px] leading-[1.7]" aria-live="polite" data-testid="ai-search-answer">
        <div class="contents" v-html="renderedAnswer" />
        <span v-if="revealing" class="text-accent" aria-hidden="true">▍</span>
      </div>

      <div v-if="!revealing && result.citations.length" class="flex flex-col gap-3 border-t border-line pt-5">
        <span class="eyebrow text-muted">{{ formatCount(result.citations.length, 'cited project') }}</span>
        <div class="grid grid-cols-[repeat(auto-fill,minmax(min(100%,220px),1fr))] gap-3">
          <NuxtLink
            v-for="project in result.citations"
            :key="project.slug"
            :to="`/projects/${project.slug}`"
            class="flex flex-col overflow-hidden rounded-xl border border-line bg-bg text-text transition-[border-color,transform] duration-300 hover:-translate-y-0.75 hover:border-accent hover:text-text"
            data-testid="ai-search-citation"
          >
            <NuxtImg
              v-if="project.cardImageUrl"
              format="webp"
              :src="getImageUrl(project.cardImageUrl)"
              :alt="project.title"
              width="260"
              height="146"
              fit="cover"
              :modifiers="{ position: 'left top' }"
              sizes="100vw sm:50vw md:260px"
              loading="lazy"
              class="block aspect-video w-full bg-surface2 object-cover object-left-top"
            />
            <div v-else class="aspect-video bg-surface2" />
            <div class="flex flex-col gap-1 px-3.5 py-3">
              <span class="font-semibold">{{ project.title }}</span>
              <span class="text-[13px] leading-[1.45] text-muted">{{ project.shortDescription }}</span>
            </div>
          </NuxtLink>
        </div>
      </div>
      <span v-else-if="!revealing && !result.citationsLoading" class="text-sm text-muted">No projects were cited for this answer.</span>
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed, onBeforeUnmount, ref, watch } from 'vue'
import type { AiAnswer } from '~/types/AiAnswer'
import { useReducedMotion } from '~/composables/useReducedMotion'
import { useImageUrl } from '~/composables/useImageUrl'
import { renderBasicMarkdown } from '~/utils/markdown-helper'
import { formatCount } from '~/utils/string-helper'

interface Props {
  result: AiAnswer
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'reset' | 'retry'): void
}>()

const DOT_DELAY_CLASSES = ['[--dot-delay:0ms]', '[--dot-delay:150ms]', '[--dot-delay:300ms]']
const SHIMMER_WIDTH_CLASSES = ['w-[92%]', 'w-[78%]', 'w-[64%]']
const CHARS_PER_TICK = 7
const TICK_MS = 16

const { prefersReducedMotion } = useReducedMotion()
const { getImageUrl } = useImageUrl()

const shown = ref(0)
let timer: ReturnType<typeof setInterval> | undefined

const revealing = computed(() => shown.value < props.result.answer.length)
const renderedAnswer = computed(() => renderBasicMarkdown(props.result.answer.slice(0, shown.value)))

function stopReveal() {
  if (timer) clearInterval(timer)
  timer = undefined
}

function startReveal() {
  stopReveal()
  shown.value = 0
  if (prefersReducedMotion()) {
    shown.value = props.result.answer.length
    return
  }
  timer = setInterval(() => {
    shown.value = Math.min(props.result.answer.length, shown.value + CHARS_PER_TICK)
    if (!revealing.value) stopReveal()
  }, TICK_MS)
}

onBeforeUnmount(stopReveal)

watch(
  () => props.result.answer,
  (answer) => {
    if (answer) startReveal()
    else stopReveal()
  },
  { immediate: true },
)
</script>
