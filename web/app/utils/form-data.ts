function appendValue(formData: FormData, key: string, value: unknown) {
  if (value === null || value === undefined) return

  if (value instanceof Blob) {
    formData.append(key, value)
  } else if (value instanceof Date) {
    formData.append(key, value.toISOString())
  } else if (Array.isArray(value)) {
    value.forEach((item, index) => appendValue(formData, `${key}[${index}]`, item))
  } else if (typeof value === 'object') {
    Object.entries(value).forEach(([childKey, child]) => appendValue(formData, key ? `${key}.${childKey}` : childKey, child))
  } else {
    formData.append(key, String(value))
  }
}

export function serializeToFormData(body: unknown): FormData {
  const formData = new FormData()
  if (typeof body === 'object' && body !== null && !Array.isArray(body)) {
    Object.entries(body).forEach(([key, value]) => appendValue(formData, key, value))
  }
  return formData
}
