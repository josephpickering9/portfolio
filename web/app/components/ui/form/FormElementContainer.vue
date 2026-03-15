<template>
  <label v-if="label" class="w-full flex flex-col gap-1">
    <span class="label">
      <span class="label-text">{{ label }}</span>
    </span>
    <slot />
  </label>
  <span v-else class="relative w-full">
    <slot />
  </span>
</template>

<script setup lang="ts">
interface Props {
  label?: string | null
}

withDefaults(defineProps<Props>(), {
  label: undefined,
})
</script>
