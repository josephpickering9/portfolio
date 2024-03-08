export const getImageUrl = (image: string): string => {
  if (!image) return ''
  if (image.startsWith('http')) return image

  return `${useRuntimeConfig().public.apiBase}/media/uploads/${image}`
}

export default {
  getImageUrl,
}
