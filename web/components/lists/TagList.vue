<template>
  <div class="tags prose w-full max-w-5xl">
    <div class="flex flex-col items-center justify-between gap-4 md:flex-row">
      <h1 class="m-0">Tags</h1>
      <div class="flex items-center gap-4">
        <TextInput v-model="search" class="w-full md:max-w-48" size="sm" placeholder="Search" :disabled="loading" />
        <TagTypeSelectList v-model="tagType" class="w-full md:max-w-48" size="sm" placeholder="Type" :disabled="loading" />
        <ClientOnly>
          <FormButton v-if="isAuthenticated" label="Add Tag" type="primary" size="sm" href="/tags/new" :disabled="loading" />
        </ClientOnly>
      </div>
    </div>
    <div v-if="loading" class="flex flex-col items-center space-y-4">
      <Skeleton />
      <Skeleton />
      <Skeleton />
    </div>
    <div v-else-if="Object.keys(filteredTags).length === 0" class="flex flex-col justify-start space-y-4">
      <h2>No tags found</h2>
      <div class="text-lg">Try searching for something else</div>
    </div>
    <div v-else class="flex flex-col items-center justify-start">
      <div v-for="(typeTags, type) in filteredTags" :key="type" class="flex w-full flex-col">
        <h2>{{ type }}</h2>
        <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          <TagListItem v-for="(tag, index) in typeTags" :key="index" :tag="tag" />
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import { isEmpty } from 'lodash-es'
import Skeleton from '../loading/Skeleton.vue'
import { useTagStore } from '../../store/TagStore'
import type { Tag } from '../../api/models/Tag'
import { TagType } from '../../api/models/TagType'
import TextInput from '../forms/TextInput.vue'
import FormButton from '../forms/FormButton.vue'
import TagTypeSelectList from '../forms/global/TagTypeSelectList.vue'
import useAuth from '../../composables/useAuth'
import { getEnumValue } from '../../utils/enum-helper'
import TagListItem from './TagListItem.vue'

interface Data {
  initialLoad: boolean
  search: string
  tagType?: TagType
}

export default defineComponent({
  name: 'TagList',
  components: {
    Skeleton,
    TagListItem,
    TextInput,
    FormButton,
    TagTypeSelectList,
  },
  setup() {
    const { isAuthenticated } = useAuth()

    return {
      isAuthenticated,
    }
  },
  data(): Data {
    return {
      initialLoad: true,
      search: '',
      tagType: undefined,
    }
  },
  computed: {
    tags(): Tag[] {
      return useTagStore().tags
    },
    loading(): boolean {
      return useTagStore().tagsLoading || this.initialLoad
    },
    filteredTags(): Record<string, Tag[]> {
      let tags = this.tags

      if (!isEmpty(this.search)) {
        tags = tags.filter((tag) => tag.title.toLowerCase().includes(this.search.toLowerCase()))
      }

      if (this.tagType) {
        tags = tags.filter((tag) => tag.type === this.tagType)
      }

      const groupedAndSorted = tags.reduce((acc: any, tag: Tag) => {
        const type = (acc[tag.type] = acc[tag.type] || [])
        type.push(tag)
        acc[tag.type].sort((a: Tag, b: Tag) => a.title.localeCompare(b.title))
        return acc
      }, {})

      const sortedGroupTitles = Object.keys(groupedAndSorted).sort((a, b) => a.localeCompare(b))
      const sortedGroupedTags = sortedGroupTitles.reduce((acc: any, title: string) => {
        acc[title] = groupedAndSorted[title]
        return acc
      }, {})

      return sortedGroupedTags
    },
  },
  async mounted() {
    this.setValues()
    await useTagStore().getTags()
  },
  methods: {
    setValues() {
      const route = this.$route
      this.search = route.query.search?.toString() || this.search
      this.tagType = (route.query.type ? getEnumValue(TagType, route.query.type.toString()) : undefined) || this.tagType
    },
    updateQueryParams() {
      this.$router.push({
        path: this.$route.path,
        query: {
          search: !isEmpty(this.search) ? this.search : undefined,
          type: this.tagType ? this.tagType : undefined,
        },
      })
    },
  },
  watch: {
    tags() {
      this.initialLoad = false
    },
    $route() {
      this.setValues()
    },
    search() {
      this.updateQueryParams()
    },
    tagType() {
      this.updateQueryParams()
    },
  },
})
</script>
