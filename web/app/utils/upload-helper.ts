import { ImageType } from '@api'
import type { CreateProjectImage, ProjectImage } from '@api'
import type { UploadItem } from '~/types/UploadItem'

export interface ProjectImageSlots {
  banner: UploadItem | null
  card: UploadItem | null
  logo: UploadItem | null
  desktop: UploadItem[]
  mobile: UploadItem[]
}

export function fromExistingImage(image: ProjectImage, previewUrl: string): UploadItem {
  return { key: image.id, id: image.id, previewUrl }
}

export function fromFile(file: File, previewUrl: string): UploadItem {
  return { key: `${file.name}-${file.lastModified}-${previewUrl}`, file, previewUrl }
}

function toImage(item: UploadItem, type: ImageType, order?: number): CreateProjectImage {
  return item.file ? { type, image: item.file, order } : { id: item.id, type, order }
}

export function toProjectImages(slots: ProjectImageSlots): CreateProjectImage[] {
  const singles: [UploadItem | null, ImageType][] = [
    [slots.logo, ImageType.LOGO],
    [slots.banner, ImageType.BANNER],
    [slots.card, ImageType.CARD],
  ]
  const galleries: [UploadItem[], ImageType][] = [
    [slots.desktop, ImageType.DESKTOP],
    [slots.mobile, ImageType.MOBILE],
  ]

  return [
    ...singles.flatMap(([item, type]) => (item ? [toImage(item, type)] : [])),
    ...galleries.flatMap(([items, type]) => items.map((item, index) => toImage(item, type, index + 1))),
  ]
}
