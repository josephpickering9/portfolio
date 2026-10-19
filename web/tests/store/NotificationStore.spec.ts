import { beforeEach, describe, expect, it } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { useNotificationStore } from '~/store/NotificationStore'
import { NotificationType } from '~/types/NotificationType'

describe('NotificationStore', () => {
  beforeEach(() => setActivePinia(createPinia()))

  it('queues success and error toasts with the default duration', () => {
    const store = useNotificationStore()

    store.displaySuccessNotification('Saved')
    store.displayErrorNotification('Failed')

    expect(store.notifications.map(({ message, type }) => ({ message, type }))).toEqual([
      { message: 'Saved', type: NotificationType.SUCCESS },
      { message: 'Failed', type: NotificationType.ERROR },
    ])
    expect(store.notifications[0]?.seconds).toBe(3.2)
  })

  it('accepts a custom duration', () => {
    const store = useNotificationStore()

    store.displayNotification('Heads up', NotificationType.INFO, 10)

    expect(store.notifications[0]?.seconds).toBe(10)
  })

  it('removes a toast by id', () => {
    const store = useNotificationStore()
    store.displaySuccessNotification('One')
    store.displaySuccessNotification('Two')
    const [first] = store.notifications

    if (first) store.removeNotification(first.id)

    expect(store.notifications.map((notification) => notification.message)).toEqual(['Two'])
  })
})
