<template>
  <div class="w-full relative z-20">
    <div 
      class="relative group transition-transform duration-300"
      :class="{'scale-105': isInputFocused && !hasSearched}"
    >
      <div class="absolute inset-0 bg-gradient-to-r from-primary/20 to-secondary/20 rounded-full blur-xl opacity-0 group-hover:opacity-100 transition-opacity duration-500"/>
      
      <div class="relative bg-base-100/80 backdrop-blur-xl border border-base-content/10 shadow-lg rounded-full overflow-hidden flex items-center p-2 transition-all duration-300 ring-4 ring-transparent focus-within:ring-primary/10 focus-within:border-primary/50 focus-within:shadow-xl">
        <div class="pl-4 pr-2 text-base-content/40">
          <Icon name="heroicons:magnifying-glass" class="w-6 h-6" />
        </div>
        <input
          :value="modelValue"
          type="text"
          placeholder="Ask anything..."
          class="flex-1 bg-transparent border-none outline-none text-lg px-2 h-12 md:h-14 placeholder:text-base-content/30"
          @input="$emit('update:modelValue', ($event.target as HTMLInputElement).value)"
          @focus="isInputFocused = true"
          @blur="isInputFocused = false"
          @keyup.enter="$emit('search')"
        >
        <button 
          class="btn btn-circle btn-primary btn-md ml-2 border-none shadow-lg hover:shadow-primary/50 hover:scale-105 transition-all duration-300"
          :disabled="loading || !modelValue"
          @click="$emit('search')"
        >
          <span v-if="loading" class="loading loading-spinner loading-sm text-white"/>
          <Icon v-else name="heroicons:arrow-right" class="w-5 h-5 text-white" />
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

defineProps<{
  modelValue: string
  loading: boolean
  hasSearched: boolean
}>()

defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'search'): void
}>()

const isInputFocused = ref(false)
</script>
