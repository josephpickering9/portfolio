const PASSTHROUGH_PREFIXES = ['http', 'blob:', 'data:']
const UPLOADS_PATH = '/media/uploads/'

export function buildImageUrl(image: string, apiBase: string): string {
  if (!image) return ''
  if (PASSTHROUGH_PREFIXES.some((prefix) => image.startsWith(prefix))) return image
  return `${apiBase}${UPLOADS_PATH}${image}`
}
