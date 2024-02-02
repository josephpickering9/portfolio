import { defineStore } from 'pinia'
import { ProjectService } from '../api/services/ProjectService'
import { extractError } from '../utils/error-helper'
import type { Project } from '../api/models/Project'
import type { CreateProject } from '../api/models/CreateProject'

export const useProjectStore = defineStore('projectStore', {
  state: () => ({
    projects: [] as Project[],
    projectsLoading: false,
    projectsError: undefined as string | undefined,
    project: undefined as Project | undefined,
    projectLoading: false,
    projectError: undefined as string | undefined,
    projectCreating: false,
    projectCreateError: undefined as string | undefined,
  }),
  actions: {
    async getProjects(search?: string): Promise<void> {
      if (this.projectsLoading) return

      try {
        this.projectsError = undefined
        this.projectsLoading = true

        this.projects = await ProjectService.getProject(search)
      } catch (error) {
        this.projectsError = extractError(error)
      } finally {
        this.projectsLoading = false
      }
    },
    async getProject(id: number): Promise<void> {
      if (!id || this.projectLoading) return

      try {
        this.projectError = undefined
        this.projectLoading = true

        this.project = await ProjectService.getProject1(id)
      } catch (error) {
        this.projectError = extractError(error)
      } finally {
        this.projectLoading = false
      }
    },
    async createProject(project: CreateProject): Promise<Project | undefined> {
      if (!project || this.projectCreating) return

      let response: Project | undefined

      try {
        this.projectCreateError = undefined
        this.projectCreating = true

        response = await ProjectService.postProject(project)
      } catch (error) {
        this.projectCreateError = extractError(error)
      } finally {
        this.projectCreating = false
      }

      return response
    },
    async updateProject(id: number, project: CreateProject): Promise<Project | undefined> {
      if (!project || this.projectCreating) return

      let response: Project | undefined

      try {
        this.projectCreateError = undefined
        this.projectCreating = true

        response = await ProjectService.putProject(id, project)
      } catch (error) {
        this.projectCreateError = extractError(error)
      } finally {
        this.projectCreating = false
      }

      return response
    },
    async deleteProject(id: number): Promise<void> {
      if (!id || this.projectCreating) return

      try {
        this.projectCreateError = undefined
        this.projectCreating = true

        await ProjectService.de(id)
      } catch (error) {
        this.projectCreateError = extractError(error)
      } finally {
        this.projectCreating = false
      }
    },
  },
})
