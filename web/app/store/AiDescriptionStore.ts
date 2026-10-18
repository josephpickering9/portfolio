import { defineStore } from 'pinia'
import { ref } from 'vue'
import { postVertexProjectsByIdDescriptionSuggest } from '@api'
import type { ProjectDescriptionSuggestionResponse, SuggestProjectDescriptionRequest } from '@api'
import { asyncForm, tryCatchFinally, unwrap } from '~/utils/async-helper'

export const useAiDescriptionStore = defineStore('aiDescriptionStore', {
  state: () => ({
    suggestionForm: asyncForm<ProjectDescriptionSuggestionResponse>(),
  }),
  getters: {
    suggesting: (state) => Boolean(state.suggestionForm.loading),
    suggestionError: (state) => state.suggestionForm.error,
    suggestedDescription: (state) => state.suggestionForm.data?.suggestedDescription,
  },
  actions: {
    async suggestDescription(projectId: string, request: SuggestProjectDescriptionRequest): Promise<string | undefined> {
      if (this.suggestionForm.loading) return undefined

      const suggestion = await tryCatchFinally(ref(this.suggestionForm), async () =>
        unwrap(await postVertexProjectsByIdDescriptionSuggest({ path: { id: projectId }, body: request })),
      )
      return suggestion?.suggestedDescription
    },
  },
})
