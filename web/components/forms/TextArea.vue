<template>
  <FormElementContainer :label="label">
    <textarea
      v-model="value"
      :type="type"
      :placeholder="placeholder"
      :required="required"
      :disabled="disabled"
      class="textarea textarea-bordered w-full"
    />
  </FormElementContainer>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import FormElementContainer from './FormElementContainer.vue'

interface Data {
  value: string
}

export default defineComponent({
  name: 'TextArea',
  components: { FormElementContainer },
  props: {
    label: {
      type: String,
      default: null,
    },
    modelValue: {
      type: String,
      default: null,
    },
    placeholder: {
      type: String,
      default: null,
    },
    type: {
      type: String,
      default: 'text',
    },
    required: {
      type: Boolean,
      default: false,
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
    modelValue() {
      this.value = this.modelValue
    },
    value() {
      this.$emit('update:modelValue', this.value)
    },
  },
})
</script>
