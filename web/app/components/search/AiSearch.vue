<template>
  <div class="relative mx-auto flex w-full max-w-205 flex-col gap-7 px-7 pb-30" :class="compact ? 'pt-12' : 'pt-20'">
    <div
      class="pointer-events-none absolute top-0 left-1/2 h-105 w-175 max-w-full -translate-x-1/2 bg-[radial-gradient(closest-side,var(--glow),transparent)]"
      aria-hidden="true"
    />
    <AiSearchHeader :compact="compact" />
    <AiSearchInput v-model="query" @submit="ask" />
    <AiSuggestedPrompts :prompts="prompts" @select="ask" @shuffle="shufflePrompts" />
    <AiSearchResult
      v-if="askedQuestion"
      :question="askedQuestion"
      :thinking="thinking"
      :error="failed"
      :answer="answer"
      :citations="citations"
      :citations-loading="aiStore.citedProjectsLoading ?? false"
      @reset="reset"
      @retry="ask(askedQuestion)"
    />
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { sampleSize } from 'lodash-es'
import { useRoute, useRouter } from 'vue-router'
import { useAiStore } from '~/store/AiStore'
import { aiPrompts, SUGGESTED_PROMPT_COUNT } from '~/data/AiPrompts'
import type { AiPrompt } from '~/data/AiPrompts'
import { uniqueBy } from '~/utils/markdown-helper'
import { useViewTransition } from '~/composables/useViewTransition'
import AiSearchHeader from './AiSearchHeader.vue'
import AiSearchInput from './AiSearchInput.vue'
import AiSuggestedPrompts from './AiSuggestedPrompts.vue'
import AiSearchResult from './AiSearchResult.vue'

const route = useRoute()
const router = useRouter()
const aiStore = useAiStore()
const { withViewTransition } = useViewTransition()

const query = ref('')
const prompts = ref<AiPrompt[]>([])
const askedQuestion = ref('')
const thinking = ref(false)
const failed = ref(false)
const answer = ref('')
let requestId = 0

const initialQuestion = readInitialQuestion()
const openedWithQuestion = ref(initialQuestion !== '')
const compact = computed(() => openedWithQuestion.value || askedQuestion.value !== '')

const citations = computed(() => uniqueBy(aiStore.citedProjects, (project) => project.slug))

function readInitialQuestion(): string {
  const value = route.query['q']
  return typeof value === 'string' ? value.trim() : ''
}

function shufflePrompts() {
  prompts.value = sampleSize(aiPrompts, SUGGESTED_PROMPT_COUNT)
}

async function ask(question: string) {
  const id = ++requestId
  const start = () => {
    query.value = question
    askedQuestion.value = question
    thinking.value = true
    failed.value = false
    answer.value = ''
  }
  if (compact.value) start()
  else await withViewTransition(start)
  aiStore.getCitedProjects([])
  router.replace({ query: { q: question } })

  const result = await aiStore.query(question)
  if (id !== requestId) return

  thinking.value = false
  if (aiStore.queryForm.error || !result) {
    failed.value = true
    return
  }

  answer.value = result.answer ?? ''
  aiStore.getCitedProjects(result.citations ?? [])
}

async function reset() {
  requestId++
  await withViewTransition(() => {
    openedWithQuestion.value = false
    query.value = ''
    askedQuestion.value = ''
    thinking.value = false
    failed.value = false
    answer.value = ''
  })
  aiStore.getCitedProjects([])
  shufflePrompts()
  router.replace({ query: {} })
}

onMounted(() => {
  shufflePrompts()
  if (initialQuestion) ask(initialQuestion)
})
</script>
