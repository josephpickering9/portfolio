import { defineStore } from 'pinia'
import { ref } from 'vue'
import { getTag, getTagById, getTagBySlug, postTag, putTagById, deleteTagById } from '@api'
import { asyncForm, tryCatchFinally } from '~/utils/async-helper'
import type { Tag, CreateTag } from '@api'

export const useTagStore = defineStore('tagStore', {
  state: () => ({
    tagsForm: asyncForm<Tag[]>(),
    tagForm: asyncForm<Tag>(),
    tagCreateForm: asyncForm<Tag>(),
  }),
  getters: {
    tags: (state) => state.tagsForm.data ?? [],
    tagsLoading: (state) => state.tagsForm.loading,
    tagsError: (state) => state.tagsForm.error,
    tag: (state) => state.tagForm.data,
    tagLoading: (state) => state.tagForm.loading,
    tagError: (state) => state.tagForm.error,
    tagCreating: (state) => state.tagCreateForm.loading,
    tagCreateError: (state) => state.tagCreateForm.error,
  },
  actions: {
    async getTags(search?: string): Promise<Tag[]> {
      if (this.tagsForm.loading) return []

      return await tryCatchFinally(ref(this.tagsForm), async () => {
        return (await getTag({ query: { search } })).data
      }) ?? []
    },
    async getTag(id: string): Promise<Tag | undefined> {
      if (!id || this.tagForm.loading) return

      return await tryCatchFinally(ref(this.tagForm), async () => {
        return (await getTagById({ path: { id } })).data
      })
    },
    async getTagBySlug(slug: string): Promise<Tag | undefined> {
      if (!slug || this.tagForm.loading) return

      return await tryCatchFinally(ref(this.tagForm), async () => {
        return (await getTagBySlug({ path: { slug } })).data
      })
    },
    async createTag(tag: CreateTag): Promise<Tag | undefined> {
      if (!tag || this.tagCreateForm.loading) return

      return await tryCatchFinally(ref(this.tagCreateForm), async () => {
        return (await postTag({ body: tag })).data
      })
    },
    async updateTag(id: string, tag: CreateTag): Promise<Tag | undefined> {
      if (!tag || this.tagCreateForm.loading) return

      return await tryCatchFinally(ref(this.tagCreateForm), async () => {
        return (await putTagById({ path: { id }, body: tag })).data
      })
    },
    async deleteTag(id: string): Promise<void> {
      if (!id || this.tagCreateForm.loading) return

      await tryCatchFinally(ref(this.tagCreateForm), async () => {
        return (await deleteTagById({ path: { id } })).data
      })
    },
  },
})
