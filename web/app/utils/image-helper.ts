import { useRuntimeConfig } from 'nuxt/app'

export const getImageUrl = (image: string): string => {
  if (!image) return ''
  if (image.startsWith('http') || image.startsWith('blob:') || image.startsWith('data:')) return image

  return `${useRuntimeConfig().public.apiBase}/media/uploads/${image}`
}

export default {
  getImageUrl,
}
