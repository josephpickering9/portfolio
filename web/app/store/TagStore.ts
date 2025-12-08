import { defineStore } from 'pinia'
import { ref } from 'vue'
import { TagService } from '@api/services/TagService'
import { asyncForm, tryCatchFinally } from '~/utils/async-helper'
import type { Tag } from '@api/models/Tag'
import type { CreateTag } from '@api/models/CreateTag'

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
    async getTags(search?: string): Promise<void> {
      if (this.tagsForm.loading) return

      await tryCatchFinally(ref(this.tagsForm), () => TagService.getTag(search))
    },
    async getTag(id: string): Promise<void> {
      if (!id || this.tagForm.loading) return

      await tryCatchFinally(ref(this.tagForm), () => TagService.getTag1(id))
    },
    async getTagBySlug(slug: string): Promise<void> {
      if (!slug || this.tagForm.loading) return

      await tryCatchFinally(ref(this.tagForm), () => TagService.getTag2(slug))
    },
    async createTag(tag: CreateTag): Promise<Tag | undefined> {
      if (!tag || this.tagCreateForm.loading) return

      return await tryCatchFinally(ref(this.tagCreateForm), () => TagService.postTag(tag))
    },
    async updateTag(id: string, tag: CreateTag): Promise<Tag | undefined> {
      if (!tag || this.tagCreateForm.loading) return

      return await tryCatchFinally(ref(this.tagCreateForm), () => TagService.putTag(id, tag))
    },
    async deleteTag(id: string): Promise<void> {
      if (!id || this.tagCreateForm.loading) return

      await tryCatchFinally(ref(this.tagCreateForm), () => TagService.deleteTag(id))
    },
  },
})
