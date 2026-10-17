<template>
  <div class="relative">
    <select
      :id="id"
      :value="modelValue ?? ''"
      :disabled="disabled"
      :aria-invalid="invalid"
      :class="[fieldClass, 'appearance-none pr-10']"
      @change="emit('update:modelValue', ($event.target as HTMLSelectElement).value || undefined)"
    >
      <option v-if="placeholder" value="">{{ placeholder }}</option>
      <option v-for="option in options" :key="option.value" :value="option.value">{{ option.label }}</option>
    </select>
    <Icon
      name="heroicons:chevron-down"
      class="pointer-events-none absolute top-1/2 right-3.5 -translate-y-1/2 text-muted"
      aria-hidden="true"
    />
  </div>
</template>

<script setup lang="ts">
import { useFormField } from '~/composables/useFormField'

export interface SelectOption {
  value: string
  label: string
}

interface Props {
  modelValue?: string | null
  options: SelectOption[]
  placeholder?: string
  disabled?: boolean
}

withDefaults(defineProps<Props>(), {
  modelValue: undefined,
  placeholder: undefined,
  disabled: false,
})

const emit = defineEmits<{ (e: 'update:modelValue', value: string | undefined): void }>()

const { id, invalid, fieldClass } = useFormField()
</script>
