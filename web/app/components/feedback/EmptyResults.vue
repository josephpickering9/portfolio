<template>
  <div class="flex flex-col items-start gap-3 py-12" data-testid="empty-results">
    <span class="font-display text-[32px]">{{ message }}</span>
    <button
      type="button"
      class="rounded-full border border-line px-4 py-2.25 text-sm text-text"
      data-testid="clear-filters"
      @click="emit('clear')"
    >
      Clear filters
    </button>
  </div>
</template>

<script setup lang="ts">
interface Props {
  message: string
}

defineProps<Props>()

const emit = defineEmits<{ (e: 'clear'): void }>()
</script>
