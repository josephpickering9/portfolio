import { defineStore } from 'pinia'
import { ref } from 'vue'
import { deleteProjectById, getProject, getProjectBySlug, postProject, putProjectById } from '@api'
import type { CreateProject, Project } from '@api'
import { asyncForm, isAwaitingData, tryCatchFinally, unwrap } from '~/utils/async-helper'
import { isNotFoundCode } from '~/utils/error-helper'
import { serializeToFormData } from '~/utils/form-data'

export const useProjectStore = defineStore('projectStore', {
  state: () => ({
    projectsForm: asyncForm<Project[]>(),
    projectForm: asyncForm<Project>(),
    projectSaveForm: asyncForm<Project>(),
    projectDeleteForm: asyncForm<unknown>(),
    latestProjectRequest: 0,
  }),
  getters: {
    projects: (state) => state.projectsForm.data ?? [],
    projectsLoading: (state) => state.projectsForm.loading,
    projectsPending: (state) => isAwaitingData(state.projectsForm),
    projectsError: (state) => state.projectsForm.error,
    project: (state) => state.projectForm.data,
    projectLoading: (state) => state.projectForm.loading,
    projectError: (state) => state.projectForm.error,
    projectNotFound: (state) => isNotFoundCode(state.projectForm.errorCode),
    projectSaving: (state) => state.projectSaveForm.loading,
    projectSaveError: (state) => state.projectSaveForm.error,
    projectDeleting: (state) => state.projectDeleteForm.loading,
    projectDeleteError: (state) => state.projectDeleteForm.error,
  },
  actions: {
    async getProjects(search?: string): Promise<Project[]> {
      if (this.projectsForm.loading) return []

      const projects = await tryCatchFinally(ref(this.projectsForm), async () =>
        unwrap(await getProject({ query: { search } })),
      )
      return projects ?? []
    },
    async ensureProjects(): Promise<void> {
      if (!this.projectsForm.data) await this.getProjects()
    },
    async getProjectBySlug(slug: string): Promise<Project | undefined> {
      if (!slug) return undefined

      const request = ++this.latestProjectRequest
      const form = ref({ ...asyncForm<Project>(), data: this.projectForm.data })
      this.projectForm.loading = true
      const loaded = await tryCatchFinally(form, async () => unwrap(await getProjectBySlug({ path: { slug } })))
      if (request !== this.latestProjectRequest) return undefined

      this.projectForm = form.value
      return loaded
    },
    clearProject(slug: string) {
      if (this.projectForm.data?.slug === slug) this.projectForm.data = undefined
    },
    async createProject(project: CreateProject): Promise<Project | undefined> {
      if (this.projectSaveForm.loading) return undefined

      const created = await tryCatchFinally(ref(this.projectSaveForm), async () =>
        unwrap(await postProject({ body: { createProject: project }, bodySerializer: serializeToFormData })),
      )
      if (created) this.cacheProject(created)
      return created
    },
    async updateProject(id: string, project: CreateProject): Promise<Project | undefined> {
      if (this.projectSaveForm.loading) return undefined

      const updated = await tryCatchFinally(ref(this.projectSaveForm), async () =>
        unwrap(await putProjectById({ path: { id }, body: { createProject: project }, bodySerializer: serializeToFormData })),
      )
      if (updated) this.cacheProject(updated)
      return updated
    },
    async deleteProject(id: string): Promise<void> {
      if (!id || this.projectDeleteForm.loading) return

      await tryCatchFinally(ref(this.projectDeleteForm), async () => unwrap(await deleteProjectById({ path: { id } })))
      if (this.projectDeleteForm.error) return

      if (this.projectsForm.data) this.projectsForm.data = this.projectsForm.data.filter((item) => item.id !== id)
      if (this.projectForm.data?.id === id) this.projectForm.data = undefined
    },
    cacheProject(project: Project) {
      this.projectForm.data = project
      const cached = this.projectsForm.data
      if (!cached) return

      const index = cached.findIndex((item) => item.id === project.id)
      this.projectsForm.data = index === -1 ? [...cached, project] : cached.map((item, i) => (i === index ? project : item))
    },
  },
})
