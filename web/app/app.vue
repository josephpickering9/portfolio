<template>
  <div id="app" :data-theme="theme">
    <NuxtPwaManifest />
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>

    <Notifications />
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useThemeStore } from '~/store/ThemeStore'
import Notifications from '~/components/feedback/notification/Notifications.vue'
import { useHead } from '#imports'

useHead({
  title: 'Portfolio | Joseph Pickering',
})

const themeStore = useThemeStore()
const theme = computed(() => themeStore.selectedTheme)
</script>

<style>
.page-enter-active,
.page-leave-active {
  transition: opacity 0.4s ease-out, filter 0.4s ease-out;
}
.page-enter-from,
.page-leave-to {
  opacity: 0;
  filter: blur(1rem);
}

@media (prefers-reduced-motion: reduce) {
  .page-enter-active,
  .page-leave-active {
    transition: opacity 0.01ms;
  }
  .page-enter-from,
  .page-leave-to {
    filter: none;
  }
}
</style>
