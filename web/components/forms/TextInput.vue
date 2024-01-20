<template>
  <FormElementContainer :label="label">
    <div class="relative flex items-center">
      <input
        v-model="value"
        :type="type"
        :placeholder="placeholder"
        :required="required"
        :disabled="disabled"
        class="input input-bordered w-full pr-8"
        v-bind="$attrs"
      />
      <button v-if="value" type="button" class="absolute right-4 text-sm" @click="clear">X</button>
    </div>
  </FormElementContainer>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import FormElementContainer from './FormElementContainer.vue'

interface Data {
  value: string
}

export default defineComponent({
  name: 'TextInput',
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
  methods: {
    clear() {
      this.value = ''
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
