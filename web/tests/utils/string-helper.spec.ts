import { describe, expect, it } from 'vitest'
import { capitalise, formatCount, formatShortSha, getHost, includesSearch, padNumber, pluralise } from '~/utils/string-helper'

describe('capitalise', () => {
  it('upper-cases the first character only', () => {
    expect(capitalise('technology')).toBe('Technology')
    expect(capitalise('')).toBe('')
  })
})

describe('pluralise', () => {
  it('uses the singular for one and the plural otherwise', () => {
    expect(pluralise(1, 'project')).toBe('project')
    expect(pluralise(0, 'project')).toBe('projects')
    expect(pluralise(2, 'company', 'companies')).toBe('companies')
  })
})

describe('formatCount', () => {
  it('prefixes the count', () => {
    expect(formatCount(1, 'project')).toBe('1 project')
    expect(formatCount(3, 'cited project')).toBe('3 cited projects')
  })
})

describe('padNumber', () => {
  it('pads single digits to two characters', () => {
    expect(padNumber(1)).toBe('01')
    expect(padNumber(12)).toBe('12')
  })
})

describe('getHost', () => {
  it('strips the protocol and trailing slash', () => {
    expect(getHost('https://experience.josephpickering.co.uk/')).toBe('experience.josephpickering.co.uk')
    expect(getHost(undefined)).toBe('')
  })
})

describe('includesSearch', () => {
  it('matches case-insensitively and ignores surrounding whitespace', () => {
    expect(includesSearch('TypeScript', '  script ')).toBe(true)
    expect(includesSearch('Kotlin', 'swift')).toBe(false)
  })

  it('matches everything for an empty search', () => {
    expect(includesSearch('Vue', '')).toBe(true)
  })
})

describe('formatShortSha', () => {
  it('keeps the first seven characters of a commit SHA', () => {
    expect(formatShortSha('142bad0f2c9e4f1a8b7d')).toBe('142bad0')
  })

  it('returns an empty string when no SHA is set', () => {
    expect(formatShortSha('')).toBe('')
  })
})
