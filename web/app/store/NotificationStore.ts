import { defineStore } from 'pinia'
import type { Notification } from '~/types/Notification'
import { NotificationType } from '~/types/NotificationType'

const TOAST_SECONDS = 3.2

export const useNotificationStore = defineStore('notificationStore', {
  state: () => ({
    notifications: [] as Notification[],
  }),
  actions: {
    removeNotification(id: string) {
      this.notifications = this.notifications.filter((n) => n.id !== id)
    },
    displayNotification(message: string, type: NotificationType, seconds: number = TOAST_SECONDS) {
      this.notifications.push({ id: crypto.randomUUID(), message, type, seconds })
    },
    displaySuccessNotification(message: string) {
      this.displayNotification(message, NotificationType.SUCCESS)
    },
    displayErrorNotification(message: string) {
      this.displayNotification(message, NotificationType.ERROR)
    },
  },
})
