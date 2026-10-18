<template>
  <form
    class="relative flex items-center gap-3 rounded-[18px] border border-line bg-surface py-2 pr-2 pl-5.5 shadow-[0_24px_70px_-24px_var(--glow)] [view-transition-name:ai-search-field]"
    role="search"
    @submit.prevent="submit"
  >
    <input
      :value="modelValue"
      type="text"
      aria-label="Ask about my work"
      placeholder="What has Joe built with .NET?"
      class="min-w-0 flex-1 border-0 bg-transparent py-3.5 text-lg text-text outline-none"
      data-testid="ai-search-input"
      @input="emit('update:modelValue', ($event.target as HTMLInputElement).value)"
    >
    <button type="submit" class="rounded-xl bg-accent px-5.5 py-3.5 text-[15px] font-medium text-accent-ink" data-testid="ai-search-submit">
      Ask
    </button>
  </form>
</template>

<script setup lang="ts">
interface Props {
  modelValue: string
}

const props = defineProps<Props>()

const emit = defineEmits<{
  (e: 'update:modelValue' | 'submit', value: string): void
}>()

function submit() {
  const question = props.modelValue.trim()
  if (question) emit('submit', question)
}
</script>
