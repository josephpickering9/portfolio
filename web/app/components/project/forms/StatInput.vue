<template>
  <div class="flex flex-col gap-2.5">
    <div v-for="(stat, index) in modelValue" :key="index" class="flex flex-wrap items-center gap-2.5">
      <input
        :value="stat.value"
        placeholder="240+"
        :aria-label="`Stat ${index + 1} value`"
        class="min-w-0 flex-[0_1_140px] rounded-field border bg-bg px-3.5 py-2.75 text-[15px] text-text outline-none focus:border-accent"
        :class="hasError(index, 'value') ? 'border-danger' : 'border-line'"
        @input="update(index, 'value', ($event.target as HTMLInputElement).value)"
      >
      <input
        :value="stat.label"
        placeholder="gigs logged"
        :aria-label="`Stat ${index + 1} label`"
        class="min-w-0 flex-[1_1_220px] rounded-field border bg-bg px-3.5 py-2.75 text-[15px] text-text outline-none focus:border-accent"
        :class="hasError(index, 'label') ? 'border-danger' : 'border-line'"
        @input="update(index, 'label', ($event.target as HTMLInputElement).value)"
      >
      <RowRemoveButton label="Remove stat" @click="remove(index)" />
    </div>
    <FormButton label="Add stat" variant="dashed" icon="mdi:plus" @click="add" />
  </div>
</template>

<script setup lang="ts">
import type { CreateProjectStat } from '@api'
import { removeAt, replaceAt } from '~/utils/array-helper'
import FormButton from '~/components/ui/form/FormButton.vue'
import RowRemoveButton from '~/components/ui/button/RowRemoveButton.vue'

type StatField = 'value' | 'label'

interface Props {
  modelValue: CreateProjectStat[]
  invalidFields?: Record<number, StatField[]>
}

const props = withDefaults(defineProps<Props>(), {
  invalidFields: () => ({}),
})

const emit = defineEmits<{ (e: 'update:modelValue', value: CreateProjectStat[]): void }>()

function hasError(index: number, field: StatField): boolean {
  return props.invalidFields[index]?.includes(field) ?? false
}

function update(index: number, field: StatField, value: string) {
  const current = props.modelValue[index]
  if (current) emit('update:modelValue', replaceAt(props.modelValue, index, { ...current, [field]: value }))
}

function add() {
  emit('update:modelValue', [...props.modelValue, { value: '', label: '' }])
}

function remove(index: number) {
  emit('update:modelValue', removeAt(props.modelValue, index))
}
</script>
