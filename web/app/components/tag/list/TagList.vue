<template>
  <ListLayout
    title="Tags"
    description="Browse and manage project technologies and categories."
    :loading="loading"
    :is-empty="Object.keys(filteredTags).length === 0"
    empty-title="No tags found"
  >
    <template #actions>
      <div class="group relative">
        <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
          <Icon name="heroicons:magnifying-glass" class="h-5 w-5 text-base-content/40 group-hover:text-primary" />
        </div>
        <TextInput
          v-model="search"
          class="w-full md:max-w-48" 
          size="md"
          placeholder="Search tags..."
          :disabled="loading"
        />
      </div>
      
      <TagTypeSelectList
        v-model="tagType"
        class="w-full sm:w-48"
        size="md"
        placeholder="Filter by Type"
        :disabled="loading"
      />

      <ClientOnly>
        <FormButton
          v-if="isAuthenticated"
          label="Add Tag"
          icon="heroicons:plus"
          type="primary"
          size="md"
          href="/tags/new"
          :disabled="loading"
          class="w-full shadow-lg shadow-primary/20 sm:w-auto"
        />
      </ClientOnly>
    </template>

    <div v-for="(typeTags, type) in filteredTags" :key="type" class="flex w-full flex-col">
      <div class="mb-6 flex items-center gap-4">
        <h2 class="text-2xl font-bold text-base-content">{{ type }}</h2>
        <div class="h-px flex-1 bg-gradient-to-r from-base-content/10 via-base-content/5 to-transparent" />
      </div>
      
      <div class="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
        <TagListItem
          v-for="(tag, index) in typeTags"
          :key="index"
          :tag="tag"
        />
      </div>
    </div>
  </ListLayout>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { chain, isEmpty, sortBy } from 'lodash-es'
import { useQuerySync } from '~/composables/useQuerySync'
import { useTagStore } from '~/store/TagStore'
import type { Tag } from '@api/models/Tag'
import { TagType } from '@api/models/TagType'
import useAuth from '~/composables/useAuth'
import { getEnumValue } from '~/utils/enum-helper'
import ListLayout from '~/components/ui/layout/ListLayout.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import TagTypeSelectList from '~/components/tag/form/TagTypeSelectList.vue'
import TagListItem from './TagListItem.vue'
import TextInput from '~/components/ui/input/TextInput.vue'


const { isAuthenticated } = useAuth()
const tagStore = useTagStore()

const initialLoad = ref(true)
const search = ref('')
const tagType = ref<TagType | undefined>(undefined)

useQuerySync(search, { key: 'search', initialValue: '' })
useQuerySync(tagType, { 
  key: 'type', 
  initialValue: undefined,
  transform: (val) => val ? getEnumValue(TagType, val) : undefined,
})

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

  const sortedGroupedTags = chain(tagsFiltered)
    .groupBy('type')
    .mapValues((tags) => sortBy(tags, ['title']))
    .toPairs()
    .sortBy(0)
    .fromPairs()
    .value()

  return sortedGroupedTags as Record<string, Tag[]>
})

onMounted(async () => {
  await tagStore.getTags()
})

watch(tags, () => {
  initialLoad.value = false
})
</script>

