import { defineStore } from 'pinia'
import { ref } from 'vue'
import { getProject, getProjectById, getProjectBySlug, postProject, putProjectById, deleteProjectById } from '@api'
import { asyncForm, tryCatchFinally } from '~/utils/async-helper'
import type { Project, CreateProject } from '@api'
import { serializeToFormData } from '~/utils/form-data'

let latestProjectRequest = 0

export const useProjectStore = defineStore('projectStore', {
  state: () => ({
    projectsForm: asyncForm<Project[]>(),
    projectForm: asyncForm<Project>(),
    projectCreateForm: asyncForm<Project>(),
  }),
  getters: {
    projects: (state) => state.projectsForm.data ?? [],
    projectsLoading: (state) => state.projectsForm.loading,
    projectsError: (state) => state.projectsForm.error,
    project: (state) => state.projectForm.data,
    projectLoading: (state) => state.projectForm.loading,
    projectError: (state) => state.projectForm.error,
    projectCreating: (state) => state.projectCreateForm.loading,
    projectCreateError: (state) => state.projectCreateForm.error,
  },
  actions: {
    async getProjects(search?: string): Promise<Project[]> {
      if (this.projectsForm.loading) return []

      return await tryCatchFinally(ref(this.projectsForm), async () => {
        return (await getProject({ query: { search } })).data
      }) || []
    },
    async getProject(id: string): Promise<Project | undefined> {
      if (!id || this.projectForm.loading) return

      return await tryCatchFinally(ref(this.projectForm), async () => {
        return (await getProjectById({ path: { id } })).data
      })
    },
    async getProjectBySlug(slug: string): Promise<Project | undefined> {
      if (!slug) return

      const request = ++latestProjectRequest
      const form = ref({ ...asyncForm<Project>(), data: this.projectForm.data })
      this.projectForm.loading = true
      const loaded = await tryCatchFinally(form, async () => {
        return (await getProjectBySlug({ path: { slug } })).data
      })
      if (request !== latestProjectRequest) return

      this.projectForm = form.value
      return loaded
    },
    async createProject(project: CreateProject): Promise<Project | undefined> {
      if (!project || this.projectCreateForm.loading) return

      const created = await tryCatchFinally(ref(this.projectCreateForm), async () => {
        return (await postProject({
          body: { createProject: project },
          bodySerializer: serializeToFormData,
        })).data
      })
      if (created) this.cacheProject(created)
      return created
    },
    async updateProject(id: string, project: CreateProject): Promise<Project | undefined> {
      if (!project || this.projectCreateForm.loading) return

      const updated = await tryCatchFinally(ref(this.projectCreateForm), async () => {
        return (await putProjectById({
          path: { id },
          body: { createProject: project },
          bodySerializer: serializeToFormData,
        })).data
      })
      if (updated) this.cacheProject(updated)
      return updated
    },
    async deleteProject(id: string): Promise<undefined> {
      if (!id || this.projectCreateForm.loading) return

      await tryCatchFinally(ref(this.projectCreateForm), async () => {
        await deleteProjectById({ path: { id } })
        return undefined
      })
      if (this.projectCreateForm.error) return

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
