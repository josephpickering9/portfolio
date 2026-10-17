import { describe, expect, it } from 'vitest'
import { isHttpUrl, URL_MESSAGE, validateRepositories, validateStats } from '~/utils/validation-helper'

describe('isHttpUrl', () => {
  it('accepts empty values and full http(s) URLs', () => {
    expect(isHttpUrl('')).toBe(true)
    expect(isHttpUrl(undefined)).toBe(true)
    expect(isHttpUrl('https://example.com')).toBe(true)
  })

  it('rejects partial or non-http URLs', () => {
    expect(isHttpUrl('example.com')).toBe(false)
    expect(isHttpUrl('ftp://example.com')).toBe(false)
  })
})

describe('validateRepositories', () => {
  it('returns no errors for complete rows', () => {
    expect(validateRepositories([{ title: 'API', url: 'https://github.com/a/b' }])).toEqual({})
  })

  it('reports missing titles and invalid URLs per row', () => {
    expect(
      validateRepositories([
        { title: 'API', url: 'https://github.com/a/b' },
        { title: '', url: 'github.com/a/b' },
      ]),
    ).toEqual({ 1: { title: 'Title is required', url: URL_MESSAGE } })
  })
})

describe('validateStats', () => {
  it('flags the empty fields of each stat', () => {
    expect(
      validateStats([
        { value: '240+', label: 'gigs logged' },
        { value: '', label: 'users' },
      ]),
    ).toEqual({ 1: ['value'] })
  })
})
