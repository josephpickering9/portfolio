<template>
  <div id="app" :data-theme="theme">
    <NuxtPwaManifest />
    <NuxtLayout>
      <NuxtPage />
    </NuxtLayout>

    <Notifications />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useThemeStore } from './store/ThemeStore'
import Notifications from './components/notification/Notifications.vue'
import { useHead } from '#imports'

export default defineComponent({
  name: 'App',
  components: {
    Notifications,
  },
  setup() {
    useHead({
      title: 'Joseph Pickering',
    })
  },
  computed: {
    theme(): string {
      return useThemeStore().selectedTheme
    },
  },
})
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
