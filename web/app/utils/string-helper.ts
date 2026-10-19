const PAD_LENGTH = 2

export function capitalise(text: string): string {
  return text.charAt(0).toUpperCase() + text.slice(1)
}

export function pluralise(count: number, singular: string, plural = `${singular}s`): string {
  return count === 1 ? singular : plural
}

export function formatCount(count: number, singular: string, plural?: string): string {
  return `${count} ${pluralise(count, singular, plural)}`
}

export function padNumber(value: number): string {
  return String(value).padStart(PAD_LENGTH, '0')
}

export function getHost(url?: string | null): string {
  return (url ?? '').replace(/^https?:\/\//, '').replace(/\/$/, '')
}

export function includesSearch(text: string, search: string): boolean {
  return text.toLowerCase().includes(search.trim().toLowerCase())
}
