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
import { defineComponent, defineAsyncComponent } from 'vue'
import 'vue3-colorpicker/style.css'
import FormElementContainer from './FormElementContainer.vue'

interface Data {
  value: string
}

export default defineComponent({
  name: 'ColourPicker',
  components: {
    FormElementContainer,
    ColorPicker: defineAsyncComponent(() => {
      return import.meta.client ? import('vue3-colorpicker').then((m) => m.ColorPicker) : new Promise(() => {})
    }),
  },
  props: {
    label: {
      type: String,
      default: null,
    },
    modelValue: {
      type: String,
      default: '',
    },
  },
  emits: ['update:modelValue'],
  data(): Data {
    return {
      value: this.modelValue ?? '',
    }
  },
  watch: {
    modelValue() {
      this.value = this.modelValue ?? ''
    },
    value() {
      this.$emit('update:modelValue', this.value)
    },
  },
})
</script>

<style>
.vc-colorpicker {
  width: 18rem;
  overflow: hidden;
  border-radius: 0.375rem;
  border-width: 1px;
  border-color: rgb(75 85 99);
  background-color: var(--fallback-b1, oklch(var(--b1))) !important;
}
</style>
