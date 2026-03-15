<template>
  <ListLayout
    v-if="showHeader"
    title="Projects"
    :loading="loading"
    :is-empty="filteredProjects.length === 0"
    empty-title="No projects found"
    :max-width="maxWidth"
  >
    <template #actions>
      <FilterBar v-model:filters="filters" />
      <ViewToggle v-model="viewMode" :disabled="loading" class="hidden md:flex" />
      <ClientOnly>
        <FormButton
          v-if="isAuthenticated"
          label="Add Project"
          type="primary"
          size="sm"
          href="/projects/new"
          :disabled="loading"
        />
      </ClientOnly>
    </template>

    <ProjectTableView
      v-if="viewMode === ViewMode.TABLE"
      :projects="sortedProjects"
      :sort-column="sortColumn"
      :sort-direction="sortDirection"
      @sort="handleSort"
    />

    <TransitionGroup
      v-else
      name="list"
      tag="div"
      class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3"
    >
      <ProjectListItem v-for="project in filteredProjects" :key="project.id" :project="project" />
    </TransitionGroup>
  </ListLayout>

  <div v-else class="projects w-full space-y-8" :class="wrapperClass">
    <div v-if="loading" class="mt-12 grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
      <Skeleton :type="loadingTypeCard" />
      <Skeleton :type="loadingTypeCard" />
      <Skeleton :type="loadingTypeCard" />
    </div>
    <div v-else-if="filteredProjects.length === 0" class="flex flex-col justify-start space-y-4">
      <h2>No projects found</h2>
    </div>
    <div v-else class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
      <ProjectListItem v-for="project in filteredProjects" :key="project.id" :project="project" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { useProjectStore } from '~/store/ProjectStore'
import { usePreferencesStore } from '~/store/PreferencesStore'
import type { Project } from '@api'
import { LoadingType } from '~/types/LoadingType'
import { ViewMode } from '~/types/ViewMode'
import useAuth from '~/composables/useAuth'
import { useProjectFilters } from '~/composables/useProjectFilters'
import ListLayout from '~/components/ui/layout/ListLayout.vue'
import Skeleton from '~/components/feedback/loading/Skeleton.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import FilterBar from '~/components/ui/filter/FilterBar.vue'
import ViewToggle from '~/components/ui/button/ViewToggle.vue'
import ProjectListItem from './ProjectListItem.vue'
import ProjectTableView from './ProjectTableView.vue'

interface Props {
  showHeader?: boolean
  maxWidth?: string
  modelSearch?: string
  setProjects?: Project[]
}

const props = withDefaults(defineProps<Props>(), {
  showHeader: false,
  maxWidth: 'max-w-6xl',
  modelSearch: undefined,
  setProjects: () => [],
})

const projectStore = useProjectStore()
const preferencesStore = usePreferencesStore()
const { isAuthenticated } = useAuth()
const initialLoad = ref(props.setProjects.length === 0)
const loadingTypeCard = ref(LoadingType.CARD)

const viewMode = computed({
  get: () => preferencesStore.projectsViewMode,
  set: (value: ViewMode) => preferencesStore.setProjectsViewMode(value),
})

const { filters, sortColumn, sortDirection, filteredProjects, sortedProjects, handleSort } = useProjectFilters(
  () => projectStore.projects,
  () => props.setProjects,
  () => props.modelSearch,
)

const loading = computed(() => projectStore.projectsLoading || initialLoad.value)

const wrapperClass = computed(() => ({ [props.maxWidth]: true }))

watch(() => projectStore.projects, () => { initialLoad.value = false })
watch(() => props.setProjects, () => { initialLoad.value = false })

onMounted(async () => {
  if (props.setProjects.length === 0) await projectStore.getProjects()
})
</script>

<style scoped>
.list-move,
.list-enter-active,
.list-leave-active {
  transition: all 0.5s ease;
}

.list-enter-from,
.list-leave-to {
  opacity: 0;
  transform: translateY(30px);
}

.list-leave-active {
  position: absolute;
}
</style>
