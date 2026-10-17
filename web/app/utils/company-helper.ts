import type { Company } from '@api'

export function sortCompaniesNewestFirst(companies: Company[]): Company[] {
  const undatedFirst = '9999'
  return [...companies].sort((a, b) => (b.startDate ?? undatedFirst).localeCompare(a.startDate ?? undatedFirst))
}

export default {
  sortCompaniesNewestFirst,
}
