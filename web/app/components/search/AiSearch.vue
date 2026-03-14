<template>
  <div
    class="w-full max-w-4xl flex flex-col gap-4 md:gap-8 transition-all duration-700 pb-2 ease-in-out"
    :class="hasSearched || searchResult ? 'pt-8 md:pt-10 mb-auto' : 'my-auto'"
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
const displayedPrompts = ref<Array<{ text: string, icon: string, color?: string }>>([])
const hasSearched = ref(false)

// TODO: Resolve all commented out prompts
const allPrompts = [
  // { text: "What project is Joe most proud of?", icon: "heroicons:trophy" },
  { text: "What React projects has Joe worked on?", icon: "logos:react", color: "#61DAFB" },
  // { text: "What's the most complex project Joe has worked on?", icon: "heroicons:clock" },
  // { text: "Summarize Joe's work history.", icon: "heroicons:clipboard-document-list" },
  { text: "What technologies does Joe use most?", icon: "heroicons:code-bracket" },
  // { text: "What Python projects has Joe worked on?", icon: "mdi:language-python", color: "#3776AB" },
  { text: "What has Joe built with Next.js?", icon: "simple-icons:nextdotjs", color: "#ffffff" },
  { text: "Tell me about Joe's .NET background.", icon: "simple-icons:dotnet", color: "#512BD4" },
  { text: "Which projects use Vue?", icon: "mdi:vuejs", color: "#42B883" },
  { text: "Show me projects using TypeScript.", icon: "mdi:language-typescript", color: "#3178C6" },
  { text: "What Flutter apps has Joe built?", icon: "simple-icons:flutter", color: "#02569B" },
  // { text: "Has Joe worked with databases like PostgreSQL?", icon: "simple-icons:postgresql", color: "#4169E1" },
  { text: "What about mobile development with Android?", icon: "mdi:android", color: "#3DDC84" },
  { text: "What PHP projects has Joe worked on?", icon: "mdi:language-php", color: "#556097" },
  // { text: "What fintech experience does Joe have?", icon: "heroicons:banknotes" },
  { text: "What AI projects has Joe worked on?", icon: "heroicons:cpu-chip" },
  // { text: "Tell me about Joe's experience with GCP.", icon: "simple-icons:googlecloud", color: "#4285F4" },
  // { text: "What authentication systems has Joe used?", icon: "heroicons:lock-closed" },
  { text: "Has Joe worked on any mobile apps?", icon: "heroicons:device-phone-mobile" },
  { text: "What projects involved document processing?", icon: "heroicons:document-text" },
  { text: "What's Joe's experience with Nuxt?", icon: "simple-icons:nuxtdotjs", color: "#00DC82" },
  { text: "Tell me about Joe's CI/CD experience.", icon: "simple-icons:githubactions", color: "#2088FF" },
  { text: "What Kotlin projects has Joe built?", icon: "simple-icons:kotlin", color: "#7F52FF" },
  { text: "Has Joe worked with WordPress or CMS platforms?", icon: "simple-icons:wordpress", color: "#21759B" },
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
    aiStore.getCitedProjects(result.citations ?? [])
    searchResult.value = finalResult
  }
}

function usePrompt(prompt: string) {
  searchQuery.value = prompt
  performSearch()
}

function resetSearch() {
  searchQuery.value = ''
  searchResult.value = null
  aiStore.getCitedProjects([])
  hasSearched.value = false
  displayedPrompts.value = sampleSize(allPrompts, 3)
}
</script>
