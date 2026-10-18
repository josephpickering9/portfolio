import { describe, expect, it } from 'vitest'
import type { ImageSpec } from '~/data/ImageSpecs'
import { croppedShare, describeImageFit, describeSpec, visibleWidth } from '~/utils/image-fit-helper'

const wide: ImageSpec = { ratio: 16 / 9, ratioLabel: '16:9', minWidth: 2400, minHeight: 1350 }
const card: ImageSpec = { ratio: 16 / 10, ratioLabel: '16:10', minWidth: 1200, minHeight: 750 }
const twoToOne: ImageSpec = { ratio: 2, ratioLabel: '2:1', minWidth: 2400, minHeight: 1200 }

describe('croppedShare', () => {
  it('is zero when the shape matches', () => {
    expect(croppedShare({ width: 1600, height: 900 }, 16 / 9)).toBeCloseTo(0)
  })

  it('measures how much a wide strip loses in a 16:9 slot', () => {
    expect(croppedShare({ width: 1920, height: 516 }, 16 / 9)).toBeCloseTo(0.52, 2)
  })
})

describe('visibleWidth', () => {
  it('is the cropped width for images wider than the slot', () => {
    expect(visibleWidth({ width: 3840, height: 1032 }, 16 / 9)).toBeCloseTo(1834.7, 1)
  })

  it('is the full width for images narrower than the slot', () => {
    expect(visibleWidth({ width: 1000, height: 1000 }, 2)).toBe(1000)
  })
})

describe('describeImageFit', () => {
  it('says nothing for a well-sized image of the right shape', () => {
    expect(describeImageFit({ width: 2560, height: 1440 }, wide)).toBeUndefined()
  })

  it('tolerates shapes that only lose a little to cropping', () => {
    expect(describeImageFit({ width: 2400, height: 1290 }, wide)).toBeUndefined()
    expect(describeImageFit({ width: 1520, height: 800 }, card)).toBeUndefined()
  })

  it('warns about small images', () => {
    expect(describeImageFit({ width: 800, height: 500 }, card)).toBe(
      '800×500 is smaller than the recommended 1200×750, so it may look soft.',
    )
  })

  it('warns about the wrong shape with the share that will be cropped', () => {
    expect(describeImageFit({ width: 4800, height: 1290 }, wide)).toBe("It isn't 16:9, so about 52% will be cropped.")
  })

  it('judges sharpness on the part that is left after cropping', () => {
    expect(describeImageFit({ width: 3840, height: 1032 }, wide)).toBe(
      "It isn't 16:9, so about 52% will be cropped, and what's left is smaller than the recommended 2400×1350, so it may look soft.",
    )
  })

  it('combines both problems for a small image of the wrong shape', () => {
    expect(describeImageFit({ width: 1000, height: 1000 }, twoToOne)).toBe(
      "It isn't 2:1, so about 50% will be cropped, and what's left is smaller than the recommended 2400×1200, so it may look soft.",
    )
  })
})

describe('describeSpec', () => {
  it('summarises the recommended shape and size', () => {
    expect(describeSpec(card)).toBe('16:10, at least 1200×750')
  })
})
