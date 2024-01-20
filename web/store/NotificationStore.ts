import { defineStore } from 'pinia'
import { type Notification } from '../types/Notification'
import { NotificationType } from '../types/NotificationType'

export const useNotificationStore = defineStore('notificationStore', {
  state: () => ({
    notifications: [] as Notification[],
  }),
  actions: {
    addNotification(notification: Notification) {
      this.notifications.push(notification)
    },
    removeNotification(id: string) {
      this.notifications = this.notifications.filter((n) => n.id !== id)
    },
    displayNotification(message: string, type: NotificationType, seconds: number = 5) {
      this.addNotification({ id: crypto.randomUUID().toString(), message, type, seconds })
    },
    displaySuccessNotification(message: string) {
      this.displayNotification(message, NotificationType.SUCCESS)
    },
    displayErrorNotification(message: string) {
      this.displayNotification(message, NotificationType.ERROR)
    },
    displayWarningNotification(message: string) {
      this.displayNotification(message, NotificationType.WARNING)
    },
    displayInfoNotification(message: string) {
      this.displayNotification(message, NotificationType.INFO)
    },
  },
})
