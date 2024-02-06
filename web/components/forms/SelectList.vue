<template>
  <FormElementContainer :label="label">
    <select v-model="value" class="select select-bordered w-full" :class="selectClass" :disabled="disabled">
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

<script lang="ts">
import { defineComponent, type PropType } from 'vue'
import type { SelectListItem } from '../../types/SelectListItem'
import FormElementContainer from './FormElementContainer.vue'

interface Data {
  value: string | number | null
}

export default defineComponent({
  name: 'SelectList',
  components: { FormElementContainer },
  props: {
    label: {
      type: String,
      default: null,
    },
    modelValue: {
      type: [String, Number],
      default: null,
    },
    options: {
      type: Array as PropType<SelectListItem[]>,
      default: () => [],
    },
    required: {
      type: Boolean,
      default: false,
    },
    disabled: {
      type: Boolean,
      default: false,
    },
    size: {
      type: String,
      default: null,
    },
    placeholder: {
      type: String,
      default: null,
    },
    clearable: {
      type: Boolean,
      default: false,
    },
  },
  emits: ['update:modelValue'],
  data(): Data {
    return {
      value: this.modelValue,
    }
  },
  computed: {
    selectClass(): object {
      return {
        'select-sm': this.size === 'sm',
        'select-md': this.size === 'md',
        'select-lg': this.size === 'lg',
        'bg-none': this.value && this.clearable,
      }
    },
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
