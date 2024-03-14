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

<script lang="ts">
import { defineComponent } from 'vue'
import { useNotificationStore } from '../../store/NotificationStore'
import type { Notification } from '../../types/Notification'
import { NotificationPosition } from '../../types/NotificationPosition'
import Toast from './Toast.vue'

export default defineComponent({
  name: 'Notifications',
  components: { Toast },
  computed: {
    notifications(): Notification[] {
      return useNotificationStore().notifications
    },
    notificationsTopLeft(): Notification[] {
      return this.getFilteredNotifications(NotificationPosition.TOP_LEFT)
    },
    notificationsTopRight(): Notification[] {
      return this.getFilteredNotifications(NotificationPosition.TOP_RIGHT)
    },
    notificationsBottomLeft(): Notification[] {
      return this.getFilteredNotifications(NotificationPosition.BOTTOM_LEFT)
    },
    notificationsBottomRight(): Notification[] {
      return this.getFilteredNotifications(NotificationPosition.BOTTOM_RIGHT)
    },
  },
  methods: {
    getFilteredNotifications(position: NotificationPosition): Notification[] {
      return this.notifications.filter((n) => n.position === position)
    },
  },
})
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
