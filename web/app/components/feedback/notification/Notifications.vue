<template>
  <TransitionGroup
    tag="div"
    class="pointer-events-none fixed bottom-7 left-1/2 z-60 flex -translate-x-1/2 flex-col items-center gap-2"
    enter-active-class="transition duration-200"
    enter-from-class="translate-y-2 opacity-0"
    leave-active-class="transition duration-200"
    leave-to-class="opacity-0"
    aria-live="polite"
  >
    <Toast v-for="notification in notificationStore.notifications" :key="notification.id" :notification="notification" />
  </TransitionGroup>
</template>

<script setup lang="ts">
import { useNotificationStore } from '~/store/NotificationStore'
import Toast from '~/components/feedback/notification/Toast.vue'

const notificationStore = useNotificationStore()
</script>
