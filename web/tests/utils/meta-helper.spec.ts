import { describe, expect, it } from 'vitest'
import { formatPageTitle, SITE_NAME } from '~/utils/meta-helper'

describe('formatPageTitle', () => {
  it('appends the site name to a page title', () => {
    expect(formatPageTitle('Projects')).toBe(`Projects | ${SITE_NAME}`)
  })

  it('falls back to the site name alone', () => {
    expect(formatPageTitle()).toBe(SITE_NAME)
    expect(formatPageTitle('')).toBe(SITE_NAME)
  })
})
