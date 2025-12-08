<template>
  <FormElementContainer :label="label">
    <div class="relative flex items-center">
      <input
        ref="inputRef"
        v-model="value"
        :type="type"
        :placeholder="placeholder"
        :required="required"
        :disabled="disabled"
        class="input w-full pr-8"
        :class="inputClass"
        @focus="$emit('focus')"
        @blur="$emit('blur')"
        @keyup.enter="$emit('keyup.enter')"
      >
      <button v-if="value" type="button" tabindex="-1" class="absolute right-4 text-sm" @click="clear">X</button>
    </div>
  </FormElementContainer>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import FormElementContainer from '~/components/ui/form/FormElementContainer.vue'

interface Props {
  modelValue?: string | undefined
  label?: string | undefined
  placeholder?: string | undefined
  type?: string
  required?: boolean
  disabled?: boolean
  size?: string | undefined
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: undefined,
  label: undefined,
  placeholder: undefined,
  type: 'text',
  required: false,
  disabled: false,
  size: undefined,
})


const emit = defineEmits<{
  'update:modelValue': [value: string]
  'focus': []
  'blur': []
  'keyup.enter': []
}>()

const value = ref(props.modelValue)
const inputRef = ref<HTMLInputElement | null>(null)

const inputClass = computed(() => ({
  'input-sm': props.size === 'sm',
  'input-lg': props.size === 'lg',
}))

function clear() {
  value.value = ''
}

function focus() {
  inputRef.value?.focus()
}

function blur() {
  inputRef.value?.blur()
}

watch(() => props.modelValue, (newValue) => {
  value.value = newValue
})

watch(value, (newValue) => {
  emit('update:modelValue', newValue ?? '')
})

defineExpose({
  focus,
  blur,
})
</script>
