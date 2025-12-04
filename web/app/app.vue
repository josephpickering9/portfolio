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
import { useThemeStore } from './store/ThemeStore'
import Notifications from './components/notification/Notifications.vue'
import { useHead } from '#imports'

useHead({
  title: 'Joseph Pickering',
})

const themeStore = useThemeStore()
const theme = computed(() => themeStore.selectedTheme)
</script>

<style>
.page-enter-active,
.page-leave-active {
  transition: all 0.4s;
}
.page-enter-from,
.page-leave-to {
  opacity: 0;
  filter: blur(1rem);
}
</style>
