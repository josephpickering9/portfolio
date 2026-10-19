import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import type { Company, CreateCompany } from '@api'
import { ERROR_MESSAGES, ErrorCodes } from '~/types/errors'
import { fail, ok } from '../helpers/api-responses'

const api = vi.hoisted(() => ({
  getCompany: vi.fn(),
  getCompanyById: vi.fn(),
  postCompany: vi.fn(),
  putCompanyById: vi.fn(),
  deleteCompanyById: vi.fn(),
}))

vi.mock('@api', async (importOriginal) => ({ ...(await importOriginal<object>()), ...api }))

const { useCompanyStore } = await import('~/store/CompanyStore')

const company = (id: string): Company => ({ id, name: id, slug: id, description: '' })
const payload: CreateCompany = { name: 'Acme', description: '' }

describe('CompanyStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.resetAllMocks()
  })

  describe('getCompanies', () => {
    it('sets loading, then stores the companies', async () => {
      const store = useCompanyStore()
      api.getCompany.mockResolvedValue(ok([company('acme')]))

      const pending = store.getCompanies()
      expect(store.companiesLoading).toBe(true)
      expect(await pending).toHaveLength(1)

      expect(store.companies.map((item) => item.id)).toEqual(['acme'])
      expect(store.companiesLoading).toBe(false)
      expect(store.companiesPending).toBe(false)
    })

    it('records the mapped error on failure', async () => {
      const store = useCompanyStore()
      api.getCompany.mockResolvedValue(fail(ErrorCodes.InternalError, 500))

      expect(await store.getCompanies()).toEqual([])
      expect(store.companiesError).toBe(ERROR_MESSAGES[ErrorCodes.InternalError])
      expect(store.companiesPending).toBe(false)
    })
  })

  describe('ensureCompanies', () => {
    it('only loads when nothing is cached', async () => {
      const store = useCompanyStore()
      api.getCompany.mockResolvedValue(ok([company('acme')]))

      await store.ensureCompanies()
      await store.ensureCompanies()

      expect(api.getCompany).toHaveBeenCalledTimes(1)
    })
  })

  describe('getCompany', () => {
    it('stores the loaded company', async () => {
      const store = useCompanyStore()
      api.getCompanyById.mockResolvedValue(ok(company('acme')))

      const pending = store.getCompany('acme')
      expect(store.companyLoading).toBe(true)
      await pending

      expect(store.company?.id).toBe('acme')
      expect(store.companyLoading).toBe(false)
    })

    it('records a not-found error', async () => {
      const store = useCompanyStore()
      api.getCompanyById.mockResolvedValue(fail(ErrorCodes.CompanyNotFound, 404))

      expect(await store.getCompany('missing')).toBeUndefined()
      expect(store.companyError).toBe(ERROR_MESSAGES[ErrorCodes.CompanyNotFound])
    })

    it('does nothing without an id', async () => {
      const store = useCompanyStore()

      expect(await store.getCompany('')).toBeUndefined()
      expect(api.getCompanyById).not.toHaveBeenCalled()
    })
  })

  describe('createCompany', () => {
    it('returns the created company and clears saving', async () => {
      const store = useCompanyStore()
      api.postCompany.mockResolvedValue(ok(company('acme')))

      const pending = store.createCompany(payload)
      expect(store.companySaving).toBe(true)

      expect((await pending)?.id).toBe('acme')
      expect(store.companySaving).toBe(false)
      expect(api.postCompany).toHaveBeenCalledWith(expect.objectContaining({ body: { createCompany: payload } }))
    })

    it('records a save error', async () => {
      const store = useCompanyStore()
      api.postCompany.mockResolvedValue(fail(ErrorCodes.BadRequest, 400))

      expect(await store.createCompany(payload)).toBeUndefined()
      expect(store.companySaveError).toBe(ERROR_MESSAGES[ErrorCodes.BadRequest])
    })
  })

  describe('updateCompany', () => {
    it('returns the updated company', async () => {
      const store = useCompanyStore()
      api.putCompanyById.mockResolvedValue(ok(company('acme')))

      expect((await store.updateCompany('acme', payload))?.id).toBe('acme')
      expect(api.putCompanyById).toHaveBeenCalledWith(expect.objectContaining({ path: { id: 'acme' } }))
    })

    it('records a conflict error', async () => {
      const store = useCompanyStore()
      api.putCompanyById.mockResolvedValue(fail(ErrorCodes.Conflict, 409))

      expect(await store.updateCompany('acme', payload)).toBeUndefined()
      expect(store.companySaveError).toBe(ERROR_MESSAGES[ErrorCodes.Conflict])
    })
  })

  describe('deleteCompany', () => {
    it('removes the company from the cache', async () => {
      const store = useCompanyStore()
      store.companiesForm.data = [company('acme'), company('other')]
      store.companyForm.data = company('acme')
      api.deleteCompanyById.mockResolvedValue(ok({}))

      const pending = store.deleteCompany('acme')
      expect(store.companyDeleting).toBe(true)
      await pending

      expect(store.companies.map((item) => item.id)).toEqual(['other'])
      expect(store.company).toBeUndefined()
      expect(store.companyDeleting).toBe(false)
    })

    it('keeps the cache and records the error when the delete fails', async () => {
      const store = useCompanyStore()
      store.companiesForm.data = [company('acme')]
      api.deleteCompanyById.mockResolvedValue(fail(ErrorCodes.Forbidden, 403))

      await store.deleteCompany('acme')

      expect(store.companies).toHaveLength(1)
      expect(store.companyDeleteError).toBe(ERROR_MESSAGES[ErrorCodes.Forbidden])
    })
  })
})
