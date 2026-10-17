import { useRouter } from 'vue-router'
import { useNotificationStore } from '~/store/NotificationStore'

interface EntityFormOptions<T, CreateT> {
  entityName: string
  redirectRoute: string
  actions: {
    get: (id: string) => Promise<T | undefined>
    create: (data: CreateT) => Promise<T | undefined>
    update: (id: string, data: CreateT) => Promise<T | undefined>
    delete: (id: string) => Promise<void>
  }
  state: {
    loadError: () => string | undefined
    saveError: () => string | undefined
  }
}

export function useEntityForm<T, CreateT>(options: EntityFormOptions<T, CreateT>) {
  const { entityName, redirectRoute, actions, state } = options
  const router = useRouter()
  const notificationStore = useNotificationStore()
  const capitalised = entityName.charAt(0).toUpperCase() + entityName.slice(1)

  async function fetch(id: string): Promise<T | undefined> {
    const entity = await actions.get(id)
    if (!entity || state.loadError()) {
      notificationStore.displayErrorNotification(state.loadError() || `Couldn't load the ${entityName}`)
      return undefined
    }
    return entity
  }

  async function save(id: string | undefined, data: CreateT) {
    const saved = id ? await actions.update(id, data) : await actions.create(data)
    if (state.saveError() || !saved) {
      notificationStore.displayErrorNotification(state.saveError() || `Couldn't save the ${entityName}`)
      return
    }
    notificationStore.displaySuccessNotification(`${capitalised} ${id ? 'updated' : 'created'} successfully`)
    router.push(redirectRoute)
  }

  async function remove(id: string) {
    await actions.delete(id)
    if (state.saveError()) {
      notificationStore.displayErrorNotification(state.saveError() || `Couldn't delete the ${entityName}`)
      return
    }
    notificationStore.displaySuccessNotification(`${capitalised} deleted`)
    router.push(redirectRoute)
  }

  function invalid() {
    notificationStore.displayErrorNotification('Fix the highlighted fields')
  }

  return { fetch, save, remove, invalid }
}
