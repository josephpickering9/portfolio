<template>
  <div :class="{ error: error }" class="relative w-full">
    <slot />
    <div v-if="error" class="input-errors">
      <div class="error-msg">{{ error }}</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { ErrorObject } from '@vuelidate/core'

interface Props {
  errors?: ErrorObject[]
  name?: string | undefined
}

const props = withDefaults(defineProps<Props>(), {
  errors: () => [],
  name: undefined,
})

const error = computed((): string | undefined => {
  if (!props.errors.length) return undefined

  return props.errors[0]?.$message.toString().replace('Value', props.name ?? 'Value')
})
</script>

<style scoped>
.error-msg {
  position: absolute;
  bottom: -22px;
  left: 0.25rem;
  font-size: 0.875rem;
  line-height: 1.25rem;
  color: rgb(239 68 68);
}
</style>
