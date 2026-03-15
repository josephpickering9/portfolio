import { defineStore } from 'pinia'
import { ref } from 'vue'
import { getProject, getProjectById, getProjectBySlug, postProject, putProjectById, deleteProjectById, getProjectByIdRelated } from '@api'
import { asyncForm, tryCatchFinally } from '~/utils/async-helper'
import type { Project, CreateProject } from '@api'
import { serializeToFormData } from '~/utils/form-data'

export const useProjectStore = defineStore('projectStore', {
  state: () => ({
    projectsForm: asyncForm<Project[]>(),
    projectForm: asyncForm<Project>(),
    projectCreateForm: asyncForm<Project>(),
    relatedProjectsForm: asyncForm<Project[]>(),
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
    relatedProjects: (state) => state.relatedProjectsForm.data ?? [],
    relatedProjectsLoading: (state) => state.relatedProjectsForm.loading,
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
      if (!slug || this.projectForm.loading) return

      return await tryCatchFinally(ref(this.projectForm), async () => {
        return (await getProjectBySlug({ path: { slug } })).data
      })
    },
    async getRelatedProjects(id: string): Promise<Project[]> {
      if (!id || this.relatedProjectsForm.loading) return []

      return await tryCatchFinally(ref(this.relatedProjectsForm), async () => {
        return (await getProjectByIdRelated({ path: { id } })).data
      }) || []
    },
    async createProject(project: CreateProject): Promise<Project | undefined> {
      if (!project || this.projectCreateForm.loading) return

      return await tryCatchFinally(ref(this.projectCreateForm), async () => {
        return (await postProject({
          body: { createProject: project },
          bodySerializer: serializeToFormData,
        })).data
      })
    },
    async updateProject(id: string, project: CreateProject): Promise<Project | undefined> {
      if (!project || this.projectCreateForm.loading) return

      return await tryCatchFinally(ref(this.projectCreateForm), async () => {
        return (await putProjectById({
          path: { id },
          body: { createProject: project },
          bodySerializer: serializeToFormData,
        })).data
      })
    },
    async deleteProject(id: string): Promise<undefined> {
      if (!id || this.projectCreateForm.loading) return

      await tryCatchFinally(ref(this.projectCreateForm), async () => {
        await deleteProjectById({ path: { id } })
      })
    },
  },
})
