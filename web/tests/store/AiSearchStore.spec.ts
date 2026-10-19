import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { VertexFeatureType } from '@api'
import type { Project, VertexCitation, VertexQueryResult } from '@api'
import { ERROR_MESSAGES, ErrorCodes } from '~/types/errors'
import { deferred, fail, ok } from '../helpers/api-responses'

const api = vi.hoisted(() => ({ postVertexQuery: vi.fn(), getProjectById: vi.fn() }))

vi.mock('@api', async (importOriginal) => ({ ...(await importOriginal<object>()), ...api }))

const { useAiSearchStore } = await import('~/store/AiSearchStore')

const project = (id: string): Project => ({ id, slug: id, title: id }) as Project
const citation = (id: string, inline?: Project): VertexCitation => ({ id, featureType: VertexFeatureType.PROJECT, project: inline })

describe('AiSearchStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.resetAllMocks()
  })

  it('is pending while the query runs, then exposes the answer and cited projects', async () => {
    const store = useAiSearchStore()
    api.postVertexQuery.mockResolvedValue(ok({ answer: 'Joe uses Vue.', citations: [citation('a', project('a'))] }))

    const pending = store.query('Vue?')
    expect(store.queryPending).toBe(true)
    await pending

    expect(store.queryPending).toBe(false)
    expect(store.answer).toBe('Joe uses Vue.')
    expect(store.citedProjects.map((item) => item.id)).toEqual(['a'])
    expect(api.postVertexQuery).toHaveBeenCalledWith({ body: { query: 'Vue?' } })
  })

  it('fetches cited projects that are not inline, dedupes by slug and skips ones that fail', async () => {
    const store = useAiSearchStore()
    const citations = [
      citation('a', project('a')),
      citation('a', project('a')),
      citation('b'),
      citation('gone'),
      { id: 'c', featureType: VertexFeatureType.COMPANY },
    ]
    api.postVertexQuery.mockResolvedValue(ok({ answer: 'Answer', citations }))
    api.getProjectById.mockImplementation(({ path }: { path: { id: string } }) =>
      Promise.resolve(path.id === 'b' ? ok(project('b')) : fail(ErrorCodes.ProjectNotFound, 404)),
    )

    await store.query('Anything?')

    expect(store.citedProjects.map((item) => item.id)).toEqual(['a', 'b'])
    expect(api.getProjectById).toHaveBeenCalledTimes(2)
    expect(store.citedProjectsLoading).toBe(false)
  })

  it('records the mapped error when the query fails', async () => {
    const store = useAiSearchStore()
    api.postVertexQuery.mockResolvedValue(fail(ErrorCodes.UpstreamUnavailable, 503))

    await store.query('Vue?')

    expect(store.queryPending).toBe(false)
    expect(store.queryError).toBe(ERROR_MESSAGES[ErrorCodes.UpstreamUnavailable])
    expect(store.answer).toBe('')
  })

  it('ignores the response of a query that was superseded', async () => {
    const store = useAiSearchStore()
    const first = deferred<ReturnType<typeof ok<VertexQueryResult>>>()
    api.postVertexQuery.mockReturnValueOnce(first.promise).mockResolvedValueOnce(ok({ answer: 'Second', citations: [] }))

    const firstQuery = store.query('first')
    await store.query('second')
    first.resolve(ok({ answer: 'First', citations: [] }))
    await firstQuery

    expect(store.answer).toBe('Second')
  })

  it('reset clears the answer and drops any in-flight query', async () => {
    const store = useAiSearchStore()
    const pending = deferred<ReturnType<typeof ok<VertexQueryResult>>>()
    api.postVertexQuery.mockReturnValueOnce(pending.promise)

    const query = store.query('first')
    store.reset()
    pending.resolve(ok({ answer: 'Late', citations: [] }))
    await query

    expect(store.answer).toBe('')
    expect(store.citedProjects).toEqual([])
  })
})
