import { afterEach, describe, expect, it, vi } from 'vitest'
import { useImageUrl } from '~/composables/useImageUrl'

describe('useImageUrl', () => {
  afterEach(() => vi.unstubAllGlobals())

  it('resolves uploads against the configured API base', () => {
    vi.stubGlobal('useRuntimeConfig', () => ({ public: { apiBase: 'https://api.example.com' } }))

    const { getImageUrl } = useImageUrl()

    expect(getImageUrl('logo.png')).toBe('https://api.example.com/media/uploads/logo.png')
  })
})
