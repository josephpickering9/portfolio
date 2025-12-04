<template>
  <div :class="{ error: error }" class="relative w-full">
    <slot />
    <div v-if="error" class="input-errors">
      <div class="error-msg">{{ error }}</div>
    </div>
  </div>
</template>

<script lang="ts">
import type { ErrorObject } from '@vuelidate/core'
import type { PropType } from 'vue'
import { defineComponent } from 'vue'

export default defineComponent({
  name: 'FormGroup',
  props: {
    errors: {
      type: Array as PropType<ErrorObject[]>,
      default: () => [],
    },
    name: {
      type: String,
      default: undefined,
    },
  },
  computed: {
    error(): string | undefined {
      if (!this.errors.length) return undefined

      return this.errors[0]?.$message.toString().replace('Value', this.name ?? 'Value')
    },
  },
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
