<template>
  <div v-if="icon" :class="[fieldClass, 'flex items-center gap-2.5 py-0 focus-within:border-accent']">
    <Icon :name="icon" class="shrink-0 text-muted" />
    <input
      :id="id"
      :value="modelValue ?? ''"
      type="text"
      :placeholder="placeholder"
      :disabled="disabled"
      :aria-invalid="invalid"
      class="min-w-0 flex-1 border-0 bg-transparent py-3 text-text outline-none"
      :class="{ 'font-mono text-sm': mono }"
      @input="emit('update:modelValue', ($event.target as HTMLInputElement).value)"
    >
  </div>
  <input
    v-else
    :id="id"
    :value="modelValue ?? ''"
    type="text"
    :placeholder="placeholder"
    :disabled="disabled"
    :aria-invalid="invalid"
    :class="[fieldClass, { 'font-mono text-sm': mono }]"
    @input="emit('update:modelValue', ($event.target as HTMLInputElement).value)"
  >
</template>

<script setup lang="ts">
import { useFormField } from '~/composables/useFormField'

interface Props {
  modelValue?: string | null
  placeholder?: string
  disabled?: boolean
  icon?: string
  mono?: boolean
}

withDefaults(defineProps<Props>(), {
  modelValue: undefined,
  placeholder: undefined,
  disabled: false,
  icon: undefined,
  mono: false,
})

const emit = defineEmits<{ (e: 'update:modelValue', value: string): void }>()

const { id, invalid, fieldClass } = useFormField()
</script>
