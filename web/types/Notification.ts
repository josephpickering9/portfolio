import type { NotificationType } from './NotificationType'
import type { NotificationPosition } from './NotificationPosition'

export interface Notification {
  id: string
  message: string
  type: NotificationType
  position: NotificationPosition
  seconds: number
}
