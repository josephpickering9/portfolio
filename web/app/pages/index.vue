<template>
  <div class="flex mx-auto flex-col items-center justify-center gap-12 px-4 pt-8 pb-16 min-h-[85vh] bg-gradient-to-b from-transparent to-base-200/50">
    <HeroSection />
    <NavigationCards />
  </div>
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import NavigationCards from '~/components/home/NavigationCards.vue'
import HeroSection from '~/components/home/HeroSection.vue'

const { updateMeta } = useMeta()
updateMeta({ title: 'Home', description: "Welcome to my work experience portfolio." })
</script>
