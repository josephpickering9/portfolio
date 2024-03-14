import { defineStore } from 'pinia'
import { ProjectImageService } from '../api'

export const useProjectImageStore = defineStore('projectImageStore', {
  state: () => ({
    optimising: false,
    optimiseError: undefined as string | undefined,
  }),
  actions: {
    async optimiseImages(): Promise<void> {
      try {
        this.optimising = true
        this.optimiseError = undefined

        await ProjectImageService.putProjectimageOptimise()
      } catch (error) {
        this.optimiseError = extractError(error)
      } finally {
        this.optimising = false
      }
    },
  },
})
