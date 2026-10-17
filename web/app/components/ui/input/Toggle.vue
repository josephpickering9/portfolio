<template>
  <label class="flex cursor-pointer items-center justify-between gap-4 rounded-xl border border-line bg-bg px-4.5 py-4">
    <span class="flex flex-col gap-0.5">
      <span class="font-medium">{{ label }}</span>
      <span v-if="description" class="text-[13px] text-muted">{{ description }}</span>
    </span>
    <button
      type="button"
      role="switch"
      :aria-checked="modelValue"
      :disabled="disabled"
      class="relative h-7 w-12 shrink-0 rounded-full transition-colors duration-200"
      :class="modelValue ? 'bg-accent' : 'bg-surface2'"
      @click="emit('update:modelValue', !modelValue)"
    >
      <span
        class="absolute top-0.75 size-5.5 rounded-full transition-[left] duration-200"
        :class="modelValue ? 'left-5.75 bg-accent-ink' : 'left-0.75 bg-muted'"
      />
    </button>
  </label>
</template>

<script setup lang="ts">
interface Props {
  modelValue: boolean
  label: string
  description?: string
  disabled?: boolean
}

withDefaults(defineProps<Props>(), {
  description: undefined,
  disabled: false,
})

const emit = defineEmits<{ (e: 'update:modelValue', value: boolean): void }>()
</script>
