<template>
  <div class="flex min-w-0 flex-col gap-2">
    <div v-if="label || $slots['aside']" class="flex items-baseline justify-between gap-3">
      <component :is="labelTag" :for="labelTag === 'label' ? context.id : undefined" class="eyebrow tracking-[0.05em] text-muted">
        {{ label }}<template v-if="required"> *</template>
      </component>
      <slot name="aside" />
    </div>
    <slot />
    <span v-if="error" class="text-[13px] text-danger" role="alert">{{ error }}</span>
    <span v-else-if="helper" class="text-[13px] text-muted">{{ helper }}</span>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { ErrorObject } from '@vuelidate/core'
import { provideFormField } from '~/composables/useFormField'

interface Props {
  label?: string
  required?: boolean
  helper?: string
  errors?: ErrorObject[]
  name?: string
  labelTag?: 'label' | 'span'
}

const props = withDefaults(defineProps<Props>(), {
  label: undefined,
  required: false,
  helper: undefined,
  errors: () => [],
  name: undefined,
  labelTag: 'label',
})

const error = computed((): string | undefined => {
  const message = props.errors[0]?.$message
  if (!message) return undefined
  return message.toString().replace('Value', props.name ?? props.label ?? 'Value')
})

const context = provideFormField(computed(() => Boolean(error.value)))
</script>
