import { defineStore } from 'pinia'
import { toRef } from 'vue'
import { uniqBy } from 'lodash-es'
import { getProjectById, postVertexQuery, VertexFeatureType } from '@api'
import type { Project, VertexCitation, VertexQueryResult } from '@api'
import { asyncForm, isAwaitingData, tryCatchFinallyIfLatest, unwrap } from '~/utils/async-helper'

type ProjectCitation = VertexCitation & { id: string }

function isProjectCitation(citation: VertexCitation): citation is ProjectCitation {
  return citation.featureType === VertexFeatureType.PROJECT && Boolean(citation.id)
}

async function resolveCitedProject(citation: ProjectCitation): Promise<Project> {
  if (citation.project) return citation.project
  return unwrap(await getProjectById({ path: { id: citation.id } }))
}

export const useAiSearchStore = defineStore('aiSearchStore', {
  state: () => ({
    queryForm: asyncForm<VertexQueryResult>(),
    citedProjectsForm: asyncForm<Project[]>(),
    latestQueryRequest: 0,
  }),
  getters: {
    answer: (state) => state.queryForm.data?.answer ?? '',
    queryPending: (state) => isAwaitingData(state.queryForm),
    queryError: (state) => state.queryForm.error,
    citedProjects: (state) => uniqBy(state.citedProjectsForm.data ?? [], (project) => project.slug),
    citedProjectsLoading: (state) => Boolean(state.citedProjectsForm.loading),
  },
  actions: {
    async query(question: string): Promise<void> {
      const request = this.reset()
      const result = await tryCatchFinallyIfLatest(
        toRef(this, 'queryForm'),
        () => request === this.latestQueryRequest,
        async () => unwrap(await postVertexQuery({ body: { query: question } })),
      )
      if (result) await this.getCitedProjects(result.citations ?? [], request)
    },
    async getCitedProjects(citations: VertexCitation[], request: number): Promise<void> {
      const projectCitations = citations.filter(isProjectCitation)
      await tryCatchFinallyIfLatest(
        toRef(this, 'citedProjectsForm'),
        () => request === this.latestQueryRequest,
        async () => {
          const results = await Promise.allSettled(projectCitations.map(resolveCitedProject))
          return results.flatMap((result) => (result.status === 'fulfilled' ? [result.value] : []))
        },
      )
    },
    reset(): number {
      this.queryForm = asyncForm()
      this.citedProjectsForm = asyncForm()
      return ++this.latestQueryRequest
    },
  },
})
