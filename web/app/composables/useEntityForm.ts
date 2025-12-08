
import type { useRouter } from 'vue-router'
import type { useNotificationStore } from '~/store/NotificationStore'

interface EntityFormOptions<T, CreateT, UpdateT = CreateT> {
    store: any
    notificationStore: ReturnType<typeof useNotificationStore>
    router: ReturnType<typeof useRouter>
    redirectRoute: string
    actions: {
        get: (id: string) => Promise<T | undefined>
        create: (data: CreateT) => Promise<T | undefined>
        update: (id: string, data: UpdateT) => Promise<T | undefined>
        delete: (id: string) => Promise<void>
    }
    state: {
        entity: () => T | undefined
        error: () => string | undefined
        loading: () => boolean
        createError: () => string | undefined
        loadingType?: 'creating' | 'updating' | 'loading'
    }
}

export function useEntityForm<T extends { id: string | number }, CreateT, UpdateT = CreateT>(options: EntityFormOptions<T, CreateT, UpdateT>) {
    const { notificationStore, router, redirectRoute, actions, state } = options

    const save = async (id: string | null, data: CreateT | UpdateT, isUpdate: boolean) => {
        let response: T | undefined

        if (isUpdate && id) {
            response = await actions.update(id, data as UpdateT)
        } else {
            response = await actions.create(data as CreateT)
        }

        if (!state.createError() && response) {
            router.push(redirectRoute)
            notificationStore.displaySuccessNotification(`${isUpdate ? 'Updated' : 'Created'} successfully`)
        } else {
            notificationStore.displayErrorNotification(state.createError() || 'An error occurred')
        }
    }

    const remove = async (id: string) => {
        await actions.delete(id)

        if (!state.createError()) {
            router.push(redirectRoute)
            notificationStore.displaySuccessNotification('Deleted successfully')
        } else {
            notificationStore.displayErrorNotification(state.createError() || 'An error occurred')
        }
    }

    const fetch = async (id: string) => {
        await actions.get(id)
        if (state.error()) {
            notificationStore.displayErrorNotification(state.error() || 'An error occurred')
        }
        return state.entity()
    }

    return {
        save,
        remove,
        fetch,
    }
}
