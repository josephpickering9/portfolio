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

<script setup lang="ts">
import { ref, watch } from 'vue'
import FormElementContainer from '~/components/ui/form/FormElementContainer.vue'

interface Props {
  label?: string | null
  modelValue?: string | null
  placeholder?: string | null
  type?: string
  required?: boolean
  disabled?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  label: undefined,
  modelValue: undefined,
  placeholder: undefined,
  type: 'text',
  required: false,
  disabled: false,
})


const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

const value = ref(props.modelValue)


watch(() => props.modelValue, (newValue) => {
  value.value = newValue
})

watch(value, (newValue) => {
  emit('update:modelValue', newValue ?? '')
})
</script>
