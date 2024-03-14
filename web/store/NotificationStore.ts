import { defineStore } from 'pinia'
import { type Notification } from '../types/Notification'
import { NotificationType } from '../types/NotificationType'
import { NotificationPosition } from '../types/NotificationPosition'

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
    displayNotification(
      message: string,
      type: NotificationType,
      position: NotificationPosition = NotificationPosition.TOP_RIGHT,
      seconds: number = 5,
    ) {
      this.addNotification({ id: crypto.randomUUID().toString(), message, type, position, seconds })
    },
    displaySuccessNotification(message: string, position: NotificationPosition = NotificationPosition.TOP_RIGHT) {
      this.displayNotification(message, NotificationType.SUCCESS, position)
    },
    displayErrorNotification(message: string, position: NotificationPosition = NotificationPosition.TOP_RIGHT) {
      this.displayNotification(message, NotificationType.ERROR, position)
    },
    displayWarningNotification(message: string, position: NotificationPosition = NotificationPosition.TOP_RIGHT) {
      this.displayNotification(message, NotificationType.WARNING, position)
    },
    displayInfoNotification(message: string, position: NotificationPosition = NotificationPosition.TOP_RIGHT) {
      this.displayNotification(message, NotificationType.INFO, position)
    },
  },
})
