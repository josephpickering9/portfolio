import type { Company, CreateCompany } from '@api'
import { toDateInputValue } from '~/utils/date-helper'

const UNDATED_SORT_KEY = '9999'

export function sortCompaniesNewestFirst(companies: Company[]): Company[] {
  return [...companies].sort((a, b) => (b.startDate ?? UNDATED_SORT_KEY).localeCompare(a.startDate ?? UNDATED_SORT_KEY))
}

export function companyToForm(company: Company): CreateCompany {
  return {
    name: company.name,
    description: company.description,
    website: company.website ?? '',
    startDate: toDateInputValue(company.startDate),
    endDate: toDateInputValue(company.endDate),
  }
}
