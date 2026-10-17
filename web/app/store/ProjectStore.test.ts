import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import type { CreateProject, Project } from '@api'

const api = vi.hoisted(() => ({
  getProject: vi.fn(),
  getProjectById: vi.fn(),
  getProjectBySlug: vi.fn(),
  postProject: vi.fn(),
  putProjectById: vi.fn(),
  deleteProjectById: vi.fn(),
}))

vi.mock('@api', () => api)

const { useProjectStore } = await import('./ProjectStore')

const project = (id: string, title: string): Project => ({ id, slug: id, title }) as Project
const payload = { title: 'Edited' } as CreateProject

function deferred<T>() {
  let resolve!: (value: T) => void
  const promise = new Promise<T>((res) => (resolve = res))
  return { promise, resolve }
}

describe('ProjectStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.resetAllMocks()
  })

  it('writes an updated project back to the cached project and list', async () => {
    const store = useProjectStore()
    store.projectsForm.data = [project('a', 'Original'), project('b', 'Other')]
    store.projectForm.data = project('a', 'Original')
    api.putProjectById.mockResolvedValue({ data: project('a', 'Edited') })

    await store.updateProject('a', payload)

    expect(store.project?.title).toBe('Edited')
    expect(store.projects.map((item) => item.title)).toEqual(['Edited', 'Other'])
  })

  it('adds a created project to the cached list', async () => {
    const store = useProjectStore()
    store.projectsForm.data = [project('a', 'Existing')]
    api.postProject.mockResolvedValue({ data: project('b', 'New') })

    await store.createProject(payload)

    expect(store.projects.map((item) => item.id)).toEqual(['a', 'b'])
  })

  it('removes a deleted project from the cached project and list', async () => {
    const store = useProjectStore()
    store.projectsForm.data = [project('a', 'Gone'), project('b', 'Kept')]
    store.projectForm.data = project('a', 'Gone')
    api.deleteProjectById.mockResolvedValue({})

    await store.deleteProject('a')

    expect(store.projects.map((item) => item.id)).toEqual(['b'])
    expect(store.project).toBeUndefined()
  })

  it('keeps the cache when a delete fails', async () => {
    const store = useProjectStore()
    store.projectsForm.data = [project('a', 'Kept')]
    api.deleteProjectById.mockRejectedValue(new Error('boom'))

    await store.deleteProject('a')

    expect(store.projects.map((item) => item.id)).toEqual(['a'])
    expect(store.projectCreateError).toBeTruthy()
  })

  it('loads a second slug requested while the first is still loading, and ignores the stale response', async () => {
    const store = useProjectStore()
    const first = deferred<{ data: Project }>()
    api.getProjectBySlug.mockReturnValueOnce(first.promise).mockResolvedValueOnce({ data: project('b', 'Second') })

    const firstLoad = store.getProjectBySlug('a')
    const secondLoad = await store.getProjectBySlug('b')
    first.resolve({ data: project('a', 'First') })
    await firstLoad

    expect(secondLoad?.id).toBe('b')
    expect(store.project?.id).toBe('b')
    expect(store.projectLoading).toBe(false)
  })
})
