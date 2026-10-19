import { defineStore } from 'pinia'
import { ref } from 'vue'
import { deleteTagById, getTag, getTagById, postTag, putTagById } from '@api'
import type { CreateTag, Tag } from '@api'
import { asyncForm, tryCatchFinally, unwrap } from '~/utils/async-helper'

export const useTagStore = defineStore('tagStore', {
  state: () => ({
    tagsForm: asyncForm<Tag[]>(),
    tagForm: asyncForm<Tag>(),
    tagSaveForm: asyncForm<Tag>(),
    tagDeleteForm: asyncForm<unknown>(),
  }),
  getters: {
    tags: (state) => state.tagsForm.data ?? [],
    tagsLoading: (state) => state.tagsForm.loading,
    tagsError: (state) => state.tagsForm.error,
    tag: (state) => state.tagForm.data,
    tagLoading: (state) => state.tagForm.loading,
    tagError: (state) => state.tagForm.error,
    tagSaving: (state) => state.tagSaveForm.loading,
    tagSaveError: (state) => state.tagSaveForm.error,
    tagDeleting: (state) => state.tagDeleteForm.loading,
    tagDeleteError: (state) => state.tagDeleteForm.error,
  },
  actions: {
    async getTags(search?: string): Promise<Tag[]> {
      if (this.tagsForm.loading) return []

      const tags = await tryCatchFinally(ref(this.tagsForm), async () => unwrap(await getTag({ query: { search } })))
      return tags ?? []
    },
    async ensureTags(): Promise<void> {
      if (!this.tagsForm.data) await this.getTags()
    },
    async getTag(id: string): Promise<Tag | undefined> {
      if (!id || this.tagForm.loading) return undefined

      return await tryCatchFinally(ref(this.tagForm), async () => unwrap(await getTagById({ path: { id } })))
    },
    async createTag(tag: CreateTag): Promise<Tag | undefined> {
      if (this.tagSaveForm.loading) return undefined

      return await tryCatchFinally(ref(this.tagSaveForm), async () => unwrap(await postTag({ body: tag })))
    },
    async updateTag(id: string, tag: CreateTag): Promise<Tag | undefined> {
      if (this.tagSaveForm.loading) return undefined

      return await tryCatchFinally(ref(this.tagSaveForm), async () => unwrap(await putTagById({ path: { id }, body: tag })))
    },
    async deleteTag(id: string): Promise<void> {
      if (!id || this.tagDeleteForm.loading) return

      await tryCatchFinally(ref(this.tagDeleteForm), async () => unwrap(await deleteTagById({ path: { id } })))
      if (this.tagDeleteForm.error) return

      if (this.tagsForm.data) this.tagsForm.data = this.tagsForm.data.filter((item) => item.id !== id)
      if (this.tagForm.data?.id === id) this.tagForm.data = undefined
    },
  },
})
