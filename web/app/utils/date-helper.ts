import { format, formatDuration, intervalToDuration, parseISO } from 'date-fns'
import type { Company } from '@api'

export function getDateRange(company: Company): string {
  const start = company.startDate ? format(parseISO(company.startDate), 'MMM yyyy') : ''
  const end = company.endDate ? format(parseISO(company.endDate), 'MMM yyyy') : 'Present'

  if (!start) return ''
  return `${start} - ${end}`
}

export function getDuration(company: Company): string {
  if (!company.startDate) return ''
  const end = company.endDate ? parseISO(company.endDate) : new Date()
  const duration = intervalToDuration({ start: parseISO(company.startDate), end })
  return formatDuration(duration, { format: ['years', 'months'] })
}
