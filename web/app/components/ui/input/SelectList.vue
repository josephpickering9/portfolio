<template>
  <FormElementContainer :label="label">
    <select v-model="value" class="select w-full" :class="selectClass" :disabled="disabled">
      <option v-if="placeholder" :value="null" disabled selected>{{ placeholder }}</option>
      <option v-for="(option, index) in options" :key="index" :value="option.value">
        {{ option.text }}
      </option>
    </select>
    <button
      v-if="value && clearable"
      type="button"
      tabindex="-1"
      class="absolute right-4 top-2 text-xs"
      @click="value = null"
    >
      X
    </button>
  </FormElementContainer>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import type { SelectListItem } from '~/types/SelectListItem'
import FormElementContainer from '~/components/ui/form/FormElementContainer.vue'

interface Props {
  label?: string | null
  modelValue?: string | number | null
  options?: SelectListItem[]
  required?: boolean
  disabled?: boolean
  size?: string | null
  placeholder?: string | null
  clearable?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  label: undefined,
  modelValue: undefined,
  options: () => [],
  required: false,
  disabled: false,
  size: null,
  placeholder: undefined,
  clearable: false,
})


const emit = defineEmits<{
  'update:modelValue': [value: string | number | null]
}>()

const value = ref<string | number | null>(props.modelValue)

const selectClass = computed(() => ({
  'select-sm': props.size === 'sm',
  'select-md': props.size === 'md',
  'select-lg': props.size === 'lg',
  'bg-none': value.value && props.clearable,
}))


watch(() => props.modelValue, (newValue) => {
  value.value = newValue
})

watch(value, (newValue) => {
  emit('update:modelValue', newValue)
})
</script>
