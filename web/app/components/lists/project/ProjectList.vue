<template>
  <div class="projects w-full space-y-8" :class="wrapperClass">
    <div v-if="showHeader" class="prose flex max-w-full flex-col items-center justify-between gap-4 md:flex-row">
      <h1 class="m-0">Projects</h1>
      <div class="flex items-center gap-4">
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
      </div>
    </div>
    <div v-if="loading" class="mt-12 grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      <Skeleton :type="loadingTypeCard" />
      <Skeleton :type="loadingTypeCard" />
      <Skeleton :type="loadingTypeCard" />
      <Skeleton :type="loadingTypeCard" />
      <Skeleton :type="loadingTypeCard" />
      <Skeleton :type="loadingTypeCard" />
    </div>
    <div v-else-if="filteredProjects.length === 0" class="flex flex-col justify-start space-y-4">
      <h2>No projects found</h2>
      <div class="text-lg">Try searching for something else</div>
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
import { useProjectStore } from '~/app/store/ProjectStore'
import type { Project } from '~/api/models/Project'
import { TagType } from '~/api'
import { LoadingType } from '~/types/LoadingType'
import useAuth from '~/app/composables/useAuth'
import { getEnumValue } from '~/app/utils/enum-helper'
import Skeleton from '~/app/components/loading/Skeleton.vue'
import TextInput from '~/app/components/forms/elements/TextInput.vue'
import FormButton from '~/app/components/forms/elements/FormButton.vue'
import TagTypeSelectList from '~/app/components/forms/tag/TagTypeSelectList.vue'
import CompanyAutoComplete from '~/app/components/forms/company/CompanyAutoComplete.vue'
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
const companyId = ref<number | undefined>(undefined)
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
  search.value = route.query.search?.toString() || search.value
  companyId.value = route.query.company ? parseInt(route.query.company.toString()) : undefined
  tagType.value = route.query.type ? getEnumValue(TagType, route.query.type.toString()) : undefined
}

function updateQueryParams() {
  router.push({
    path: route.path,
    query: {
      search: !isEmpty(search.value) ? search.value : undefined,
      company: companyId.value ? companyId.value : undefined,
      type: tagType.value ? tagType.value : undefined,
    },
  })
}

onMounted(async () => {
  setValues()

  if (props.setProjects.length === 0) await projectStore.getProjects()
})

// Watch methods
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
</script>
