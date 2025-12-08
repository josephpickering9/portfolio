<template>
  <Transition name="notifications" mode="out-in">
    <div v-show="notificationsTopLeft" key="notificationsTopLeft" class="toast toast-start toast-top">
      <Toast v-for="notification in notificationsTopLeft" :key="notification.id" :notification="notification" />
    </div>
  </Transition>
  <Transition name="notifications" mode="out-in">
    <div v-show="notificationsTopRight" key="notificationsTopRight" class="toast toast-end toast-top">
      <Toast v-for="notification in notificationsTopRight" :key="notification.id" :notification="notification" />
    </div>
  </Transition>
  <Transition name="notifications" mode="out-in">
    <div v-show="notificationsBottomLeft" key="notificationsBottomLeft" class="toast toast-start toast-bottom">
      <Toast v-for="notification in notificationsBottomLeft" :key="notification.id" :notification="notification" />
    </div>
  </Transition>
  <Transition name="notifications" mode="out-in">
    <div v-show="notificationsBottomRight" key="notificationsBottomRight" class="toast toast-end toast-bottom">
      <Toast v-for="notification in notificationsBottomRight" :key="notification.id" :notification="notification" />
    </div>
  </Transition>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useNotificationStore } from '~/store/NotificationStore'
import type { Notification } from '~/types/Notification'
import { NotificationPosition } from '~/types/NotificationPosition'
import Toast from '~/components/feedback/notification/Toast.vue'

const notifications = computed((): Notification[] => {
  return useNotificationStore().notifications
})

const notificationsTopLeft = computed((): Notification[] => {
  return getFilteredNotifications(NotificationPosition.TOP_LEFT)
})

const notificationsTopRight = computed((): Notification[] => {
  return getFilteredNotifications(NotificationPosition.TOP_RIGHT)
})

const notificationsBottomLeft = computed((): Notification[] => {
  return getFilteredNotifications(NotificationPosition.BOTTOM_LEFT)
})

const notificationsBottomRight = computed((): Notification[] => {
  return getFilteredNotifications(NotificationPosition.BOTTOM_RIGHT)
})

function getFilteredNotifications(position: NotificationPosition): Notification[] {
  return notifications.value.filter((n) => n.position === position)
}
</script>

<style scoped>
.toast.toast-top {
  top: 4rem;
}

.notifications-move,
.notifications-enter-active,
.notifications-leave-active {
  transition: all 0.5s ease;
}

.notifications-enter-form,
.notifications-leave-to {
  opacity: 0;
  transform: translateY(200px);
}

.notifications-leave-active {
  position: absolute;
}
</style>
