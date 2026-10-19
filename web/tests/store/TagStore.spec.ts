import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { TagType } from '@api'
import type { CreateTag, Tag } from '@api'
import { ERROR_MESSAGES, ErrorCodes } from '~/types/errors'
import { fail, ok } from '../helpers/api-responses'

const api = vi.hoisted(() => ({
  getTag: vi.fn(),
  getTagById: vi.fn(),
  postTag: vi.fn(),
  putTagById: vi.fn(),
  deleteTagById: vi.fn(),
}))

vi.mock('@api', async (importOriginal) => ({ ...(await importOriginal<object>()), ...api }))

const { useTagStore } = await import('~/store/TagStore')

const tag = (id: string): Tag => ({ id, title: id, slug: id, type: TagType.FRONTEND })
const payload: CreateTag = { title: 'Vue', type: TagType.FRONTEND }

describe('TagStore', () => {
  beforeEach(() => {
    setActivePinia(createPinia())
    vi.resetAllMocks()
  })

  describe('getTags', () => {
    it('sets loading, then stores the tags', async () => {
      const store = useTagStore()
      api.getTag.mockResolvedValue(ok([tag('vue')]))

      const pending = store.getTags()
      expect(store.tagsLoading).toBe(true)
      await pending

      expect(store.tags.map((item) => item.id)).toEqual(['vue'])
      expect(store.tagsLoading).toBe(false)
    })

    it('records the mapped error on failure', async () => {
      const store = useTagStore()
      api.getTag.mockResolvedValue(fail(ErrorCodes.Unauthorized, 401))

      expect(await store.getTags()).toEqual([])
      expect(store.tagsError).toBe(ERROR_MESSAGES[ErrorCodes.Unauthorized])
    })
  })

  describe('ensureTags', () => {
    it('only loads when nothing is cached', async () => {
      const store = useTagStore()
      api.getTag.mockResolvedValue(ok([tag('vue')]))

      await store.ensureTags()
      await store.ensureTags()

      expect(api.getTag).toHaveBeenCalledTimes(1)
    })
  })

  describe('getTag', () => {
    it('stores the loaded tag', async () => {
      const store = useTagStore()
      api.getTagById.mockResolvedValue(ok(tag('vue')))

      await store.getTag('vue')

      expect(store.tag?.id).toBe('vue')
      expect(store.tagLoading).toBe(false)
    })

    it('records a not-found error', async () => {
      const store = useTagStore()
      api.getTagById.mockResolvedValue(fail(ErrorCodes.TagNotFound, 404))

      expect(await store.getTag('missing')).toBeUndefined()
      expect(store.tagError).toBe(ERROR_MESSAGES[ErrorCodes.TagNotFound])
    })
  })

  describe('createTag', () => {
    it('returns the created tag and clears saving', async () => {
      const store = useTagStore()
      api.postTag.mockResolvedValue(ok(tag('vue')))

      const pending = store.createTag(payload)
      expect(store.tagSaving).toBe(true)

      expect((await pending)?.id).toBe('vue')
      expect(store.tagSaving).toBe(false)
    })

    it('records a conflict error', async () => {
      const store = useTagStore()
      api.postTag.mockResolvedValue(fail(ErrorCodes.Conflict, 409))

      expect(await store.createTag(payload)).toBeUndefined()
      expect(store.tagSaveError).toBe(ERROR_MESSAGES[ErrorCodes.Conflict])
    })
  })

  describe('updateTag', () => {
    it('returns the updated tag', async () => {
      const store = useTagStore()
      api.putTagById.mockResolvedValue(ok(tag('vue')))

      expect((await store.updateTag('vue', payload))?.id).toBe('vue')
      expect(api.putTagById).toHaveBeenCalledWith({ path: { id: 'vue' }, body: payload })
    })

    it('records a validation error', async () => {
      const store = useTagStore()
      api.putTagById.mockResolvedValue(fail(ErrorCodes.BadRequest, 400))

      expect(await store.updateTag('vue', payload)).toBeUndefined()
      expect(store.tagSaveError).toBe(ERROR_MESSAGES[ErrorCodes.BadRequest])
    })
  })

  describe('deleteTag', () => {
    it('removes the tag from the cache', async () => {
      const store = useTagStore()
      store.tagsForm.data = [tag('vue'), tag('react')]
      api.deleteTagById.mockResolvedValue(ok({}))

      const pending = store.deleteTag('vue')
      expect(store.tagDeleting).toBe(true)
      await pending

      expect(store.tags.map((item) => item.id)).toEqual(['react'])
      expect(store.tagDeleting).toBe(false)
    })

    it('keeps the cache and records the error when the delete fails', async () => {
      const store = useTagStore()
      store.tagsForm.data = [tag('vue')]
      api.deleteTagById.mockResolvedValue(fail(ErrorCodes.Forbidden, 403))

      await store.deleteTag('vue')

      expect(store.tags).toHaveLength(1)
      expect(store.tagDeleteError).toBe(ERROR_MESSAGES[ErrorCodes.Forbidden])
    })
  })
})
