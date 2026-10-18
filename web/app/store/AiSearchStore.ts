import { defineStore } from 'pinia'
import { ref } from 'vue'
import { uniqBy } from 'lodash-es'
import { getProjectById, postVertexQuery, VertexFeatureType } from '@api'
import type { Project, VertexCitation, VertexQueryResult } from '@api'
import { asyncForm, isAwaitingData, tryCatchFinally, unwrap } from '~/utils/async-helper'

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
      this.queryForm.loading = true

      const form = ref(asyncForm<VertexQueryResult>())
      const result = await tryCatchFinally(form, async () => unwrap(await postVertexQuery({ body: { query: question } })))
      if (request !== this.latestQueryRequest) return

      this.queryForm = form.value
      if (result) await this.getCitedProjects(result.citations ?? [], request)
    },
    async getCitedProjects(citations: VertexCitation[], request: number): Promise<void> {
      const projectCitations = citations.filter(isProjectCitation)
      const form = ref(asyncForm<Project[]>())
      this.citedProjectsForm.loading = true

      await tryCatchFinally(form, async () => {
        const results = await Promise.allSettled(projectCitations.map(resolveCitedProject))
        return results.flatMap((result) => (result.status === 'fulfilled' ? [result.value] : []))
      })
      if (request === this.latestQueryRequest) this.citedProjectsForm = form.value
    },
    reset(): number {
      this.queryForm = asyncForm()
      this.citedProjectsForm = asyncForm()
      return ++this.latestQueryRequest
    },
  },
})
