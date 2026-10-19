import { describe, expect, it } from 'vitest'
import { formatDateRange, formatDuration, formatYear, toDateInputValue, wholeYearsSince } from '~/utils/date-helper'

const now = new Date('2026-10-16T12:00:00Z')

describe('formatDuration', () => {
  it('formats months only', () => {
    expect(formatDuration('2026-03-01', null, now)).toBe('7 months')
  })

  it('formats whole years with a plural', () => {
    expect(formatDuration('2021-03-01', '2024-03-01', now)).toBe('3 years')
  })

  it('formats years and months', () => {
    expect(formatDuration('2023-01-01', '2024-03-01', now)).toBe('1 year 2 months')
  })

  it('returns an empty string for less than a month', () => {
    expect(formatDuration('2026-10-01', null, now)).toBe('')
  })
})

describe('formatDateRange', () => {
  it('shows Present when there is no end date', () => {
    expect(formatDateRange('2026-03-01', null, now)).toBe('Mar 2026 — Present (7 months)')
  })

  it('shows both months and the duration', () => {
    expect(formatDateRange('2021-03-01', '2024-03-01', now)).toBe('Mar 2021 — Mar 2024 (3 years)')
  })

  it('returns an empty string without a start date', () => {
    expect(formatDateRange(undefined, null, now)).toBe('')
  })
})

describe('formatYear', () => {
  it('formats the four-digit year', () => {
    expect(formatYear(now)).toBe('2026')
  })
})

describe('wholeYearsSince', () => {
  it('counts only completed years', () => {
    expect(wholeYearsSince(new Date('2016-07-01T00:00:00Z'), now)).toBe(10)
    expect(wholeYearsSince(new Date('2016-11-01T00:00:00Z'), now)).toBe(9)
  })
})

describe('toDateInputValue', () => {
  it('keeps the date part of an ISO timestamp', () => {
    expect(toDateInputValue('2024-03-01T00:00:00Z')).toBe('2024-03-01')
  })

  it('returns undefined for a missing date', () => {
    expect(toDateInputValue(null)).toBeUndefined()
    expect(toDateInputValue(undefined)).toBeUndefined()
  })
})
