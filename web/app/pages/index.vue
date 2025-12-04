<template>
  <div
    class="flex flex-grow transform flex-col items-center justify-center gap-6 px-4 py-1 transition-all duration-300 ease-in-out"
    :class="{ 'justify-start': isSearchActive }"
  >
    <TagAutoComplete
      v-model="tags"
      v-model:model-search="search"
      type="text"
      class="z-50 w-full max-w-xs transform transition-all duration-300 ease-in-out"
      :class="{ 'max-w-5xl': isSearchActive }"
      :placeholder="placeholder"
      :show-empty-message="false"
      :open-on-focus="false"
      @focus="handleFocus"
      @blur="handleBlur"
    />
    <div v-if="isSearchActive" class="z-0 flex w-full justify-center">
      <ProjectList :model-search="search" :tags="tags" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, nextTick } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { isEmpty } from 'lodash-es'
import { useProjectStore } from '~/store/ProjectStore'
import useMeta from '~/composables/useMeta'
import { notEmpty } from '~/utils/array-helper'
import ProjectList from '~/components/project/list/ProjectList.vue'
import TagAutoComplete from '~/components/tag/form/TagAutoComplete.vue'

const route = useRoute()
const router = useRouter()
const projectStore = useProjectStore()
const { updateMeta } = useMeta()

updateMeta({ title: 'Work Experience', description: "Work experience portfolio for my life's work" })

const isSearchActive = ref(false)
const search = ref('')
const tags = ref<string[]>([])

const placeholder = computed((): string => {
  return isSearchActive.value ? 'Search by tag or keyword' : 'Search'
})

async function getProjects() {
  await projectStore.getProjects()
}

function handleFocus() {
  isSearchActive.value = true
}

function handleBlur() {
  if (isEmpty(search.value) && isEmpty(tags.value)) {
    isSearchActive.value = false
  }
}

function setValues() {
  search.value = route.query.search?.toString() || search.value
  const queryTags = route.query.tags ? route.query.tags : []
  tags.value = Array.isArray(queryTags)
    ? queryTags.map((tag) => tag?.toString()).filter(notEmpty)
    : [queryTags as string]

  isSearchActive.value = !isEmpty(search.value) || tags.value.length > 0
}

function updateQueryParams() {
  router.push({
    path: route.path,
    query: {
      search: !isEmpty(search.value) ? search.value : undefined,
      tags: tags.value && tags.value.length > 0 ? tags.value : undefined,
    },
  })
}

onMounted(() => {
  setValues()
})

watch(tags, () => {
  nextTick(() => {
    updateQueryParams()
  })
}, { deep: true })
</script>
