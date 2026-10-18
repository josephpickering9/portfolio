import { describe, expect, it } from 'vitest'
import { buildImageUrl } from '~/utils/image-helper'

const API_BASE = 'https://api.example.com'

describe('buildImageUrl', () => {
  it('points an uploaded file name at the API uploads path', () => {
    expect(buildImageUrl('pulse.webp', API_BASE)).toBe('https://api.example.com/media/uploads/pulse.webp')
  })

  it('leaves absolute, blob and data URLs untouched', () => {
    expect(buildImageUrl('https://cdn.example.com/a.png', API_BASE)).toBe('https://cdn.example.com/a.png')
    expect(buildImageUrl('blob:local-preview', API_BASE)).toBe('blob:local-preview')
    expect(buildImageUrl('data:image/png;base64,AAA', API_BASE)).toBe('data:image/png;base64,AAA')
  })

  it('returns an empty string for a missing image', () => {
    expect(buildImageUrl('', API_BASE)).toBe('')
  })
})
