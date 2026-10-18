import { useRuntimeConfig } from '#imports'
import { buildImageUrl } from '~/utils/image-helper'

export function useImageUrl() {
  const { apiBase } = useRuntimeConfig().public

  function getImageUrl(image: string): string {
    return buildImageUrl(image, apiBase)
  }

  return { getImageUrl }
}
