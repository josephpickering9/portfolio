<template>
  <div id="app">
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

const themeStore = useThemeStore()

useHead({
  title: 'Portfolio | Joseph Pickering',
  htmlAttrs: {
    'data-theme': computed(() => themeStore.theme),
  },
})
</script>
