<template>
  <div 
    class="mt-8 transition-all duration-500 ease-in-out"
    :class="[hasSearched ? 'opacity-0 h-0 overflow-hidden mt-0' : 'opacity-100']"
  >
    <p v-if="!loading" class="text-sm text-center text-base-content/40 font-medium uppercase tracking-wider mb-4">Try asking</p>
    <div class="flex flex-wrap justify-center gap-3">
      <button 
        v-for="(prompt, index) in prompts" 
        :key="prompt.text"
        class="btn btn-sm h-auto py-2 px-4 rounded-xl border border-base-content/5 bg-base-100/50 hover:bg-base-200 hover:border-primary/30 transition-all duration-300 text-left font-normal animate-slide-up group"
        :style="{ animationDelay: `${index * 100}ms` }"
        @click="$emit('select', prompt.text)"
      >
        <span class="p-1.5 rounded-lg bg-base-200 group-hover:bg-primary/10 transition-colors mr-2">
          <Icon :name="prompt.icon" class="w-4 h-4 text-base-content/60 group-hover:text-primary transition-colors" />
        </span>
        <span class="text-base-content/80 group-hover:text-base-content">{{ prompt.text }}</span>
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  prompts: Array<{ text: string, icon: string }>
  loading: boolean
  hasSearched: boolean
}>()

defineEmits<{
  (e: 'select', text: string): void
}>()
</script>

<style scoped>
@keyframes slideUp {
  from { opacity: 0; transform: translateY(20px); }
  to { opacity: 1; transform: translateY(0); }
}

.animate-slide-up {
  animation: slideUp 0.5s ease-out forwards;
}
</style>
