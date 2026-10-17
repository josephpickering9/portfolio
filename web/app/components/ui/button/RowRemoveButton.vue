<template>
  <button
    type="button"
    class="flex size-11 shrink-0 items-center justify-center rounded-field border border-line text-lg text-muted hover:border-danger hover:text-danger"
    :aria-label="label"
    @click="emit('click')"
  >
    <Icon name="heroicons:trash" />
  </button>
</template>

<script setup lang="ts">
defineProps<{
  label: string
}>()

const emit = defineEmits<{ (e: 'click'): void }>()
</script>
