<template>
  <div class="mx-auto flex w-full max-w-300 flex-col gap-9 px-7 pt-16 pb-24">
    <div class="flex flex-wrap items-end justify-between gap-6">
      <PageHeader eyebrow="Projects" title="All projects" lede="All of the projects that I've been involved in" />
      <ClientOnly>
        <FormButton v-if="isAuthenticated" href="/projects/new" label="New project" icon="heroicons:plus" />
      </ClientOnly>
    </div>

    <div class="flex flex-col gap-4 border-t border-line pt-6">
      <div class="flex flex-wrap items-center gap-3">
        <SearchField v-model="search" placeholder="Search projects" class="flex-[1_1_280px]" />
        <ViewToggle v-model="viewMode" />
      </div>
      <TagTypeFilterList v-model="type" :available="availableTypes" />
    </div>

    <h2 class="sr-only">Project list</h2>

    <LoadError v-if="projectStore.projectsError && !projectStore.projectsLoading" @retry="projectStore.getProjects()" />

    <div v-else-if="projectStore.projectsPending" class="grid grid-cols-[repeat(auto-fill,minmax(min(100%,340px),1fr))] gap-8">
      <Skeleton v-for="n in SKELETON_COUNT" :key="n" :type="LoadingType.CARD" />
    </div>

    <EmptyResults v-else-if="filteredProjects.length === 0" message="No projects match." @clear="clearFilters" />

    <div
      v-else-if="viewMode === ViewMode.CARD"
      class="grid grid-cols-[repeat(auto-fill,minmax(min(100%,340px),1fr))] gap-8"
      data-testid="project-grid"
    >
      <ProjectListItem v-for="project in filteredProjects" :key="project.id" :project="project" />
    </div>

    <div v-else class="flex flex-col border-t border-line" data-testid="project-list">
      <ProjectListRow v-for="(project, index) in filteredProjects" :key="project.id" :project="project" :index="index" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useProjectStore } from '~/store/ProjectStore'
import { usePreferencesStore } from '~/store/PreferencesStore'
import { LoadingType } from '~/types/LoadingType'
import { ViewMode } from '~/types/ViewMode'
import useAuth from '~/composables/useAuth'
import { useProjectFilters } from '~/composables/useProjectFilters'
import PageHeader from '~/components/ui/layout/PageHeader.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import ViewToggle from '~/components/ui/button/ViewToggle.vue'
import SearchField from '~/components/ui/input/SearchField.vue'
import TagTypeFilterList from '~/components/tag/filter/TagTypeFilterList.vue'
import Skeleton from '~/components/feedback/loading/Skeleton.vue'
import LoadError from '~/components/feedback/LoadError.vue'
import EmptyResults from '~/components/feedback/EmptyResults.vue'
import ProjectListItem from './ProjectListItem.vue'
import ProjectListRow from './ProjectListRow.vue'

const SKELETON_COUNT = 6

const projectStore = useProjectStore()
const preferencesStore = usePreferencesStore()
const { isAuthenticated } = useAuth()
const { search, type, filteredProjects, availableTypes, clearFilters } = useProjectFilters(() => projectStore.projects)

const viewMode = computed({
  get: () => preferencesStore.projectsViewMode,
  set: (value: ViewMode) => preferencesStore.setProjectsViewMode(value),
})
</script>
