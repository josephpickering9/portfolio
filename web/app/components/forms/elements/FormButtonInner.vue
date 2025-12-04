<template>
  <div class="flex items-center gap-1">
    <Icon v-if="icon" :name="icon" />
    <span>{{ label }}</span>
  </div>
</template>

<script setup lang="ts">
interface Props {
  label?: string | null
  icon?: string | null
}

withDefaults(defineProps<Props>(), {
  label: undefined,
  icon: undefined,
})
</script>
