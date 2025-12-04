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

<script setup lang="ts">
import { ref, watch, defineAsyncComponent } from 'vue'
import 'vue3-colorpicker/style.css'
import FormElementContainer from '~/app/components/forms/elements/FormElementContainer.vue'

const ColorPicker = defineAsyncComponent(() => {
  return import.meta.client ? import('vue3-colorpicker').then((m) => m.ColorPicker) : new Promise(() => {})
})

interface Props {
  label?: string | null
  modelValue?: string
}

const props = withDefaults(defineProps<Props>(), {
  label: undefined,
  modelValue: '',
})

// Emits
const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const value = ref(props.modelValue ?? '')

// Watch methods
watch(() => props.modelValue, (newValue) => {
  value.value = newValue ?? ''
})

watch(value, (newValue) => {
  emit('update:modelValue', newValue)
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
