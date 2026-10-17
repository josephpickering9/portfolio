<template>
  <section class="flex gap-x-10" :class="stacked ? 'flex-col gap-y-6' : 'flex-wrap gap-y-4'">
    <h2 class="m-0 flex-[0_0_180px] font-mono text-[13px] font-normal text-accent" :class="{ 'flex-none': stacked }">{{ label }}</h2>
    <div class="min-w-0 flex-[1_1_460px]">
      <slot />
    </div>
  </section>
</template>

<script setup lang="ts">
interface Props {
  label: string
  stacked?: boolean
}

withDefaults(defineProps<Props>(), {
  stacked: false,
})
</script>
