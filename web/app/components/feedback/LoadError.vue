<template>
  <div class="flex flex-wrap items-center justify-between gap-4 rounded-card border border-line bg-surface px-6 py-5.5" role="alert" data-testid="load-error">
    <div class="flex items-center gap-3">
      <Icon name="heroicons:exclamation-triangle" size="22" class="shrink-0 text-accent" />
      <span>{{ message }}</span>
    </div>
    <button
      type="button"
      class="rounded-full bg-accent px-4 py-2.25 text-sm font-medium text-accent-ink"
      data-testid="load-error-retry"
      @click="emit('retry')"
    >
      Retry
    </button>
  </div>
</template>

<script setup lang="ts">
interface Props {
  message?: string
}

withDefaults(defineProps<Props>(), {
  message: "Couldn't load projects. Check your connection and try again.",
})

const emit = defineEmits<{ (e: 'retry'): void }>()
</script>
