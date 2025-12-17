<template>
  <div 
    class="w-full transition-all duration-700 ease-in-out flex flex-col gap-4 md:gap-8"
    :class="[
      hasSearched || searchResult ? 'max-w-4xl py-6 md:py-8' : 'max-w-2xl translate-y-[-10vh]'
    ]"
  >
    <AiSearchHeader :has-searched="hasSearched || !!searchResult" />

    <div class="w-full relative z-20">
      <AiSearchInput 
        v-model="searchQuery"
        :loading="aiStore.queryForm.loading ?? false"
        :has-searched="hasSearched || !!searchResult"
        @search="performSearch"
      />

      <AiSuggestedPrompts 
        :prompts="displayedPrompts"
        :loading="aiStore.queryForm.loading ?? false"
        :has-searched="hasSearched || !!searchResult"
        @select="usePrompt"
      />
    </div>

    <AiSearchResult 
      v-if="hasSearched || searchResult"
      :loading="aiStore.queryForm.loading ?? false"
      :result="searchResult"
      :error="aiStore.queryForm.error"
      :citations="aiStore.citedProjects"
      :citations-loading="aiStore.citedProjectsLoading ?? false"
      @reset="resetSearch"
      @dismiss-error="aiStore.queryForm.error = undefined"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { sampleSize } from 'lodash-es'
import { useAiStore } from '~/store/AiStore'
import type { PostVertexQueryResponse } from '~~/api'
import AiSearchHeader from './AiSearchHeader.vue'
import AiSearchInput from './AiSearchInput.vue'
import AiSuggestedPrompts from './AiSuggestedPrompts.vue'
import AiSearchResult from './AiSearchResult.vue'

const aiStore = useAiStore()
const searchQuery = ref('')
const searchResult = ref<string | null>(null)
const displayedPrompts = ref<Array<{ text: string, icon: string }>>([])
const hasSearched = ref(false)

const allPrompts = [
  { text: "What project is Joe most proud of?", icon: "heroicons:trophy" },
  { text: "What React projects has Joe worked on?", icon: "logos:react" },
  { text: "What's the longest project been?", icon: "heroicons:clock" },
  { text: "Summarize Joe's work history.", icon: "heroicons:clipboard-document-list" },
  { text: "What technologies does Joe use most?", icon: "heroicons:code-bracket" },
  { text: "What Python projects has Joe worked on?", icon: "mdi:language-python" },
  { text: "Describe Joe's experience with AWS.", icon: "simple-icons:amazonaws" },
  { text: "What has Joe built with Next.js?", icon: "simple-icons:nextdotjs" },
  { text: "Tell me about Joe's .NET background.", icon: "simple-icons:dotnet" },
  { text: "Which projects use Vue?", icon: "mdi:vuejs" },
  { text: "Show me projects using TypeScript.", icon: "mdi:language-typescript" },
  { text: "What Flutter apps has Joe built?", icon: "simple-icons:flutter" },
  { text: "Has Joe worked with databases like PostgreSQL?", icon: "simple-icons:postgresql" },
  { text: "What about mobile development with Android?", icon: "mdi:android" }
]

onMounted(() => {
  displayedPrompts.value = sampleSize(allPrompts, 3)
})

async function performSearch() {
  if (!searchQuery.value) return
  
  hasSearched.value = true
  searchResult.value = null
  
  await aiStore.query(searchQuery.value)
  const result = aiStore.queryForm.data as PostVertexQueryResponse | undefined
  
  if (result) {
    const finalResult = result.answer || ''
    
    if (result.citations?.length) {
        aiStore.getCitedProjects(result.citations)
    } else {
        aiStore.getCitedProjects([]) // Clear any previous citations if no new ones
    }
    
    searchResult.value = finalResult
    // Typing logic moved to AiSearchResult
  }
}

function usePrompt(prompt: string) {
  searchQuery.value = prompt
  performSearch()
}

function resetSearch() {
  searchQuery.value = ''
  searchResult.value = null
  aiStore.getCitedProjects([]) // Clear store state
  hasSearched.value = false
  displayedPrompts.value = sampleSize(allPrompts, 3)
}
</script>
