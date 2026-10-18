import { defineStore } from 'pinia'
import { ref } from 'vue'
import { putProjectimageOptimise } from '@api'
import { asyncForm, tryCatchFinally } from '~/utils/async-helper'

export const useProjectImageStore = defineStore('projectImageStore', {
  state: () => ({
    optimiseForm: asyncForm<any>(),
  }),
  getters: {
    optimising: (state) => state.optimiseForm.loading,
    optimiseError: (state) => state.optimiseForm.error,
  },
  actions: {
    async optimiseImages(): Promise<void> {
      if (this.optimiseForm.loading) return

      await tryCatchFinally(ref(this.optimiseForm), async () => {
        return (await putProjectimageOptimise()).data
      })
    },
  },
})
