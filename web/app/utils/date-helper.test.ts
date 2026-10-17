import { describe, expect, it } from 'vitest'
import { formatDateRange, formatDuration } from '~/utils/date-helper'

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
