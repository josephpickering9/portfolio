import { describe, expect, it } from 'vitest'
import type { Company } from '@api'
import { companyToForm, sortCompaniesNewestFirst } from '~/utils/company-helper'

function company(name: string, startDate?: string): Company {
  return { id: name, name, description: '', slug: name.toLowerCase(), startDate }
}

describe('sortCompaniesNewestFirst', () => {
  it('puts undated companies first, then newest start date first', () => {
    const companies = [company('Old', '2015-01-01'), company('New', '2024-03-01'), company('Personal')]
    expect(sortCompaniesNewestFirst(companies).map((c) => c.name)).toEqual(['Personal', 'New', 'Old'])
  })

  it('does not mutate the input', () => {
    const companies = [company('Old', '2015-01-01'), company('New', '2024-03-01')]
    sortCompaniesNewestFirst(companies)
    expect(companies[0]?.name).toBe('Old')
  })
})

describe('companyToForm', () => {
  it('copies the editable fields, trimming dates to the date input format', () => {
    const acme: Company = {
      ...company('Acme', '2020-01-01T00:00:00Z'),
      description: '<p>Hi</p>',
      endDate: '2022-06-01T00:00:00Z',
      website: null,
    }

    expect(companyToForm(acme)).toEqual({
      name: 'Acme',
      description: '<p>Hi</p>',
      website: '',
      startDate: '2020-01-01',
      endDate: '2022-06-01',
    })
  })
})
