import { defineStore } from 'pinia'
import { ref } from 'vue'
import { postVertexProjectsByIdDescriptionSuggest, postVertexQuery, type ProjectDescriptionSuggestionResponse } from "~~/api"
import { asyncForm, tryCatchFinally } from '~/utils/async-helper'

export const useAiStore = defineStore('aiStore', {
    state: () => ({
        aiForm: asyncForm<ProjectDescriptionSuggestionResponse>(),
        queryForm: asyncForm<unknown>(),
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
        },
        async query(query: string) {
            await tryCatchFinally(ref(this.queryForm), async () => {
                return (await postVertexQuery({
                    body: {
                        query
                    }
                })).data
            })
        }
    }
})
