import type { NuxtApp } from '#app'
import type { Pinia } from 'pinia'
import { useLazyAsyncData } from '#imports'
import { useCompanyStore } from '~/store/CompanyStore'

const COMPANIES_KEY = 'companies'

async function loadCompanies(nuxtApp: NuxtApp): Promise<boolean> {
  const companyStore = useCompanyStore(nuxtApp.$pinia as Pinia)
  if (!companyStore.companiesForm.data) await companyStore.getCompanies()
  return true
}

export default function useCompaniesData() {
  return useLazyAsyncData(COMPANIES_KEY, loadCompanies)
}
