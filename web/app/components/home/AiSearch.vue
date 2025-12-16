<template>
  <div class="w-full max-w-3xl flex flex-col gap-8">
    <div class="flex items-center gap-3 relative group">
      <div class="w-full relative">
        <TextInput
            v-model="searchQuery"
            placeholder="Ask anything about my experience..."
            class="w-full premium-input-wrapper"
            @keyup.enter="performSearch"
        />
      </div>
      <button 
          class="btn btn-lg rounded-full bg-gradient-to-r from-primary to-secondary border-none text-white hover:scale-105 transition-transform shadow-lg hover:shadow-primary/50 px-8" 
          :disabled="aiStore.queryForm.loading"
          @click="performSearch"
      >
          <span v-if="aiStore.queryForm.loading" class="loading loading-spinner text-white"/>
          <Icon v-else name="heroicons:sparkles" class="w-6 h-6 mr-1 animate-pulse" />
          <span class="font-bold">{{ aiStore.queryForm.loading ? 'Thinking...' : 'Ask AI' }}</span>
      </button>
    </div>

    <div v-if="!searchResult" class="flex flex-col gap-4">
      <p v-if="!aiStore.queryForm.loading" class="text-sm text-center text-base-content/60 font-medium uppercase tracking-wider">Try asking</p>
      <div v-if="!aiStore.queryForm.loading" class="flex flex-wrap justify-center gap-3">
        <button 
          v-for="(prompt, index) in displayedPrompts" 
          :key="prompt.text"
          class="btn btn-sm md:btn-md btn-outline rounded-full border-base-200 hover:bg-base-content/5 hover:border-primary/50 transition-all duration-300 font-normal normal-case animate-slide-in"
          :style="{ animationDelay: `${index * 100}ms` }"
          @click="usePrompt(prompt.text)"
        >
          <Icon :name="prompt.icon" class="w-5 h-5 mr-2 opacity-70" />
          {{ prompt.text }}
        </button>
      </div>
    </div>

    <div v-if="searchResult" class="card bg-base-100/50 backdrop-blur-md border border-base-content/10 shadow-2xl overflow-hidden animate-fade-in-up">
      <div class="card-body p-6 md:p-8">
        <div class="flex items-start gap-4">
          <div class="p-3 bg-gradient-to-br from-primary/20 to-secondary/20 rounded-2xl">
            <Icon name="heroicons:sparkles" class="w-8 h-8 text-primary" />
          </div>
          <div class="flex-1 space-y-4">
            <h3 class="font-bold text-lg text-primary">AI Answer</h3>
            <div class="prose prose-sm md:prose-base max-w-none">
              <p class="whitespace-pre-wrap leading-relaxed text-base-content/90">{{ searchResult }}</p>
            </div>
            <div class="flex justify-end pt-2">
                <button class="btn btn-ghost btn-xs text-base-content/50 hover:text-base-content" @click="searchResult = null">Clear Result</button>
            </div>
          </div>
        </div>
      </div>
    </div>
     <div v-if="aiStore.queryForm.error" class="alert alert-error shadow-lg rounded-2xl">
      <Icon name="heroicons:exclamation-circle" class="w-6 h-6" />
      <span>{{ aiStore.queryForm.error }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { sampleSize } from 'lodash-es'
import { useAiStore } from '~/store/AiStore'
import TextInput from '~/components/ui/input/TextInput.vue'

const aiStore = useAiStore()
const searchQuery = ref('')
const searchResult = ref<string | null>(null)
const displayedPrompts = ref<Array<{ text: string, icon: string }>>([])

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
  searchResult.value = null
  
  await aiStore.query(searchQuery.value)
  const result = aiStore.queryForm.data
  
  if (result) {
    if (typeof result === 'string') {
        searchResult.value = result
    } else if (typeof result === 'object') {
        const res = result as Record<string, any>
        if (res['answer']) searchResult.value = res['answer']
        else if (res['text']) searchResult.value = res['text']
        else if (res['response']) searchResult.value = res['response']
        else searchResult.value = JSON.stringify(result, null, 2)
    } else {
        searchResult.value = String(result)
    }
  }
}

function usePrompt(prompt: string) {
  searchQuery.value = prompt
  performSearch()
}
</script>

<style scoped>
:deep(.premium-input-wrapper .input) {
  border-radius: 9999px;
  height: 3rem; /* 48px */
  padding-left: 1.5rem;
  padding-right: 3rem;
  background-color: hsl(var(--b1));
  box-shadow: inset 0 2px 4px 0 rgb(0 0 0 / 0.05);
  transition: all 0.3s ease;
  border: 1px solid hsl(var(--bc) / 0.1);
}

@media (min-width: 768px) {
  :deep(.premium-input-wrapper .input) {
     height: 3.5rem; /* 56px */
     font-size: 1.125rem;
  }
}

:deep(.premium-input-wrapper .input:focus) {
  border-color: hsl(var(--p));
  box-shadow: 0 0 0 4px hsl(var(--p) / 0.1);
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-slide-in {
  opacity: 0; /* Starts hidden */
  animation: slideIn 0.5s ease-out forwards;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade-in-up {
  animation: fadeInUp 0.5s ease-out forwards;
}
</style>
