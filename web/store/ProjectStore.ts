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
    projectCreating: false,
    projectCreateError: undefined as string | undefined,
  }),
  actions: {
    async getProjects(search: string) {
      if (!search || this.projectsLoading) return

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
  },
})
