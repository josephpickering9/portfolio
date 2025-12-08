<template>
  <div class="join">
    <button
      type="button"
      class="btn btn-sm join-item"
      :class="{ 'btn-active': modelValue === ViewMode.CARD }"
      :disabled="disabled"
      @click="$emit('update:modelValue', ViewMode.CARD)"
    >
      <Icon name="heroicons:squares-2x2" class="h-4 w-4" />
      <span class="sr-only">Card View</span>
    </button>
    <button
      type="button"
      class="btn btn-sm join-item hidden md:flex"
      :class="{ 'btn-active': modelValue === ViewMode.TABLE }"
      :disabled="disabled"
      @click="$emit('update:modelValue', ViewMode.TABLE)"
    >
      <Icon name="heroicons:list-bullet" class="h-4 w-4" />
      <span class="sr-only">Table View</span>
    </button>
  </div>
</template>

<script setup lang="ts">
import { ViewMode } from '~/types/ViewMode'

interface Props {
  modelValue: ViewMode
  disabled?: boolean
}

withDefaults(defineProps<Props>(), {
  disabled: false,
})

defineEmits<{
  'update:modelValue': [value: ViewMode]
}>()
</script>
