<template>
  <SelectList v-model="value" :options="options" :label="label" />
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import SelectList from '../elements/SelectList.vue'
import type { SelectListItem } from '../../../types/SelectListItem'

interface Data {
  value: string
  options: SelectListItem[]
}

export default defineComponent({
  name: 'YearSelectList',
  components: { SelectList },
  props: {
    label: {
      type: String,
      default: 'Year',
    },
    modelValue: {
      type: Number,
      default: null,
    },
    totalYears: {
      type: Number,
      default: 15,
    },
  },
  emits: ['update:modelValue'],
  data(): Data {
    return {
      value: this.modelValue,
      options: Array.from({ length: this.totalYears }, (_, i) => ({
        text: (new Date().getFullYear() - i).toString(),
        value: new Date().getFullYear() - i,
      })),
    }
  },
  watch: {
    modelValue() {
      this.value = this.modelValue
    },
    value() {
      this.$emit('update:modelValue', this.value)
    },
  },
})
</script>
