import { defineStore } from 'pinia'
import { ProjectService } from '../api/services/ProjectService'

export const useProjectStore = defineStore('projectStore', {
    state: () => ({
        projects: [] as Project[],
    }),
    actions: {
        async getProjects(search: string) {
            
            try {
                this.projects = await ProjectService.getProject(search)
            } catch (error) {
                console.log(error)
            } finally {
                console.log('finally')
            }
        },
    },
})
