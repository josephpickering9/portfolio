import { defineStore } from 'pinia'
import { ref } from 'vue'
import {
    postVertexProjectsByIdDescriptionSuggest,
    postVertexQuery,
    type PostVertexQueryResponse,
    type ProjectDescriptionSuggestionResponse,
    type VertexCitation,
    type Project
} from "~~/api"
import { asyncForm, tryCatchFinally } from '~/utils/async-helper'
import { useProjectStore } from './ProjectStore'

export const useAiStore = defineStore('aiStore', {
    state: () => ({
        aiForm: asyncForm<ProjectDescriptionSuggestionResponse>(),
        queryForm: asyncForm<PostVertexQueryResponse>(),
        citedProjectsForm: asyncForm<Project[]>(),
    }),
    getters: {
        citedProjects: (state) => state.citedProjectsForm.data ?? [],
        citedProjectsLoading: (state) => state.citedProjectsForm.loading,
    },
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
        },
        async getCitedProjects(citations: VertexCitation[]) {
            if (!citations?.length) {
                this.citedProjectsForm.data = []
                return
            }

            await tryCatchFinally(ref(this.citedProjectsForm), async () => {
                const projectStore = useProjectStore()
                const projectCitations = citations.filter(c => c.featureType === 'Project' && c.id)
                // Use Promise.all to fetch all projects in parallel using the project store
                const promises = projectCitations.map(c => projectStore.getProject(c.id!))
                const results = await Promise.all(promises)
                return results.filter((r): r is Project => !!r)
            })
        }
    }
})
