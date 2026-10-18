<template>
  <div class="overflow-hidden border border-line bg-surface" :class="sizeClasses.frame">
    <div class="flex items-center border-b border-line" :class="sizeClasses.bar">
      <span v-for="dot in 3" :key="dot" class="shrink-0 rounded-full bg-line" :class="sizeClasses.dot" />
      <span v-if="host" class="ml-2.5 truncate font-mono text-xs text-muted">{{ host }}</span>
    </div>
    <div class="bg-surface2">
      <slot />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

type MockupSize = 'sm' | 'md' | 'lg'

interface Props {
  size?: MockupSize
  host?: string
}

const props = withDefaults(defineProps<Props>(), {
  size: 'md',
  host: undefined,
})

const sizes: Record<MockupSize, { frame: string; bar: string; dot: string }> = {
  sm: { frame: 'rounded-xl shadow-[0_30px_70px_-36px_var(--glow)]', bar: 'gap-1.25 px-3 py-2.25', dot: 'size-2' },
  md: { frame: 'rounded-card shadow-glow', bar: 'gap-1.5 px-3.5 py-2.75', dot: 'size-[9px]' },
  lg: { frame: 'rounded-2xl shadow-glow-lg', bar: 'gap-1.5 px-4 py-3', dot: 'size-2.5' },
}

const sizeClasses = computed(() => sizes[props.size])
</script>
