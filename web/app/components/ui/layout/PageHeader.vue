<template>
  <div class="flex flex-col gap-4.5">
    <span class="eyebrow text-accent">{{ eyebrow }}</span>
    <h1 class="m-0 font-display text-[clamp(48px,7vw,88px)] leading-[0.95] font-normal tracking-[-0.02em]">{{ title }}</h1>
    <p v-if="lede" class="m-0 max-w-160 text-[19px] leading-[1.55] text-muted">{{ lede }}</p>
  </div>
</template>

<script setup lang="ts">
interface Props {
  eyebrow: string
  title: string
  lede?: string
}

defineProps<Props>()
</script>
