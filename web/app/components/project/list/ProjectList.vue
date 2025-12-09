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
import { isEmpty, orderBy } from 'lodash-es'
import { useRoute, useRouter } from 'vue-router'
import { useProjectStore } from '~/store/ProjectStore'
import { usePreferencesStore } from '~/store/PreferencesStore'
import type { Project, Company, Tag  } from '@api'
import { LoadingType } from '~/types/LoadingType'
import { ViewMode } from '~/types/ViewMode'
import useAuth from '~/composables/useAuth'
import ListLayout from '~/components/ui/layout/ListLayout.vue'
import Skeleton from '~/components/feedback/loading/Skeleton.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import FilterBar from '~/components/ui/filter/FilterBar.vue'
import ViewToggle from '~/components/ui/button/ViewToggle.vue'
import ProjectListItem from './ProjectListItem.vue'
import ProjectTableView from './ProjectTableView.vue'
import type { Filter } from '~/types/Filter'
import { useCompanyStore } from '~/store/CompanyStore'
import { FilterType } from '~/types/FilterType'
import { useTagStore } from '~/store/TagStore'
import { tagTypes } from '~/data/TagTypes'

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

const route = useRoute()
const router = useRouter()
const { isAuthenticated } = useAuth()
const projectStore = useProjectStore()
const companyStore = useCompanyStore()
const tagStore = useTagStore()
const initialLoad = ref(props.setProjects.length === 0)
const filters = ref<Filter[]>([])
const loadingTypeCard = ref(LoadingType.CARD)

const preferencesStore = usePreferencesStore()
const viewMode = computed({
  get: () => preferencesStore.projectsViewMode,
  set: (value: ViewMode) => preferencesStore.setProjectsViewMode(value),
})

const sortColumn = ref<string | null>('dateRange')
const sortDirection = ref<'asc' | 'desc'>('desc')

const projects = computed((): Project[] => {
  return projectStore.projects
})

const loading = computed((): boolean => {
  return projectStore.projectsLoading || initialLoad.value
})

const companies = computed((): Company[] => companyStore.companies)
const tags = computed((): Tag[] => tagStore.tags)

const filteredProjects = computed((): Project[] => {
  let projectsFiltered: Project[] = props.setProjects.length ? [...props.setProjects] : [...projects.value]

  filters.value.forEach(filter => {
    if (filter.type === FilterType.SEARCH && !isEmpty(filter.value)) {
      projectsFiltered = projectsFiltered.filter((project) => {
        return (
          project.title.toLowerCase().includes(filter.value.toLowerCase()) ||
          project.shortDescription.toLowerCase().includes(filter.value.toLowerCase()) ||
          project.description.toLowerCase().includes(filter.value.toLowerCase())
        )
      })
    } else if (filter.type === FilterType.COMPANY) {
      projectsFiltered = projectsFiltered.filter((project) => project.companyId === filter.value)
    } else if (filter.type === FilterType.TAG_TYPE) {
      projectsFiltered = projectsFiltered.filter((project) => {
        return project.tags.some((tag) => tag.type === filter.value)
      })
    } else if (filter.type === FilterType.TAG) {
      projectsFiltered = projectsFiltered.filter((project) => {
        return project.tags.some((tag) => tag.id === filter.value)
      })
    }
  })

  if (!isEmpty(props.modelSearch) && (props.modelSearch?.length ?? 0) > 3) {
    projectsFiltered = projectsFiltered.filter((project) => {
      if (!props.modelSearch) return true

      return (
        project.title.toLowerCase().includes(props.modelSearch.toLowerCase()) ||
        project.shortDescription.toLowerCase().includes(props.modelSearch.toLowerCase()) ||
        project.description.toLowerCase().includes(props.modelSearch.toLowerCase())
      )
    })
  }

  if (tags.value.length > 0) {
    projectsFiltered = projectsFiltered.filter((project) => {
      return project.tags.some((tag) => tags.value.map((tag) => tag.title).includes(tag.title))
    })
  }

  return projectsFiltered
})

