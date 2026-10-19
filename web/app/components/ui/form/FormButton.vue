<template>
  <component
    :is="href ? NuxtLink : 'button'"
    :to="href || undefined"
    :type="href ? undefined : 'button'"
    :disabled="href ? undefined : disabled || loading"
    class="relative flex items-center justify-center gap-2 rounded-full px-4 py-2.5 text-sm font-medium whitespace-nowrap transition-colors duration-200 disabled:cursor-not-allowed disabled:opacity-60"
    :class="variantClass"
    @click="emit('click')"
  >
    <Icon v-if="loading" name="mdi:loading" class="animate-spin" />
    <Icon v-else-if="icon" :name="icon" />
    {{ label }}
  </component>
</template>

<script setup lang="ts">
import { computed, resolveComponent } from 'vue'

type ButtonVariant = 'primary' | 'outline' | 'danger' | 'dashed'

interface Props {
  label: string
  variant?: ButtonVariant
  icon?: string
  href?: string
  disabled?: boolean
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  variant: 'primary',
  icon: undefined,
  href: undefined,
  disabled: false,
  loading: false,
})

const emit = defineEmits<{ (e: 'click'): void }>()

const VARIANT_CLASSES: Record<ButtonVariant, string> = {
  primary: 'bg-accent text-accent-ink hover:text-accent-ink',
  outline: 'border border-line text-text',
  danger: 'border border-line text-danger hover:border-danger hover:text-danger',
  dashed: 'self-start border border-dashed border-line text-muted hover:text-text',
}

const NuxtLink = resolveComponent('NuxtLink')

const variantClass = computed(() => VARIANT_CLASSES[props.variant])
</script>
