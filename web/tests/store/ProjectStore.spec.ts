import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import type { CreateProject, Project } from '@api'
import { ERROR_MESSAGES, ErrorCodes } from '~/types/errors'
import { deferred, fail, ok } from '../helpers/api-responses'

const api = vi.hoisted(() => ({
  getProject: vi.fn(),
  getProjectBySlug: vi.fn(),
  postProject: vi.fn(),
  putProjectById: vi.fn(),
  deleteProjectById: vi.fn(),
}))

vi.mock('@api', async (importOriginal) => ({ ...(await importOriginal<object>()), ...api }))

const { useProjectStore } = await import('~/store/ProjectStore')

const project = (id: string, title = id): Project => ({ id, slug: id, title }) as Project
const payload = { title: 'Edited' } as CreateProject

describe('ProjectStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.resetAllMocks()
  })

  describe('getProjects', () => {
    it('sets loading, then stores the projects', async () => {
      const store = useProjectStore()
      api.getProject.mockResolvedValue(ok([project('a')]))

      const pending = store.getProjects()
      expect(store.projectsLoading).toBe(true)
      expect(store.projectsPending).toBe(true)
      const projects = await pending

      expect(projects.map((item) => item.id)).toEqual(['a'])
      expect(store.projectsLoading).toBe(false)
      expect(store.projectsPending).toBe(false)
      expect(store.projectsError).toBeUndefined()
    })

    it('records the mapped error when the API returns an error body', async () => {
      const store = useProjectStore()
      api.getProject.mockResolvedValue(fail(ErrorCodes.InternalError, 500))

      expect(await store.getProjects()).toEqual([])
      expect(store.projectsError).toBe(ERROR_MESSAGES[ErrorCodes.InternalError])
      expect(store.projectsLoading).toBe(false)
      expect(store.projectsPending).toBe(false)
    })

    it('ignores a second call while one is in flight', async () => {
      const store = useProjectStore()
      api.getProject.mockResolvedValue(ok([]))

      const first = store.getProjects()
      expect(await store.getProjects()).toEqual([])
      await first
      expect(api.getProject).toHaveBeenCalledTimes(1)
    })
  })

  describe('ensureProjects', () => {
    it('only loads when nothing is cached', async () => {
      const store = useProjectStore()
      api.getProject.mockResolvedValue(ok([project('a')]))

      await store.ensureProjects()
      await store.ensureProjects()

      expect(api.getProject).toHaveBeenCalledTimes(1)
    })
  })

  describe('getProjectBySlug', () => {
    it('stores the loaded project', async () => {
      const store = useProjectStore()
      api.getProjectBySlug.mockResolvedValue(ok(project('a')))

      const pending = store.getProjectBySlug('a')
      expect(store.projectLoading).toBe(true)
      await pending

      expect(store.project?.id).toBe('a')
      expect(store.projectLoading).toBe(false)
    })

    it('flags a not-found project by its error code', async () => {
      const store = useProjectStore()
      api.getProjectBySlug.mockResolvedValue(fail(ErrorCodes.ProjectNotFound, 404))

      expect(await store.getProjectBySlug('missing')).toBeUndefined()
      expect(store.projectNotFound).toBe(true)
      expect(store.projectError).toBe(ERROR_MESSAGES[ErrorCodes.ProjectNotFound])
    })

    it('does not flag other failures as not found', async () => {
      const store = useProjectStore()
      api.getProjectBySlug.mockResolvedValue(fail(ErrorCodes.InternalError, 500))

      await store.getProjectBySlug('a')
      expect(store.projectNotFound).toBe(false)
    })

    it('loads a second slug requested while the first is still loading, and ignores the stale response', async () => {
      const store = useProjectStore()
      const first = deferred<ReturnType<typeof ok<Project>>>()
      api.getProjectBySlug.mockReturnValueOnce(first.promise).mockResolvedValueOnce(ok(project('b', 'Second')))

      const firstLoad = store.getProjectBySlug('a')
      const secondLoad = await store.getProjectBySlug('b')
      first.resolve(ok(project('a', 'First')))
      await firstLoad

      expect(secondLoad?.id).toBe('b')
      expect(store.project?.id).toBe('b')
      expect(store.projectLoading).toBe(false)
    })

    it('returns undefined without calling the API for an empty slug', async () => {
      const store = useProjectStore()

      expect(await store.getProjectBySlug('')).toBeUndefined()
      expect(api.getProjectBySlug).not.toHaveBeenCalled()
    })
  })

  describe('clearProject', () => {
    it('clears the cached project only when the slug matches', () => {
      const store = useProjectStore()
      store.projectForm.data = project('a')

      store.clearProject('b')
      expect(store.project?.id).toBe('a')
      store.clearProject('a')
      expect(store.project).toBeUndefined()
    })
  })

  describe('createProject', () => {
    it('adds the created project to the cached list', async () => {
      const store = useProjectStore()
      store.projectsForm.data = [project('a')]
      api.postProject.mockResolvedValue(ok(project('b')))

      const pending = store.createProject(payload)
      expect(store.projectSaving).toBe(true)
      await pending

      expect(store.projects.map((item) => item.id)).toEqual(['a', 'b'])
      expect(store.projectSaving).toBe(false)
    })

    it('records a save error and leaves the cache alone', async () => {
      const store = useProjectStore()
      store.projectsForm.data = [project('a')]
      api.postProject.mockResolvedValue(fail(ErrorCodes.Conflict, 409))

      expect(await store.createProject(payload)).toBeUndefined()
      expect(store.projectSaveError).toBe(ERROR_MESSAGES[ErrorCodes.Conflict])
      expect(store.projects.map((item) => item.id)).toEqual(['a'])
    })
  })

  describe('updateProject', () => {
    it('writes the updated project back to the cached project and list', async () => {
      const store = useProjectStore()
      store.projectsForm.data = [project('a', 'Original'), project('b', 'Other')]
      store.projectForm.data = project('a', 'Original')
      api.putProjectById.mockResolvedValue(ok(project('a', 'Edited')))

      await store.updateProject('a', payload)

      expect(store.project?.title).toBe('Edited')
      expect(store.projects.map((item) => item.title)).toEqual(['Edited', 'Other'])
    })

    it('records a validation error', async () => {
      const store = useProjectStore()
      api.putProjectById.mockResolvedValue(fail(ErrorCodes.BadRequest, 400))

      expect(await store.updateProject('a', payload)).toBeUndefined()
      expect(store.projectSaveError).toBe(ERROR_MESSAGES[ErrorCodes.BadRequest])
    })
  })

  describe('deleteProject', () => {
    it('removes the deleted project from the cached project and list', async () => {
      const store = useProjectStore()
      store.projectsForm.data = [project('a'), project('b')]
      store.projectForm.data = project('a')
      api.deleteProjectById.mockResolvedValue(ok({}))

      const pending = store.deleteProject('a')
      expect(store.projectDeleting).toBe(true)
      await pending

      expect(store.projects.map((item) => item.id)).toEqual(['b'])
      expect(store.project).toBeUndefined()
      expect(store.projectDeleting).toBe(false)
    })

    it('keeps the cache when the API returns an error', async () => {
      const store = useProjectStore()
      store.projectsForm.data = [project('a')]
      api.deleteProjectById.mockResolvedValue(fail(ErrorCodes.Forbidden, 403))

      await store.deleteProject('a')

      expect(store.projects.map((item) => item.id)).toEqual(['a'])
      expect(store.projectDeleteError).toBe(ERROR_MESSAGES[ErrorCodes.Forbidden])
    })
  })
})
