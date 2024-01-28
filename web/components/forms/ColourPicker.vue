<template>
  <FormElementContainer :label="label">
    <ClientOnly>
      <div class="mx-2 flex items-center gap-2">
        <ColorPicker v-model:pure-color="value" format="hex" shape="circle" picker-type="fk" />
        <div class="flex flex-col items-start justify-center text-sm">
          <span v-if="value">{{ value.toUpperCase() }}</span>
          <span v-else class="italic">No Colour Selected</span>
        </div>
      </div>
    </ClientOnly>
  </FormElementContainer>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { ColorPicker } from 'vue3-colorpicker'
import 'vue3-colorpicker/style.css'
import FormElementContainer from './FormElementContainer.vue'

interface Data {
  value: string
}

export default defineComponent({
  name: 'ColourPicker',
  components: { FormElementContainer, ColorPicker },
  props: {
    label: {
      type: String,
      default: null,
    },
    modelValue: {
      type: String,
      default: null,
    },
  },
  emits: ['update:modelValue'],
  data(): Data {
    return {
      value: '',
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

<style>
.vc-colorpicker {
  @apply w-72 overflow-hidden rounded-md border border-gray-700 !bg-base-100;
}
</style>
