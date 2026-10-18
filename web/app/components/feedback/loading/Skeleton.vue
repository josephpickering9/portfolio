<template>
  <div v-if="type === LoadingType.CARD" class="flex flex-col gap-4" aria-hidden="true" data-testid="skeleton">
    <div class="skeleton-pulse aspect-16/10 rounded-card border border-line bg-surface" />
    <div class="skeleton-pulse h-5.5 w-[45%] rounded-md bg-surface2" />
    <div class="skeleton-pulse h-3.5 w-[85%] rounded-md bg-surface2" />
  </div>
  <div v-else class="flex flex-col gap-3.5" aria-hidden="true" data-testid="skeleton">
    <div class="flex items-center gap-4">
      <div class="skeleton-pulse size-12 shrink-0 rounded-full bg-surface2" />
      <div class="skeleton-pulse h-7 w-55 max-w-full rounded-md bg-surface2" />
    </div>
    <div class="skeleton-pulse h-3.5 w-4/5 max-w-150 rounded-md bg-surface2" />
    <div class="skeleton-pulse h-3.5 w-3/5 max-w-115 rounded-md bg-surface2" />
  </div>
</template>

<script setup lang="ts">
import { LoadingType } from '~/types/LoadingType'

interface Props {
  type?: LoadingType
}

withDefaults(defineProps<Props>(), {
  type: LoadingType.PROFILE,
})
</script>
