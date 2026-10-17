<template>
  <textarea
    :id="id"
    :value="modelValue ?? ''"
    :placeholder="placeholder"
    :disabled="disabled"
    :rows="rows"
    :aria-invalid="invalid"
    :class="[fieldClass, 'resize-y leading-[1.6]']"
    @input="emit('update:modelValue', ($event.target as HTMLTextAreaElement).value)"
  />
</template>

<script setup lang="ts">
import { useFormField } from '~/composables/useFormField'

interface Props {
  modelValue?: string | null
  placeholder?: string
  disabled?: boolean
  rows?: number
}

withDefaults(defineProps<Props>(), {
  modelValue: undefined,
  placeholder: undefined,
  disabled: false,
  rows: 4,
})

const emit = defineEmits<{ (e: 'update:modelValue', value: string): void }>()

const { id, invalid, fieldClass } = useFormField()
</script>
