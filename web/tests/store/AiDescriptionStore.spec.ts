import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { ERROR_MESSAGES, ErrorCodes } from '~/types/errors'
import { fail, ok } from '../helpers/api-responses'

const api = vi.hoisted(() => ({ postVertexProjectsByIdDescriptionSuggest: vi.fn() }))

vi.mock('@api', async (importOriginal) => ({ ...(await importOriginal<object>()), ...api }))

const { useAiDescriptionStore } = await import('~/store/AiDescriptionStore')

describe('AiDescriptionStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.resetAllMocks()
  })

  it('returns the suggestion and sends the request body', async () => {
    const store = useAiDescriptionStore()
    api.postVertexProjectsByIdDescriptionSuggest.mockResolvedValue(ok({ suggestedDescription: 'A portfolio site' }))

    const pending = store.suggestDescription('p1', { additionalContext: 'Nuxt' })
    expect(store.suggesting).toBe(true)

    expect(await pending).toBe('A portfolio site')
    expect(store.suggesting).toBe(false)
    expect(store.suggestedDescription).toBe('A portfolio site')
    expect(api.postVertexProjectsByIdDescriptionSuggest).toHaveBeenCalledWith({
      path: { id: 'p1' },
      body: { additionalContext: 'Nuxt' },
    })
  })

  it('records the mapped error when the AI service is down', async () => {
    const store = useAiDescriptionStore()
    api.postVertexProjectsByIdDescriptionSuggest.mockResolvedValue(fail(ErrorCodes.UpstreamUnavailable, 503))

    expect(await store.suggestDescription('p1', {})).toBeUndefined()
    expect(store.suggestionError).toBe(ERROR_MESSAGES[ErrorCodes.UpstreamUnavailable])
  })
})
