import type { NotificationType } from './NotificationType'

export interface Notification {
  id: string
  message: string
  type: NotificationType
  seconds: number
}
