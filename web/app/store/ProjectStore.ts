import { defineStore } from 'pinia'
import { ref } from 'vue'
import { ProjectService } from '@api/services/ProjectService'
import { asyncForm, tryCatchFinally } from '~/utils/async-helper'
import type { Project } from '@api/models/Project'
import type { CreateProject } from '@api/models/CreateProject'

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

      return await tryCatchFinally(ref(this.projectsForm), () => ProjectService.getProject(search)) || []
    },
    async getProject(id: number): Promise<void> {
      if (!id || this.projectForm.loading) return

      await tryCatchFinally(ref(this.projectForm), () => ProjectService.getProject1(id.toString()))
    },
    async getProjectBySlug(slug: string): Promise<void> {
      if (!slug || this.projectForm.loading) return

      await tryCatchFinally(ref(this.projectForm), () => ProjectService.getProject2(slug))
    },
    async getRelatedProjects(id: number): Promise<Project[]> {
      if (!id || this.relatedProjectsForm.loading) return []

      return await tryCatchFinally(ref(this.relatedProjectsForm), () => ProjectService.getProjectRelated(id.toString())) || []
    },
    async createProject(project: CreateProject): Promise<Project | undefined> {
      if (!project || this.projectCreateForm.loading) return

      return await tryCatchFinally(ref(this.projectCreateForm), () => ProjectService.postProject(project))
    },
    async updateProject(id: number, project: CreateProject): Promise<Project | undefined> {
      if (!project || this.projectCreateForm.loading) return

      return await tryCatchFinally(ref(this.projectCreateForm), () => ProjectService.putProject(id.toString(), project))
    },
    async deleteProject(id: number): Promise<void> {
      if (!id || this.projectCreateForm.loading) return

      await tryCatchFinally(ref(this.projectCreateForm), () => ProjectService.deleteProject(id.toString()))
    },
  },
})
