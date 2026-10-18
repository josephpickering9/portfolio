import { differenceInCalendarMonths, format, parseISO } from 'date-fns'

const MONTHS_PER_YEAR = 12

export function formatMonth(date: string): string {
  return format(parseISO(date), 'MMM yyyy')
}

export function formatDuration(startDate: string, endDate?: string | null, now: Date = new Date()): string {
  const end = endDate ? parseISO(endDate) : now
  const totalMonths = differenceInCalendarMonths(end, parseISO(startDate))
  if (totalMonths <= 0) return ''

  const years = Math.floor(totalMonths / MONTHS_PER_YEAR)
  const months = totalMonths % MONTHS_PER_YEAR
  const parts = []
  if (years) parts.push(pluralise(years, 'year'))
  if (months) parts.push(pluralise(months, 'month'))
  return parts.join(' ')
}

function pluralise(count: number, unit: string): string {
  return `${count} ${unit}${count === 1 ? '' : 's'}`
}

export function formatDateRange(startDate?: string | null, endDate?: string | null, now: Date = new Date()): string {
  if (!startDate) return ''

  const range = `${formatMonth(startDate)} — ${endDate ? formatMonth(endDate) : 'Present'}`
  const duration = formatDuration(startDate, endDate, now)
  return duration ? `${range} (${duration})` : range
}
