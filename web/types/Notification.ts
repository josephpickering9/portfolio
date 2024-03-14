import { NotificationType } from './NotificationType'
import { NotificationPosition } from './NotificationPosition'

export interface Notification {
  id: string
  message: string
  type: NotificationType
  position: NotificationPosition
  seconds: number
}
