import { defineStore } from 'pinia'
import { ref } from 'vue'
import { deleteCompanyById, getCompany, getCompanyById, postCompany, putCompanyById } from '@api'
import type { Company, CreateCompany } from '@api'
import { asyncForm, isAwaitingData, tryCatchFinally, unwrap } from '~/utils/async-helper'
import { serializeToFormData } from '~/utils/form-data'

export const useCompanyStore = defineStore('companyStore', {
  state: () => ({
    companiesForm: asyncForm<Company[]>(),
    companyForm: asyncForm<Company>(),
    companySaveForm: asyncForm<Company>(),
    companyDeleteForm: asyncForm<unknown>(),
  }),
  getters: {
    companies: (state) => state.companiesForm.data ?? [],
    companiesLoading: (state) => state.companiesForm.loading,
    companiesPending: (state) => isAwaitingData(state.companiesForm),
    companiesError: (state) => state.companiesForm.error,
    company: (state) => state.companyForm.data,
    companyLoading: (state) => state.companyForm.loading,
    companyError: (state) => state.companyForm.error,
    companySaving: (state) => state.companySaveForm.loading,
    companySaveError: (state) => state.companySaveForm.error,
    companyDeleting: (state) => state.companyDeleteForm.loading,
    companyDeleteError: (state) => state.companyDeleteForm.error,
  },
  actions: {
    async getCompanies(search?: string): Promise<Company[]> {
      if (this.companiesForm.loading) return []

      const companies = await tryCatchFinally(ref(this.companiesForm), async () =>
        unwrap(await getCompany({ query: { search } })),
      )
      return companies ?? []
    },
    async ensureCompanies(): Promise<void> {
      if (!this.companiesForm.data) await this.getCompanies()
    },
    async getCompany(id: string): Promise<Company | undefined> {
      if (!id || this.companyForm.loading) return undefined

      return await tryCatchFinally(ref(this.companyForm), async () => unwrap(await getCompanyById({ path: { id } })))
    },
    async createCompany(company: CreateCompany): Promise<Company | undefined> {
      if (this.companySaveForm.loading) return undefined

      return await tryCatchFinally(ref(this.companySaveForm), async () =>
        unwrap(await postCompany({ body: { createCompany: company }, bodySerializer: serializeToFormData })),
      )
    },
    async updateCompany(id: string, company: CreateCompany): Promise<Company | undefined> {
      if (this.companySaveForm.loading) return undefined

      return await tryCatchFinally(ref(this.companySaveForm), async () =>
        unwrap(await putCompanyById({ path: { id }, body: { createCompany: company }, bodySerializer: serializeToFormData })),
      )
    },
    async deleteCompany(id: string): Promise<void> {
      if (!id || this.companyDeleteForm.loading) return

      await tryCatchFinally(ref(this.companyDeleteForm), async () => unwrap(await deleteCompanyById({ path: { id } })))
      if (this.companyDeleteForm.error) return

      if (this.companiesForm.data) this.companiesForm.data = this.companiesForm.data.filter((item) => item.id !== id)
      if (this.companyForm.data?.id === id) this.companyForm.data = undefined
    },
  },
})
