import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { useEntityForm } from '~/composables/useEntityForm'
import { useNotificationStore } from '~/store/NotificationStore'
import { NotificationType } from '~/types/NotificationType'

const push = vi.hoisted(() => vi.fn())

vi.mock('vue-router', () => ({ useRouter: () => ({ push }) }))

interface Entity {
  id: string
  slug: string
}

function setup(errors: { load?: string; save?: string; delete?: string } = {}) {
  const actions = {
    get: vi.fn<(key: string) => Promise<Entity | undefined>>(),
    create: vi.fn<(data: string) => Promise<Entity | undefined>>(),
    update: vi.fn<(id: string, data: string) => Promise<Entity | undefined>>(),
    delete: vi.fn<(id: string) => Promise<void>>(),
  }
  const form = useEntityForm<Entity, string>({
    entityName: 'company',
    listRoute: '/companies',
    savedRoute: (saved) => `/companies/${saved.slug}`,
    actions,
    state: { loadError: () => errors.load, saveError: () => errors.save, deleteError: () => errors.delete },
  })
  return { actions, form, notifications: useNotificationStore() }
}

function toasts() {
  return useNotificationStore().notifications.map(({ message, type }) => ({ message, type }))
}

describe('useEntityForm', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    push.mockReset()
  })

  it('creates, toasts and redirects to the saved route', async () => {
    const { actions, form } = setup()
    actions.create.mockResolvedValue({ id: '1', slug: 'acme' })

    await form.save(undefined, 'payload')

    expect(actions.create).toHaveBeenCalledWith('payload')
    expect(toasts()).toEqual([{ message: 'Company created successfully', type: NotificationType.SUCCESS }])
    expect(push).toHaveBeenCalledWith('/companies/acme')
  })

  it('updates when given an id', async () => {
    const { actions, form } = setup()
    actions.update.mockResolvedValue({ id: '1', slug: 'acme' })

    await form.save('1', 'payload')

    expect(actions.update).toHaveBeenCalledWith('1', 'payload')
    expect(toasts()[0]?.message).toBe('Company updated successfully')
  })

  it("shows the store's save error and stays on the page", async () => {
    const { actions, form } = setup({ save: 'That clashes with something' })
    actions.create.mockResolvedValue(undefined)

    await form.save(undefined, 'payload')

    expect(toasts()).toEqual([{ message: 'That clashes with something', type: NotificationType.ERROR }])
    expect(push).not.toHaveBeenCalled()
  })

  it('falls back to a generic save message', async () => {
    const { actions, form } = setup()
    actions.create.mockResolvedValue(undefined)

    await form.save(undefined, 'payload')

    expect(toasts()[0]?.message).toBe("Couldn't save the company")
  })

  it('returns the fetched entity, or toasts the load error', async () => {
    const loaded = setup()
    loaded.actions.get.mockResolvedValue({ id: '1', slug: 'acme' })
    expect(await loaded.form.fetch('1')).toEqual({ id: '1', slug: 'acme' })

    setActivePinia(createPinia())
    const failed = setup({ load: 'That company could not be found' })
    failed.actions.get.mockResolvedValue(undefined)
    expect(await failed.form.fetch('1')).toBeUndefined()
    expect(toasts()[0]?.message).toBe('That company could not be found')
  })

  it('deletes, toasts and returns to the list', async () => {
    const { actions, form } = setup()
    actions.delete.mockResolvedValue()

    await form.remove('1')

    expect(toasts()[0]?.message).toBe('Company deleted')
    expect(push).toHaveBeenCalledWith('/companies')
  })

  it('shows the delete error and stays on the page', async () => {
    const { actions, form } = setup({ delete: "You don't have permission to do that." })
    actions.delete.mockResolvedValue()

    await form.remove('1')

    expect(toasts()[0]?.message).toBe("You don't have permission to do that.")
    expect(push).not.toHaveBeenCalled()
  })

  it('toasts when the form is invalid', () => {
    const { form } = setup()

    form.invalid()

    expect(toasts()).toEqual([{ message: 'Fix the highlighted fields', type: NotificationType.ERROR }])
  })
})
