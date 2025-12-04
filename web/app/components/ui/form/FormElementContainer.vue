<template>
  <label v-if="label" class="w-full">
    <div class="label">
      <span class="label-text">{{ label }}</span>
    </div>
    <slot />
  </label>
  <div v-else class="relative w-full">
    <slot />
  </div>
</template>

<script setup lang="ts">
interface Props {
  label?: string | null
}

withDefaults(defineProps<Props>(), {
  label: undefined,
})
</script>
