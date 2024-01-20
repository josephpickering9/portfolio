export const getImageUrl = (image: string): string => {
  return `${useRuntimeConfig().public.apiBase}/media/uploads/${image}`
}

export default {
  getImageUrl,
}
