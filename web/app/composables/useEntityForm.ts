import { useRouter } from 'vue-router'
import { useNotificationStore } from '~/store/NotificationStore'
import { capitalise } from '~/utils/string-helper'

const INVALID_FORM_MESSAGE = 'Fix the highlighted fields'

interface EntityFormOptions<T, CreateT> {
  entityName: string
  listRoute: string
  savedRoute?: (saved: T) => string
  actions: {
    get: (key: string) => Promise<T | undefined>
    create: (data: CreateT) => Promise<T | undefined>
    update: (id: string, data: CreateT) => Promise<T | undefined>
    delete: (id: string) => Promise<void>
  }
  state: {
    loadError: () => string | undefined
    saveError: () => string | undefined
    deleteError: () => string | undefined
  }
}

export function useEntityForm<T, CreateT>(options: EntityFormOptions<T, CreateT>) {
  const { entityName, listRoute, savedRoute, actions, state } = options
  const router = useRouter()
  const notificationStore = useNotificationStore()

  async function fetch(key: string): Promise<T | undefined> {
    const entity = await actions.get(key)
    const error = state.loadError()
    if (!entity || error) {
      notificationStore.displayErrorNotification(error || `Couldn't load the ${entityName}`)
      return undefined
    }
    return entity
  }

  async function save(id: string | undefined, data: CreateT): Promise<void> {
    const saved = id ? await actions.update(id, data) : await actions.create(data)
    const error = state.saveError()
    if (!saved || error) {
      notificationStore.displayErrorNotification(error || `Couldn't save the ${entityName}`)
      return
    }
    notificationStore.displaySuccessNotification(`${capitalise(entityName)} ${id ? 'updated' : 'created'} successfully`)
    await router.push(savedRoute ? savedRoute(saved) : listRoute)
  }

  async function remove(id: string): Promise<void> {
    await actions.delete(id)
    const error = state.deleteError()
    if (error) {
      notificationStore.displayErrorNotification(error)
      return
    }
    notificationStore.displaySuccessNotification(`${capitalise(entityName)} deleted`)
    await router.push(listRoute)
  }

  function invalid(): void {
    notificationStore.displayErrorNotification(INVALID_FORM_MESSAGE)
  }

  return { fetch, save, remove, invalid }
}
