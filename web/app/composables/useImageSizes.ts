import { reactive, watch } from 'vue'
import type { ImageSize } from '~/utils/image-fit-helper'

export function useImageSizes(urls: () => string[]) {
  const sizes = reactive<Record<string, ImageSize>>({})

  function measure(url: string) {
    if (sizes[url] || !import.meta.client) return
    const image = new Image()
    image.onload = () => {
      sizes[url] = { width: image.naturalWidth, height: image.naturalHeight }
    }
    image.src = url
  }

  watch(urls, (list) => list.forEach(measure), { immediate: true })

  return sizes
}
