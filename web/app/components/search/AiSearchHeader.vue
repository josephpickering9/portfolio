<template>
  <div class="relative flex flex-col items-center gap-4 text-center">
    <span
      v-if="!compact"
      class="flex size-16 items-center justify-center rounded-full border border-line bg-surface text-[30px] text-accent shadow-[0_0_40px_var(--glow)]"
      aria-hidden="true"
    >
      <Icon name="mdi:robot-happy-outline" />
    </span>
    <h1
      class="m-0 font-display font-normal [view-transition-name:ai-search-title]"
      :class="compact ? 'text-[44px] leading-none tracking-[-0.01em]' : 'text-[clamp(48px,7vw,88px)] leading-[0.95] tracking-[-0.02em]'"
    >
      Ask about my work
    </h1>
    <p v-if="!compact" class="m-0 max-w-160 text-[19px] leading-[1.55] text-muted">
      Ask questions about my work experience, projects and skills
    </p>
  </div>
</template>

<script setup lang="ts">
interface Props {
  compact?: boolean
}

withDefaults(defineProps<Props>(), {
  compact: false,
})
</script>
