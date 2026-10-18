export interface ImageSpec {
  ratio: number
  ratioLabel: string
  minWidth: number
  minHeight: number
}

export const imageSpecs = {
  banner: { ratio: 16 / 9, ratioLabel: '16:9', minWidth: 1920, minHeight: 1080 },
  card: { ratio: 16 / 10, ratioLabel: '16:10', minWidth: 1120, minHeight: 700 },
  desktop: { ratio: 2, ratioLabel: '2:1', minWidth: 1200, minHeight: 600 },
  mobile: { ratio: 9 / 19.5, ratioLabel: '9:19.5', minWidth: 390, minHeight: 845 },
} satisfies Record<string, ImageSpec>

export const MAX_CROPPED_SHARE = 0.2
export const SIZE_TOLERANCE = 0.15
