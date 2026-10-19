import { defineStore } from 'pinia'
import { ref } from 'vue'
import { putProjectimageOptimise } from '@api'
import { asyncForm, tryCatchFinally, unwrap } from '~/utils/async-helper'

export const useProjectImageStore = defineStore('projectImageStore', {
  state: () => ({
    optimiseForm: asyncForm<unknown>(),
  }),
  getters: {
    optimising: (state) => state.optimiseForm.loading,
    optimiseError: (state) => state.optimiseForm.error,
  },
  actions: {
    async optimiseImages(): Promise<void> {
      if (this.optimiseForm.loading) return

      await tryCatchFinally(ref(this.optimiseForm), async () => unwrap(await putProjectimageOptimise()))
    },
  },
})
