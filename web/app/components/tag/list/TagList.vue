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
    <div v-if="loading" class="mt-12 flex flex-col items-center space-y-4">
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

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { isEmpty } from 'lodash-es'
import { useRoute, useRouter } from 'vue-router'
import { useTagStore } from '~/store/TagStore'
import type { Tag } from '@api/models/Tag'
import { TagType } from '@api/models/TagType'
import useAuth from '~/composables/useAuth'
import { getEnumValue } from '~/utils/enum-helper'
import Skeleton from '~/components/feedback/loading/Skeleton.vue'
import TextInput from '~/components/ui/input/TextInput.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import TagTypeSelectList from '~/components/tag/form/TagTypeSelectList.vue'
import TagListItem from './TagListItem.vue'

const route = useRoute()
const router = useRouter()
const { isAuthenticated } = useAuth()
const tagStore = useTagStore()

const initialLoad = ref(true)
const search = ref('')
const tagType = ref<TagType | undefined>(undefined)

const tags = computed((): Tag[] => {
  return tagStore.tags
})

const loading = computed((): boolean => {
  return tagStore.tagsLoading || initialLoad.value
})

const filteredTags = computed((): Record<string, Tag[]> => {
  let tagsFiltered = tags.value

  if (!isEmpty(search.value)) {
    tagsFiltered = tagsFiltered.filter((tag) => tag.title.toLowerCase().includes(search.value.toLowerCase()))
  }

  if (tagType.value) {
    tagsFiltered = tagsFiltered.filter((tag) => tag.type === tagType.value)
  }

  const groupedAndSorted = tagsFiltered.reduce((acc: any, tag: Tag) => {
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
})

function setValues() {
  search.value = route.query.search?.toString() || search.value
  tagType.value = (route.query.type ? getEnumValue(TagType, route.query.type.toString()) : undefined) || tagType.value
}

function updateQueryParams() {
  router.push({
    path: route.path,
    query: {
      search: !isEmpty(search.value) ? search.value : undefined,
      type: tagType.value ? tagType.value : undefined,
    },
  })
}

onMounted(async () => {
  setValues()
  await tagStore.getTags()
})

// Watch methods
watch(tags, () => {
  initialLoad.value = false
})

watch(() => route.query, () => {
  setValues()
})

watch(search, () => {
  updateQueryParams()
})

watch(tagType, () => {
  updateQueryParams()
})
</script>
../../../store/TagStore../../../api/models/Tag../../../api/models/TagType../../../composables/useAuth../../../utils/enum-helper
