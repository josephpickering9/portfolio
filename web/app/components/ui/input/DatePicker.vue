<template>
  <input
    :id="id"
    type="date"
    :value="modelValue ?? ''"
    :disabled="disabled"
    :aria-invalid="invalid"
    :class="fieldClass"
    @input="emit('update:modelValue', ($event.target as HTMLInputElement).value || undefined)"
  >
</template>

<script setup lang="ts">
import { useFormField } from '~/composables/useFormField'

interface Props {
  modelValue?: string | null
  disabled?: boolean
}

withDefaults(defineProps<Props>(), {
  modelValue: undefined,
  disabled: false,
})

const emit = defineEmits<{ (e: 'update:modelValue', value: string | undefined): void }>()

const { id, invalid, fieldClass } = useFormField()
</script>
