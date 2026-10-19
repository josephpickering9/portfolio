<template>
  <div class="relative mx-auto flex w-full max-w-205 flex-col gap-7 px-7 pb-30" :class="compact ? 'pt-12' : 'pt-20'">
    <div
      class="pointer-events-none absolute top-0 left-1/2 h-105 w-175 max-w-full -translate-x-1/2 bg-[radial-gradient(closest-side,var(--glow),transparent)]"
      aria-hidden="true"
    />
    <AiSearchHeader :compact="compact" />
    <AiSearchInput v-model="query" @submit="ask" />
    <AiSuggestedPrompts :prompts="prompts" @select="ask" @shuffle="shufflePrompts" />
    <AiSearchResult v-if="askedQuestion" :result="result" @reset="reset" @retry="ask(askedQuestion)" />
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAiSearchStore } from '~/store/AiSearchStore'
import { useSuggestedPrompts } from '~/composables/useSuggestedPrompts'
import { useViewTransition } from '~/composables/useViewTransition'
import type { AiAnswer } from '~/types/AiAnswer'
import AiSearchHeader from './AiSearchHeader.vue'
import AiSearchInput from './AiSearchInput.vue'
import AiSuggestedPrompts from './AiSuggestedPrompts.vue'
import AiSearchResult from './AiSearchResult.vue'

const PROMPTS_STATE_KEY = 'search-prompts'
const QUESTION_QUERY_KEY = 'q'

const route = useRoute()
const router = useRouter()
const aiSearchStore = useAiSearchStore()
const { withViewTransition } = useViewTransition()
const { prompts, shufflePrompts } = useSuggestedPrompts(PROMPTS_STATE_KEY)

const initialQuestion = readInitialQuestion()

const query = ref('')
const askedQuestion = ref('')
const openedWithQuestion = ref(initialQuestion !== '')

const compact = computed(() => openedWithQuestion.value || askedQuestion.value !== '')
const result = computed<AiAnswer>(() => ({
  question: askedQuestion.value,
  thinking: aiSearchStore.queryPending,
  failed: Boolean(aiSearchStore.queryError),
  answer: aiSearchStore.answer,
  citations: aiSearchStore.citedProjects,
  citationsLoading: aiSearchStore.citedProjectsLoading,
}))

function readInitialQuestion(): string {
  const value = route.query[QUESTION_QUERY_KEY]
  return typeof value === 'string' ? value.trim() : ''
}

async function ask(question: string) {
  const start = () => {
    query.value = question
    askedQuestion.value = question
    aiSearchStore.reset()
  }
  if (compact.value) start()
  else await withViewTransition(start)

  await router.replace({ query: { [QUESTION_QUERY_KEY]: question } })
  await aiSearchStore.query(question)
}

async function reset() {
  await withViewTransition(() => {
    openedWithQuestion.value = false
    query.value = ''
    askedQuestion.value = ''
    aiSearchStore.reset()
  })
  shufflePrompts()
  await router.replace({ query: {} })
}

onMounted(() => {
  if (initialQuestion) ask(initialQuestion)
})
</script>
