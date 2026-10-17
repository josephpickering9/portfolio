<template>
  <div class="flex min-h-dvh flex-col bg-bg text-text">
    <a
      href="#main-content"
      class="sr-only focus:not-sr-only focus:absolute focus:top-2 focus:left-2 focus:z-50 focus:rounded-full focus:bg-accent focus:px-4 focus:py-2 focus:font-medium focus:text-accent-ink"
    >
      Skip to content
    </a>
    <HeaderBar />
    <main id="main-content" class="flex grow flex-col">
      <slot />
    </main>
    <FooterBar />
  </div>
</template>

<script setup lang="ts">
import HeaderBar from '~/components/layout/navigation/HeaderBar.vue'
import FooterBar from '~/components/layout/navigation/FooterBar.vue'
</script>
