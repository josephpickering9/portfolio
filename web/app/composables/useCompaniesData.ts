import type { NuxtApp } from '#app'
import type { Pinia } from 'pinia'
import { useAsyncData } from '#imports'
import { useCompanyStore } from '~/store/CompanyStore'

const COMPANIES_KEY = 'companies'

async function loadCompanies(nuxtApp: NuxtApp): Promise<boolean> {
  await useCompanyStore(nuxtApp.$pinia as Pinia).ensureCompanies()
  return true
}

export default function useCompaniesData() {
  return useAsyncData(COMPANIES_KEY, loadCompanies, { lazy: true })
}
