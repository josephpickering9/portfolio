import { defineStore } from 'pinia'
import { ref } from 'vue'
import { postVertexProjectsByIdDescriptionSuggest, type ProjectDescriptionSuggestionResponse } from "~~/api"
import { asyncForm, tryCatchFinally } from '~/utils/async-helper'

export const useAiStore = defineStore('aiStore', {
    state: () => ({
        aiForm: asyncForm<ProjectDescriptionSuggestionResponse>(),
    }),
    actions: {
        async getSuggestedDescription(
            projectId: string,
            additionalContext: string | null,
            additionalData: Record<string, string> | null,
            targetWordCount: number | null
        ) {
            await tryCatchFinally(ref(this.aiForm), async () => {
                return (await postVertexProjectsByIdDescriptionSuggest({
                    path: {
                        id: projectId
                    },
                    body: {
                        additionalContext,
                        additionalData,
                        targetWordCount,
                    }
                })).data
            })
        }
    }
})
