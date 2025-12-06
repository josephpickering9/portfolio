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
      <TextInput v-model="search" class="w-full md:w-48" size="sm" placeholder="Search" :disabled="loading" />
      <CompanyAutoComplete
        v-model="companyId"
        class="w-full md:w-48"
        placeholder="Company"
        size="sm"
        :disabled="loading"
      />
      <TagTypeSelectList v-model="tagType" class="w-full md:w-48" placeholder="Type" size="sm" :disabled="loading" />
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

    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      <ProjectListItem v-for="project in filteredProjects" :key="project.id" :project="project" />
    </div>
  </ListLayout>

  <!-- Fallback for generic list usage without filters/header (if needed for widgets) -->
  <div v-else class="projects w-full space-y-8" :class="wrapperClass">
     <div v-if="loading" class="mt-12 grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      <Skeleton :type="loadingTypeCard" />
      <Skeleton :type="loadingTypeCard" />
      <Skeleton :type="loadingTypeCard" />
     </div>
     <div v-else-if="filteredProjects.length === 0" class="flex flex-col justify-start space-y-4">
       <h2>No projects found</h2>
     </div>
     <div v-else class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      <ProjectListItem v-for="project in filteredProjects" :key="project.id" :project="project" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { isEmpty } from 'lodash-es'
import { useRoute, useRouter } from 'vue-router'
import { useProjectStore } from '~/store/ProjectStore'
import type { Project } from '@api/models/Project'
import { TagType } from '@api'
import { LoadingType } from '@types/LoadingType'
import useAuth from '~/composables/useAuth'
import { getEnumValue } from '~/utils/enum-helper'
import ListLayout from '~/components/ui/layout/ListLayout.vue'
import Skeleton from '~/components/feedback/loading/Skeleton.vue'
import TextInput from '~/components/ui/input/TextInput.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import TagTypeSelectList from '~/components/tag/form/TagTypeSelectList.vue'
import CompanyAutoComplete from '~/components/company/form/CompanyAutoComplete.vue'
import ProjectListItem from './ProjectListItem.vue'

interface Props {
  showHeader?: boolean
  maxWidth?: string
  tags?: string[]
  modelSearch?: string
  setProjects?: Project[]
}

const props = withDefaults(defineProps<Props>(), {
  showHeader: false,
  maxWidth: 'max-w-5xl',
  tags: () => [],
  modelSearch: undefined,
  setProjects: () => [],
})

const route = useRoute()
const router = useRouter()
const { isAuthenticated } = useAuth()
const projectStore = useProjectStore()

const initialLoad = ref(props.setProjects.length === 0)
const search = ref<string | undefined>(undefined)
const companyId = ref<string | undefined>(undefined)
const tagId = ref<string | undefined>(undefined)
const tagType = ref<TagType | undefined>(undefined)
const loadingTypeCard = ref(LoadingType.CARD)

const projects = computed((): Project[] => {
  return projectStore.projects
})

const loading = computed((): boolean => {
  return projectStore.projectsLoading || initialLoad.value
})

const filteredProjects = computed((): Project[] => {
  let projectsFiltered: Project[] = props.setProjects.length ? [...props.setProjects] : [...projects.value]

  if (!isEmpty(search.value)) {
    projectsFiltered = projectsFiltered.filter((project) => {
      if (!search.value) return true

      return (
        project.title.toLowerCase().includes(search.value.toLowerCase()) ||
        project.shortDescription.toLowerCase().includes(search.value.toLowerCase()) ||
        project.description.toLowerCase().includes(search.value.toLowerCase())
      )
    })
  }

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

  if (companyId.value) {
    projectsFiltered = projectsFiltered.filter((project) => project.companyId === companyId.value)
  }

  if (tagType.value) {
    projectsFiltered = projectsFiltered.filter((project) => {
      return project.tags.some((tag) => tag.type === tagType.value)
    })
  }

  if (tagId.value) {
    projectsFiltered = projectsFiltered.filter((project) => {
      return project.tags.some((tag) => tag.id === tagId.value)
    })
  }

  if (props.tags.length > 0) {
    projectsFiltered = projectsFiltered.filter((project) => {
      return project.tags.some((tag) => props.tags.includes(tag.title))
    })
  }

  return projectsFiltered
})

const wrapperClass = computed(() => ({
  [props.maxWidth]: true,
}))

function setValues() {
  search.value = route.query['search']?.toString() || search.value
  companyId.value = route.query['company'] ? parseInt(route.query['company'].toString()) : undefined
  tagType.value = route.query['type'] ? getEnumValue(TagType, route.query['type'].toString()) : undefined
  tagId.value = route.query['tag']?.toString()
}

function updateQueryParams() {
  router.push({
    path: route.path,
    query: {
      search: !isEmpty(search.value) ? search.value : undefined,
      company: companyId.value ? companyId.value : undefined,
      type: tagType.value ? tagType.value : undefined,
      tag: tagId.value ? tagId.value : undefined,
    },
  })
}

onMounted(async () => {
  setValues()

  if (props.setProjects.length === 0) await projectStore.getProjects()
})


watch(projects, () => {
  initialLoad.value = false
})

watch(() => props.setProjects, () => {
  initialLoad.value = false
})

watch(() => route.query, () => {
  setValues()
})

watch(search, () => {
  updateQueryParams()
})

watch(companyId, () => {
  updateQueryParams()
})

watch(tagType, () => {
  updateQueryParams()
})

watch(tagId, () => {
  updateQueryParams()
})
</script>
