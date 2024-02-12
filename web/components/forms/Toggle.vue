<template>
  <FormElementContainer :label="label">
    <input v-model="value" type="checkbox" class="toggle" :disabled="disabled" />
  </FormElementContainer>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import FormElementContainer from './FormElementContainer.vue'

interface Data {
  value: boolean
}

export default defineComponent({
  name: 'Toggle',
  components: { FormElementContainer },
  props: {
    modelValue: {
      type: Boolean,
      default: false,
    },
    label: {
      type: String,
      default: undefined,
    },
    disabled: {
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
  watch: {
    modelValue(newValue: boolean): void {
      this.value = newValue
    },
    value(newValue: boolean): void {
      this.$emit('update:modelValue', newValue)
    },
  },
})
</script>