const sortedProjects = computed((): Project[] => {
  if (!sortColumn.value) return [...filteredProjects.value]

  return orderBy(
    filteredProjects.value,
    [(p) => {
      if (sortColumn.value === 'project') return p.title.toLowerCase()
      if (sortColumn.value === 'dateRange') return new Date(p.startDate).getTime()
      return ''
    }],
    [sortDirection.value as 'asc' | 'desc']
  )
})

const wrapperClass = computed(() => ({
  [props.maxWidth]: true,
}))

function loadFiltersFromQuery() {
  const newFilters: Filter[] = []

  const search = route.query['search']?.toString()
  if (search) {
    newFilters.push({
      type: FilterType.SEARCH,
      value: search,
      label: 'Search',
    })
  }

  const company = route.query['company']?.toString()
  if (company) {
    const companyObj = companies.value.find(c => c.id === company)
    newFilters.push({
      type: FilterType.COMPANY,
      value: company,
      label: 'Company',
      displayValue: companyObj?.name,
      logo: companyObj?.logo,
    })
  }

  const tagType = route.query['type']?.toString()
  if (tagType) {
    const tagTypeValue = tagTypes.find(t => t.value === tagType)
    if (tagTypeValue) {
      newFilters.push({
        type: FilterType.TAG_TYPE,
        value: tagTypeValue.value,
        label: 'Tag Type',
        displayValue: tagTypeValue.label,
        icon: tagTypeValue.icon,
      })
    }
  }

  const tag = route.query['tag']?.toString()
  if (tag) {
    const tagObj = tags.value.find(t => t.id === tag)
    newFilters.push({
      type: FilterType.TAG,
      value: tag,
      label: 'Tag',
      displayValue: tagObj?.title,
      icon: tagObj?.icon,
    })
  }

  filters.value = newFilters
}

function updateQueryParams() {
  const query: Record<string, string | undefined> = {}

  filters.value.forEach(filter => {
    if (filter.type === FilterType.SEARCH) {
      query['search'] = filter.value
    } else if (filter.type === FilterType.COMPANY) {
      query['company'] = filter.value
    } else if (filter.type === FilterType.TAG_TYPE) {
      query['type'] = filter.displayValue || filter.value
    } else if (filter.type === FilterType.TAG) {
      query['tag'] = filter.value
    }
  })

  router.push({
    path: route.path,
    query,
  })
}

function handleSort(column: string) {
  if (sortColumn.value === column) {
    sortDirection.value = sortDirection.value === 'asc' ? 'desc' : 'asc'
  } else {
    sortColumn.value = column
    sortDirection.value = 'asc'
  }
}

onMounted(async () => {
  loadFiltersFromQuery()
  if (companies.value.length === 0) await companyStore.getCompanies()
  if (tags.value.length === 0) await tagStore.getTags()
  if (props.setProjects.length === 0) await projectStore.getProjects()
})

watch(
  () => projects.value,
  () => {
    initialLoad.value = false
  }
)

watch(companies, () => {  
  const updatedFilters = filters.value.map(filter => {
    if (filter.type === FilterType.COMPANY && !filter.displayValue) {
      const companyObj = companies.value.find(p => p.id === filter.value)
      if (companyObj) {
        return {
          ...filter,
          displayValue: companyObj.name,
          logo: companyObj.logo,
        }
      }
    }
    return filter
  })
  
  filters.value = updatedFilters
})

watch(tags, () => {  
  const updatedFilters = filters.value.map(filter => {
    if (filter.type === FilterType.TAG && !filter.displayValue) {
      const tagObj = tags.value.find(p => p.id === filter.value)
      if (tagObj) {
        return {
          ...filter,
          displayValue: tagObj.title,
          icon: tagObj.icon,
        }
      }
    }
    return filter
  })
  
  filters.value = updatedFilters
})

watch(() => props.setProjects, () => {
  initialLoad.value = false
})

watch(() => route.query, () => {
  loadFiltersFromQuery()
})

watch(filters, () => {
  updateQueryParams()
}, { deep: true })
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
