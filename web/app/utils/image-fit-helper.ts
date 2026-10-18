import type { ImageSpec } from '~/data/ImageSpecs'
import { MAX_CROPPED_SHARE, SIZE_TOLERANCE } from '~/data/ImageSpecs'

export interface ImageSize {
  width: number
  height: number
}

export function croppedShare(size: ImageSize, ratio: number): number {
  const actual = size.width / size.height
  return 1 - (actual > ratio ? ratio / actual : actual / ratio)
}

export function visibleWidth(size: ImageSize, ratio: number): number {
  return size.width / size.height > ratio ? size.height * ratio : size.width
}

export function describeImageFit(size: ImageSize, spec: ImageSpec): string | undefined {
  const wrongShape = croppedShare(size, spec.ratio) > MAX_CROPPED_SHARE
  const tooSmall = visibleWidth(size, spec.ratio) < spec.minWidth * (1 - SIZE_TOLERANCE)
  const recommended = `${spec.minWidth}×${spec.minHeight}`

  if (wrongShape && tooSmall) {
    const percent = Math.round(croppedShare(size, spec.ratio) * 100)
    return `It isn't ${spec.ratioLabel}, so about ${percent}% will be cropped, and what's left is smaller than the recommended ${recommended}, so it may look soft.`
  }
  if (wrongShape) {
    return `It isn't ${spec.ratioLabel}, so about ${Math.round(croppedShare(size, spec.ratio) * 100)}% will be cropped.`
  }
  if (tooSmall) return `${size.width}×${size.height} is smaller than the recommended ${recommended}, so it may look soft.`
  return undefined
}

export function describeSpec(spec: ImageSpec): string {
  return `${spec.ratioLabel}, at least ${spec.minWidth}×${spec.minHeight}`
}
