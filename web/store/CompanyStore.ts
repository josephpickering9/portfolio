import { defineStore } from 'pinia'
import { CompanyService } from '../api/services/CompanyService'
import { extractError } from '../utils/error-helper'
import type { Company } from '../api/models/Company'
import type { CreateCompany } from '../api/models/CreateCompany'

export const useCompanyStore = defineStore('companyStore', {
  state: () => ({
    companies: [] as Company[],
    companiesLoading: false,
    companiesError: undefined as string | undefined,
    company: undefined as Company | undefined,
    companyLoading: false,
    companyError: undefined as string | undefined,
    companyCreating: false,
    companyCreateError: undefined as string | undefined,
  }),
  actions: {
    async getCompanies(search?: string): Promise<void> {
      if (this.companiesLoading) return

      try {
        this.companiesError = undefined
        this.companiesLoading = true

        this.companies = await CompanyService.getCompany(search)
      } catch (error) {
        this.companiesError = extractError(error)
      } finally {
        this.companiesLoading = false
      }
    },
    async getCompany(id: number): Promise<void> {
      if (!id || this.companyLoading) return

      try {
        this.companyError = undefined
        this.companyLoading = true

        this.company = await CompanyService.getCompany1(id)
      } catch (error) {
        this.companyError = extractError(error)
      } finally {
        this.companyLoading = false
      }
    },
    async createCompany(company: CreateCompany): Promise<Company | undefined> {
      if (!company || this.companyCreating) return

      let response: Company | undefined

      try {
        this.companyCreateError = undefined
        this.companyCreating = true

        response = await CompanyService.postCompany(company)
      } catch (error) {
        this.companyCreateError = extractError(error)
      } finally {
        this.companyCreating = false
      }

      return response
    },
    async updateCompany(id: number, company: CreateCompany): Promise<Company | undefined> {
      if (!company || this.companyCreating) return

      let response: Company | undefined

      try {
        this.companyCreateError = undefined
        this.companyCreating = true

        response = await CompanyService.putCompany(id, company)
      } catch (error) {
        this.companyCreateError = extractError(error)
      } finally {
        this.companyCreating = false
      }

      return response
    },
    async deleteCompany(id: number): Promise<void> {
      if (!id || this.companyCreating) return

      try {
        this.companyCreateError = undefined
        this.companyCreating = true

        await CompanyService.deleteCompany(id)
      } catch (error) {
        this.companyCreateError = extractError(error)
      } finally {
        this.companyCreating = false
      }
    },
  },
})
