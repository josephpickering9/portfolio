<template>
  <div class="relative flex flex-col gap-5.5 rounded-[18px] border border-line bg-surface p-6.5 [view-transition-name:ai-search-answer]">
    <div class="flex flex-wrap items-baseline justify-between gap-3">
      <span class="flex-[1_1_300px] font-display text-[28px] leading-[1.15] text-balance">{{ question }}</span>
      <button type="button" class="rounded-full border border-line px-3 py-1.75 text-[13px] text-muted hover:text-text" @click="emit('reset')">
        New search
      </button>
    </div>

    <div v-if="thinking" class="flex flex-col gap-3.5" aria-live="polite">
      <div class="flex items-center gap-3 text-sm text-muted">
        <span class="flex gap-1.5" aria-hidden="true">
          <span
            v-for="dot in 3"
            :key="dot"
            class="size-2 rounded-full bg-accent"
            :style="{ animation: `pf-dot 1.2s ${(dot - 1) * 0.15}s ease-in-out infinite` }"
          />
        </span>
        <span>Searching my projects…</span>
      </div>
      <div v-for="width in SHIMMER_WIDTHS" :key="width" class="skeleton-pulse h-3.5 rounded-md bg-surface2" :style="{ width }" />
    </div>

    <div
      v-else-if="error"
      class="flex flex-wrap items-center justify-between gap-3.5 rounded-xl border border-line bg-surface2 px-4.5 py-4"
      role="alert"
    >
      <div class="flex items-center gap-2.5">
        <Icon name="heroicons:exclamation-triangle" size="20" class="text-accent" />
        <span>AI search is unavailable right now.</span>
      </div>
      <div class="flex gap-2">
        <button type="button" class="rounded-full bg-accent px-3.5 py-2 text-sm font-medium text-accent-ink" @click="emit('retry')">
          Try again
        </button>
        <NuxtLink to="/projects" class="rounded-full border border-line px-3.5 py-2 text-sm">Browse projects</NuxtLink>
      </div>
    </div>

    <template v-else-if="answer">
      <div class="rich-text text-[17px] leading-[1.7]" aria-live="polite">
        <div class="contents" v-html="renderedAnswer" />
        <span v-if="revealing" class="text-accent" aria-hidden="true">▍</span>
      </div>

      <div v-if="!revealing && citations.length" class="flex flex-col gap-3 border-t border-line pt-5">
        <span class="eyebrow text-muted">{{ citations.length }} cited {{ citations.length === 1 ? 'project' : 'projects' }}</span>
        <div class="grid grid-cols-[repeat(auto-fill,minmax(min(100%,220px),1fr))] gap-3">
          <NuxtLink
            v-for="project in citations"
            :key="project.slug"
            :to="`/projects/${project.slug}`"
            class="flex flex-col overflow-hidden rounded-xl border border-line bg-bg text-text transition-[border-color,transform] duration-300 hover:-translate-y-0.75 hover:border-accent hover:text-text"
          >
            <NuxtImg
v-if="getCardImage(project)"
              format="webp"
              :src="getImageUrl(getCardImage(project))"
              :alt="project.title"
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
      <span v-else-if="!revealing && !citationsLoading" class="text-sm text-muted">No projects were cited for this answer.</span>
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed, onBeforeUnmount, ref, watch } from 'vue'
import type { Project } from '@api'
import { getImageUrl } from '~/utils/image-helper'
import { getCardImage } from '~/utils/project-helper'
import { renderBasicMarkdown } from '~/utils/markdown-helper'

const SHIMMER_WIDTHS = ['92%', '78%', '64%']
const CHARS_PER_TICK = 7
const TICK_MS = 16

const props = defineProps<{
  question: string
  thinking: boolean
  error: boolean
  answer: string
  citations: Project[]
  citationsLoading: boolean
}>()

const emit = defineEmits<{
  (e: 'reset' | 'retry'): void
}>()

const shown = ref(0)
let timer: ReturnType<typeof setInterval> | undefined

const revealing = computed(() => shown.value < props.answer.length)
const renderedAnswer = computed(() => renderBasicMarkdown(props.answer.slice(0, shown.value)))

function stopReveal() {
  if (timer) clearInterval(timer)
  timer = undefined
}

function startReveal() {
  stopReveal()
  shown.value = 0
  if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
    shown.value = props.answer.length
    return
  }
  timer = setInterval(() => {
    shown.value = Math.min(props.answer.length, shown.value + CHARS_PER_TICK)
    if (!revealing.value) stopReveal()
  }, TICK_MS)
}

watch(
  () => props.answer,
  (answer) => {
    if (answer) startReveal()
    else stopReveal()
  },
  { immediate: true },
)

onBeforeUnmount(stopReveal)
</script>
