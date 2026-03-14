import { defineStore } from 'pinia'
import { ref } from 'vue'
import { getCompany, getCompanyById, getCompanyBySlug, postCompany, putCompanyById, deleteCompanyById } from '@api'
import { asyncForm, tryCatchFinally } from '~/utils/async-helper'
import type { Company, CreateCompany } from '@api'
import { serializeToFormData } from '~/utils/form-data'

export const useCompanyStore = defineStore('companyStore', {
  state: () => ({
    companiesForm: asyncForm<Company[]>(),
    companyForm: asyncForm<Company>(),
    companyCreateForm: asyncForm<Company>(),
  }),
  getters: {
    companies: (state) => state.companiesForm.data ?? [],
    companiesLoading: (state) => state.companiesForm.loading,
    companiesError: (state) => state.companiesForm.error,
    company: (state) => state.companyForm.data,
    companyLoading: (state) => state.companyForm.loading,
    companyError: (state) => state.companyForm.error,
    companyCreating: (state) => state.companyCreateForm.loading,
    companyCreateError: (state) => state.companyCreateForm.error,
  },
  actions: {
    async getCompanies(search?: string): Promise<void> {
      if (this.companiesForm.loading) return

      await tryCatchFinally(ref(this.companiesForm), async () => {
        return (await getCompany({ query: { search } })).data
      })
    },
    async getCompany(id: string): Promise<void> {
      if (!id || this.companyForm.loading) return

      await tryCatchFinally(ref(this.companyForm), async () => {
        return (await getCompanyById({ path: { id } })).data
      })
    },
    async getCompanyBySlug(slug: string): Promise<void> {
      if (!slug || this.companyForm.loading) return

      await tryCatchFinally(ref(this.companyForm), async () => {
        return (await getCompanyBySlug({ path: { slug } })).data
      })
    },
    async createCompany(company: CreateCompany): Promise<Company | undefined> {
      if (!company || this.companyCreateForm.loading) return

      return await tryCatchFinally(ref(this.companyCreateForm), async () => {
        return (await postCompany({ body: company as any, bodySerializer: serializeToFormData })).data
      })
    },
    async updateCompany(id: string, company: CreateCompany): Promise<Company | undefined> {
      if (!company || this.companyCreateForm.loading) return

      return await tryCatchFinally(ref(this.companyCreateForm), async () => {
        return (await putCompanyById({ path: { id }, body: company as any, bodySerializer: serializeToFormData })).data
      })
    },
    async deleteCompany(id: string): Promise<void> {
      if (!id || this.companyCreateForm.loading) return

      await tryCatchFinally(ref(this.companyCreateForm), async () => {
        return (await deleteCompanyById({ path: { id } })).data
      })
    },
  },
})
