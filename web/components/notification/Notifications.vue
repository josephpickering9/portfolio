<template>
  <Transition name="notifications" mode="out-in">
    <div v-show="notifications" key="notifications" class="toast toast-end toast-top">
      <Toast v-for="notification in notifications" :key="notification.id" :notification="notification" />
    </div>
  </Transition>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { useNotificationStore } from '../../store/NotificationStore'
import type { Notification } from '../../types/Notification'
import Toast from './Toast.vue'

export default defineComponent({
  name: 'Notifications',
  components: {
    Toast,
  },
  computed: {
    notifications(): Notification[] {
      return useNotificationStore().notifications
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
