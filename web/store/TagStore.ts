import { defineStore } from 'pinia'
import { TagService } from '../api/services/TagService'
import { extractError } from '../utils/error-helper'
import type { Tag } from '../api/models/Tag'
import type { CreateTag } from '../api/models/CreateTag'

export const useTagStore = defineStore('tagStore', {
  state: () => ({
    tags: [] as Tag[],
    tagsLoading: false,
    tagsError: undefined as string | undefined,
    tag: undefined as Tag | undefined,
    tagLoading: false,
    tagError: undefined as string | undefined,
    tagCreating: false,
    tagCreateError: undefined as string | undefined,
  }),
  actions: {
    async getTags(search?: string = null): Promise<void> {
      if (this.tagsLoading) return

      try {
        this.tagsError = undefined
        this.tagsLoading = true

        this.tags = await TagService.getTag(search)
      } catch (error) {
        this.tagsError = extractError(error)
      } finally {
        this.tagsLoading = false
      }
    },
    async getTag(id: number): Promise<void> {
      if (!id || this.tagLoading) return

      try {
        this.tagError = undefined
        this.tagLoading = true

        this.tag = await TagService.getTagId(id)
      } catch (error) {
        this.tagError = extractError(error)
      } finally {
        this.tagLoading = false
      }
    },
    async createTag(tag: CreateTag): Promise<Tag | undefined> {
      if (!tag || this.tagCreating) return

      let response: Tag | undefined

      try {
        this.tagCreateError = undefined
        this.tagCreating = true

        response = await TagService.postTag(tag)
      } catch (error) {
        this.tagCreateError = extractError(error)
      } finally {
        this.tagCreating = false
      }

      return response
    },
    async updateTag(id: number, tag: CreateTag): Promise<Tag | undefined> {
      if (!tag || this.tagCreating) return

      let response: Tag | undefined

      try {
        this.tagCreateError = undefined
        this.tagCreating = true

        response = await TagService.putTag(id, tag)
      } catch (error) {
        this.tagCreateError = extractError(error)
      } finally {
        this.tagCreating = false
      }

      return response
    },
    async deleteTag(id: number): Promise<void> {
      if (!id || this.tagCreating) return

      try {
        this.tagCreateError = undefined
        this.tagCreating = true

        await TagService.deleteTagId(id)
      } catch (error) {
        this.tagCreateError = extractError(error)
      } finally {
        this.tagCreating = false
      }
    },
  },
})
