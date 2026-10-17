<template>
  <div class="mx-auto flex w-full max-w-300 flex-col gap-10 px-7 pt-16 pb-24">
    <div class="flex flex-wrap items-end justify-between gap-6">
      <PageHeader
        eyebrow="Technologies"
        title="Browse by technology"
        lede="The languages, frameworks and services behind my projects."
      />
      <ClientOnly>
        <NuxtLink
          v-if="isAuthenticated"
          to="/technologies/new"
          class="flex items-center gap-2 rounded-full bg-accent px-4 py-2.25 text-sm font-medium text-accent-ink hover:text-accent-ink"
        >
          <Icon name="heroicons:plus" />
          New technology
        </NuxtLink>
      </ClientOnly>
    </div>

    <div class="flex flex-col gap-4 border-t border-line pt-6">
      <SearchField v-model="search" placeholder="Search technologies" @enter="openOnlyMatch" />
      <TagTypeFilterList v-model="type" :available="availableTypes" />
    </div>

    <LoadError
      v-if="projectStore.projectsError && !projectStore.projectsLoading"
      message="Couldn't load technologies. Check your connection and try again."
      @retry="load"
    />

    <div v-else-if="loading" class="grid grid-cols-[repeat(auto-fill,minmax(min(100%,240px),1fr))] gap-2.5" aria-hidden="true">
      <div v-for="n in SKELETON_COUNT" :key="n" class="skeleton-pulse h-14.5 rounded-xl border border-line bg-surface" />
    </div>

    <div v-else-if="groups.length === 0" class="flex flex-col items-start gap-3 py-12">
      <span class="font-display text-[32px]">No technologies match.</span>
      <button type="button" class="rounded-full border border-line px-4 py-2.25 text-sm text-text" @click="clearFilters">
        Clear filters
      </button>
    </div>

    <section v-for="group in groups" v-else :key="group.value" class="flex flex-col gap-4">
      <h2 class="m-0 flex items-center gap-2.5 font-display text-[32px] font-normal">
        <Icon :name="group.icon" size="22" class="text-accent" />
        {{ group.label }}
      </h2>
      <div class="grid grid-cols-[repeat(auto-fill,minmax(min(100%,240px),1fr))] gap-2.5">
        <TagListItem v-for="usage in group.items" :key="usage.tag.id" :tag="usage.tag" :count="usage.count" />
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import type { TagType } from '@api'
import { useProjectStore } from '~/store/ProjectStore'
import { useTagStore } from '~/store/TagStore'
import { tagTypes } from '~/data/TagTypes'
import useAuth from '~/composables/useAuth'
import { useQuerySync } from '~/composables/useQuerySync'
import { countProjectsByTag, filterTagUsages } from '~/utils/project-helper'
import type { TagUsage } from '~/utils/project-helper'
import PageHeader from '~/components/ui/layout/PageHeader.vue'
import LoadError from '~/components/feedback/LoadError.vue'
import SearchField from '~/components/ui/input/SearchField.vue'
import TagTypeFilterList from '~/components/tag/filter/TagTypeFilterList.vue'
import TagListItem from './TagListItem.vue'

const SKELETON_COUNT = 12

const router = useRouter()
const projectStore = useProjectStore()
const tagStore = useTagStore()
const { isAuthenticated } = useAuth()

const search = ref('')
const type = ref<TagType | undefined>()
useQuerySync(search, { key: 'q', initialValue: '' })
useQuerySync(type, {
  key: 'type',
  initialValue: undefined,
  transform: (value) => tagTypes.find((tagType) => tagType.value === value)?.value,
  serialize: (value) => value,
})

const loading = computed(() => projectStore.projectsLoading || (!projectStore.projectsForm.data && !projectStore.projectsError))

const usages = computed((): TagUsage[] => {
  const used = countProjectsByTag(projectStore.projects)
  if (!isAuthenticated?.value) return used

  const usedIds = new Set(used.map((usage) => usage.tag.id))
  const unused = tagStore.tags.filter((tag) => !usedIds.has(tag.id)).map((tag) => ({ tag, count: 0 }))
  return [...used, ...unused]
})

const availableTypes = computed(() => new Set(usages.value.map((usage) => usage.tag.type)))

const matches = computed(() => filterTagUsages(usages.value, search.value, type.value))

const groups = computed(() =>
  tagTypes
    .map((tagType) => ({ ...tagType, items: matches.value.filter((usage) => usage.tag.type === tagType.value) }))
    .filter((group) => group.items.length > 0),
)

function clearFilters() {
  search.value = ''
  type.value = undefined
}

function openOnlyMatch() {
  const [only, ...rest] = matches.value
  if (only && rest.length === 0) router.push({ path: '/projects', query: { q: only.tag.title } })
}

function load() {
  projectStore.getProjects()
}

onMounted(() => {
  if (!projectStore.projectsForm.data) load()
  if (isAuthenticated?.value && !tagStore.tags.length) tagStore.getTags()
})
</script>
