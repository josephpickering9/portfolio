<template>
  <label class="flex items-center gap-2.5 rounded-xl border border-line bg-surface px-4 focus-within:border-accent">
    <Icon name="heroicons:magnifying-glass" class="shrink-0 text-muted" />
    <span class="sr-only">{{ placeholder }}</span>
    <input
      :value="modelValue"
      type="search"
      :placeholder="placeholder"
      data-testid="search-field"
      class="min-w-0 flex-1 border-0 bg-transparent py-3.25 text-[15px] text-text outline-none"
      @input="emit('update:modelValue', ($event.target as HTMLInputElement).value)"
      @keydown.enter="emit('enter')"
    >
  </label>
</template>

<script setup lang="ts">
interface Props {
  modelValue: string
  placeholder: string
}

defineProps<Props>()

const emit = defineEmits<{
  (e: 'update:modelValue', value: string): void
  (e: 'enter'): void
}>()
</script>
