<template>
  <SelectList v-model="value" :options="options" :label="label" />
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import type { SelectListItem } from '~/types/SelectListItem'
import SelectList from '~/components/ui/input/SelectList.vue'

interface Props {
  label?: string
  modelValue?: number | null
  totalYears?: number
}

const props = withDefaults(defineProps<Props>(), {
  label: 'Year',
  modelValue: undefined,
  totalYears: 15,
})


const emit = defineEmits<{
  'update:modelValue': [value: string | number]
}>()

const value = ref(props.modelValue)

const options = computed((): SelectListItem[] => {
  return Array.from({ length: props.totalYears }, (_, i) => ({
    text: (new Date().getFullYear() - i).toString(),
    value: new Date().getFullYear() - i,
  }))
})


watch(() => props.modelValue, (newValue) => {
  value.value = newValue
})

watch(value, (newValue) => {
  emit('update:modelValue', newValue ?? '')
})
</script>
