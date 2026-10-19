import { describe, expect, it } from 'vitest'
import { ImageType } from '@api'
import { toProjectImages } from '~/utils/upload-helper'
import type { ProjectImageSlots } from '~/utils/upload-helper'

const file = new File(['x'], 'new.png', { type: 'image/png' })

function slots(overrides: Partial<ProjectImageSlots> = {}): ProjectImageSlots {
  return { banner: null, card: null, logo: null, desktop: [], mobile: [], ...overrides }
}

describe('toProjectImages', () => {
  it('returns nothing when there are no images', () => {
    expect(toProjectImages(slots())).toEqual([])
  })

  it('keeps existing single images by id and uploads new ones as files', () => {
    const result = toProjectImages(
      slots({
        banner: { key: 'b', id: 'banner-id', previewUrl: '' },
        card: { key: 'c', file, previewUrl: 'blob:card' },
      }),
    )

    expect(result).toEqual([
      { id: 'banner-id', type: ImageType.BANNER, order: undefined },
      { type: ImageType.CARD, image: file, order: undefined },
    ])
  })

  it('orders gallery images by their position, mixing existing and new images', () => {
    const result = toProjectImages(
      slots({
        desktop: [
          { key: 'n', file, previewUrl: 'blob:new' },
          { key: 'e', id: 'existing', previewUrl: '' },
        ],
        mobile: [{ key: 'm', id: 'mobile', previewUrl: '' }],
      }),
    )

    expect(result).toEqual([
      { type: ImageType.DESKTOP, image: file, order: 1 },
      { id: 'existing', type: ImageType.DESKTOP, order: 2 },
      { id: 'mobile', type: ImageType.MOBILE, order: 1 },
    ])
  })
})
