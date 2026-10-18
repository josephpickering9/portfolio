<template>
  <div class="mx-auto flex w-full max-w-300 flex-col gap-10 px-7 pt-16 pb-24">
    <div class="flex flex-wrap items-end justify-between gap-6">
      <PageHeader
        eyebrow="Technologies"
        title="Browse by technology"
        lede="The languages, frameworks and services behind my projects."
      />
      <ClientOnly>
        <FormButton v-if="isAuthenticated" href="/technologies/new" label="New technology" icon="heroicons:plus" />
      </ClientOnly>
    </div>

    <div class="flex flex-col gap-4 border-t border-line pt-6">
      <SearchField v-model="search" placeholder="Search technologies" @enter="openOnlyMatch" />
      <TagTypeFilterList v-model="type" :available="availableTypes" />
    </div>

    <LoadError
      v-if="projectStore.projectsError && !projectStore.projectsLoading"
      message="Couldn't load technologies. Check your connection and try again."
      @retry="projectStore.getProjects()"
    />

    <div
      v-else-if="projectStore.projectsPending"
      class="grid grid-cols-[repeat(auto-fill,minmax(min(100%,240px),1fr))] gap-2.5"
      aria-hidden="true"
      data-testid="skeleton"
    >
      <div v-for="n in SKELETON_COUNT" :key="n" class="skeleton-pulse h-14.5 rounded-xl border border-line bg-surface" />
    </div>

    <EmptyResults v-else-if="groups.length === 0" message="No technologies match." @clear="clearFilters" />

    <section v-for="group in groups" v-else :key="group.value" class="flex flex-col gap-4" data-testid="tag-group">
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
import { computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useProjectStore } from '~/store/ProjectStore'
import { useTagStore } from '~/store/TagStore'
import { tagTypes } from '~/data/TagTypes'
import useAuth from '~/composables/useAuth'
import { useSearchFilters } from '~/composables/useSearchFilters'
import { countProjectsByTag, filterTagUsages } from '~/utils/project-helper'
import type { TagUsage } from '~/utils/project-helper'
import PageHeader from '~/components/ui/layout/PageHeader.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import LoadError from '~/components/feedback/LoadError.vue'
import EmptyResults from '~/components/feedback/EmptyResults.vue'
import SearchField from '~/components/ui/input/SearchField.vue'
import TagTypeFilterList from '~/components/tag/filter/TagTypeFilterList.vue'
import TagListItem from './TagListItem.vue'

const SKELETON_COUNT = 12

const router = useRouter()
const projectStore = useProjectStore()
const tagStore = useTagStore()
const { isAuthenticated } = useAuth()
const { search, type, clearFilters } = useSearchFilters()

const usages = computed((): TagUsage[] => {
  const used = countProjectsByTag(projectStore.projects)
  if (!isAuthenticated.value) return used

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

async function openOnlyMatch() {
  const [only, ...rest] = matches.value
  if (only && rest.length === 0) await router.push({ path: '/projects', query: { q: only.tag.title } })
}

onMounted(() => {
  if (isAuthenticated.value) tagStore.ensureTags()
})
</script>
