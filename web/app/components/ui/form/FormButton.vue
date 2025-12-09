<template>
  <button v-if="!href" class="btn relative" :class="buttonClass" :disabled="disabled || loading" @click="$emit('click')">
    <span v-if="loading" class="loading loading-spinner absolute" />
    <FormButtonInner :label="label" :icon="icon" :class="{ 'opacity-0': loading }" />
  </button>
  <NuxtLink v-else :to="href" class="btn relative" :class="buttonClass">
    <span v-if="loading" class="loading loading-spinner absolute" />
    <FormButtonInner :label="label" :icon="icon" :class="{ 'opacity-0': loading }" />
  </NuxtLink>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import FormButtonInner from '~/components/ui/form/FormButtonInner.vue'

interface Props {
  label?: string | null
  type?: string
  disabled?: boolean
  size?: string | null
  href?: string | null
  icon?: string | null
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  label: undefined,
  type: 'submit',
  disabled: false,
  size: null,
  href: null,
  icon: undefined,
  loading: false,
})

const emit = defineEmits<{
  click: []
}>()

const buttonClass = computed(() => ({
  'btn-xs': props.size === 'xs',
  'btn-sm': props.size === 'sm',
  'btn-lg': props.size === 'lg',
  'btn-primary': props.type === 'primary',
  'btn-secondary': props.type === 'secondary',
  'btn-accent': props.type === 'accent',
  'btn-error': props.type === 'error',
  'btn-warning': props.type === 'warning',
  'btn-success': props.type === 'success',
  'btn-neutral': props.type === 'neutral',
  'btn-outline': props.type === 'outline',
  'btn-link': props.type === 'link',
  'btn-ghost': props.type === 'ghost',
}))
</script>
