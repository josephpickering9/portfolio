import { differenceInCalendarMonths, differenceInYears, format, parseISO } from 'date-fns'
import { formatCount } from '~/utils/string-helper'

const MONTHS_PER_YEAR = 12
const ISO_DATE_LENGTH = 10

export function formatMonth(date: string): string {
  return format(parseISO(date), 'MMM yyyy')
}

export function formatYear(date: Date): string {
  return format(date, 'yyyy')
}

export function formatDuration(startDate: string, endDate?: string | null, now: Date = new Date()): string {
  const end = endDate ? parseISO(endDate) : now
  const totalMonths = differenceInCalendarMonths(end, parseISO(startDate))
  if (totalMonths <= 0) return ''

  const years = Math.floor(totalMonths / MONTHS_PER_YEAR)
  const months = totalMonths % MONTHS_PER_YEAR
  const parts = []
  if (years) parts.push(formatCount(years, 'year'))
  if (months) parts.push(formatCount(months, 'month'))
  return parts.join(' ')
}

export function formatDateRange(startDate?: string | null, endDate?: string | null, now: Date = new Date()): string {
  if (!startDate) return ''

  const range = `${formatMonth(startDate)} — ${endDate ? formatMonth(endDate) : 'Present'}`
  const duration = formatDuration(startDate, endDate, now)
  return duration ? `${range} (${duration})` : range
}

export function wholeYearsSince(start: Date, now: Date = new Date()): number {
  return differenceInYears(now, start)
}

export function toDateInputValue(date?: string | null): string | undefined {
  return date ? date.slice(0, ISO_DATE_LENGTH) : undefined
}
