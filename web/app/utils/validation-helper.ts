import type { CreateProjectRepository, CreateProjectStat } from '@api'

export const URL_MESSAGE = 'Enter a full URL, e.g. https://example.com'
export const CASE_STUDY_MAX_LENGTH = 2000

export function isHttpUrl(value?: string | null): boolean {
  if (!value) return true
  try {
    const parsed = new URL(value)
    return parsed.protocol === 'http:' || parsed.protocol === 'https:'
  } catch {
    return false
  }
}

export function validateRepositories(repositories: CreateProjectRepository[]): Record<number, { title?: string; url?: string }> {
  const errors: Record<number, { title?: string; url?: string }> = {}
  repositories.forEach((repository, index) => {
    const rowErrors: { title?: string; url?: string } = {}
    if (!repository.title.trim()) rowErrors.title = 'Title is required'
    if (!repository.url.trim()) rowErrors.url = 'URL is required'
    else if (!isHttpUrl(repository.url)) rowErrors.url = URL_MESSAGE
    if (rowErrors.title || rowErrors.url) errors[index] = rowErrors
  })
  return errors
}

export function validateStats(stats: CreateProjectStat[]): Record<number, ('value' | 'label')[]> {
  const errors: Record<number, ('value' | 'label')[]> = {}
  stats.forEach((stat, index) => {
    const fields = (['value', 'label'] as const).filter((field) => !stat[field].trim())
    if (fields.length) errors[index] = [...fields]
  })
  return errors
}
