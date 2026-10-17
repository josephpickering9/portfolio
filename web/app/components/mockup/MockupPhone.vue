<template>
  <div v-if="framed" class="w-45 flex-[0_0_180px] rounded-[30px] border border-line bg-surface p-1.5 shadow-glow">
    <NuxtImg format="webp" :src="src" :alt="alt" width="168" loading="lazy" class="block aspect-9/19.5 w-full rounded-3xl object-cover object-top" />
  </div>
  <NuxtImg
v-else
    format="webp"
    :src="src"
    :alt="alt"
    width="180"
    loading="lazy"
    class="block aspect-9/19.5 w-45 flex-[0_0_180px] rounded-3xl border border-line object-cover object-top"
  />
</template>

<script setup lang="ts">
interface Props {
  src: string
  alt?: string
  framed?: boolean
}

withDefaults(defineProps<Props>(), {
  alt: '',
  framed: false,
})
</script>
