import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { ERROR_MESSAGES, ErrorCodes } from '~/types/errors'
import { fail, ok } from '../helpers/api-responses'

const api = vi.hoisted(() => ({ putProjectimageOptimise: vi.fn() }))

vi.mock('@api', async (importOriginal) => ({ ...(await importOriginal<object>()), ...api }))

const { useProjectImageStore } = await import('~/store/ProjectImageStore')

describe('ProjectImageStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.resetAllMocks()
  })

  it('sets optimising while the request is in flight', async () => {
    const store = useProjectImageStore()
    api.putProjectimageOptimise.mockResolvedValue(ok({}))

    const pending = store.optimiseImages()
    expect(store.optimising).toBe(true)
    await pending

    expect(store.optimising).toBe(false)
    expect(store.optimiseError).toBeUndefined()
  })

  it('records the mapped error on failure', async () => {
    const store = useProjectImageStore()
    api.putProjectimageOptimise.mockResolvedValue(fail(ErrorCodes.InternalError, 500))

    await store.optimiseImages()

    expect(store.optimiseError).toBe(ERROR_MESSAGES[ErrorCodes.InternalError])
  })

  it('ignores a second call while one is in flight', async () => {
    const store = useProjectImageStore()
    api.putProjectimageOptimise.mockResolvedValue(ok({}))

    const first = store.optimiseImages()
    await store.optimiseImages()
    await first

    expect(api.putProjectimageOptimise).toHaveBeenCalledTimes(1)
  })
})
