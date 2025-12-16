<template>
  <div 
    class="tooltip" 
    :class="[
      position ? `tooltip-${position}` : 'tooltip-top', 
      color ? `tooltip-${color}` : '', 
      open ? 'tooltip-open' : ''
    ]" 
    :data-tip="text"
  >
    <slot />
  </div>
</template>

<script setup lang="ts">
interface Props {
  text: string
  position?: 'top' | 'bottom' | 'left' | 'right'
  color?: 'primary' | 'secondary' | 'accent' | 'info' | 'success' | 'warning' | 'error'
  open?: boolean
}

defineProps<Props>()
</script>
