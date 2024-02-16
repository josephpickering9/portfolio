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
    relatedProjects: [] as Project[],
    relatedProjectsLoading: false,
  }),
  actions: {
    async getProjects(search?: string): Promise<Project[]> {
      if (this.projectsLoading) return []

      let response: Project[] = []

      try {
        this.projectsError = undefined
        this.projectsLoading = true

        response = await ProjectService.getProject(search)
        this.projects = response
      } catch (error) {
        this.projectsError = extractError(error)
      } finally {
        this.projectsLoading = false
      }

      return response
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
    async getProjectBySlug(slug: string): Promise<void> {
      if (!slug || this.projectLoading) return

      try {
        this.projectError = undefined
        this.projectLoading = true

        this.project = await ProjectService.getProject2(slug)
      } catch (error) {
        this.projectError = extractError(error)
      } finally {
        this.projectLoading = false
      }
    },
    async getRelatedProjects(id: number): Promise<Project[]> {
      if (!id || this.relatedProjectsLoading) return []

      let response: Project[] = []

      try {
        this.relatedProjectsLoading = true

        response = await ProjectService.getProjectRelated(id)
        this.relatedProjects = response
      } catch {
      } finally {
        this.relatedProjectsLoading = false
      }

      return response
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
