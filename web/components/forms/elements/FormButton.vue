<template>
  <button v-if="!href" class="btn" :class="buttonClass" @click="$emit('click')">
    <FormButtonInner :label="label" :icon="icon" />
  </button>
  <NuxtLink v-else :to="href" class="btn" :class="buttonClass">
    <FormButtonInner :label="label" :icon="icon" />
  </NuxtLink>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import FormButtonInner from './FormButtonInner.vue'

export default defineComponent({
  name: 'FormButton',
  components: { FormButtonInner },
  props: {
    label: {
      type: String,
      default: null,
    },
    type: {
      type: String,
      default: 'submit',
    },
    disabled: {
      type: Boolean,
      default: false,
    },
    size: {
      type: String,
      default: null,
    },
    href: {
      type: String,
      default: null,
    },
    icon: {
      type: String,
      default: null,
    },
  },
  emits: ['click'],
  computed: {
    buttonClass(): object {
      return {
        'btn-xs': this.size === 'xs',
        'btn-sm': this.size === 'sm',
        'btn-lg': this.size === 'lg',
        'btn-primary': this.type === 'primary',
        'btn-secondary': this.type === 'secondary',
        'btn-accent': this.type === 'accent',
        'btn-error': this.type === 'error',
        'btn-warning': this.type === 'warning',
        'btn-success': this.type === 'success',
        'btn-neutral': this.type === 'neutral',
        'btn-outline': this.type === 'outline',
        'btn-link': this.type === 'link',
        'btn-ghost': this.type === 'ghost',
      }
    },
  },
})
</script>
