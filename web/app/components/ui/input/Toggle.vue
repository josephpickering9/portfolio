<template>
  <FormElementContainer :label="label">
    <input v-model="value" type="checkbox" class="toggle" :disabled="disabled" >
  </FormElementContainer>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import FormElementContainer from '~/components/ui/form/FormElementContainer.vue'

interface Props {
  modelValue?: boolean
  label?: string | undefined
  disabled?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: false,
  label: undefined,
  disabled: false,
})


const emit = defineEmits<{
  'update:modelValue': [value: boolean]
}>()

const value = ref(props.modelValue)


watch(() => props.modelValue, (newValue: boolean) => {
  value.value = newValue
})

watch(value, (newValue: boolean) => {
  emit('update:modelValue', newValue)
})
</script>
