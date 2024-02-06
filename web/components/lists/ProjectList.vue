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

<script lang="ts">
import { defineComponent } from 'vue'
import { isEmpty } from 'lodash-es'
import Skeleton from '../loading/Skeleton.vue'
import { useProjectStore } from '../../store/ProjectStore'
import type { Project } from '../../api/models/Project'
import { TagType } from '../../api'
import TextInput from '../forms/TextInput.vue'
import FormButton from '../forms/FormButton.vue'
import TagTypeSelectList from '../forms/global/TagTypeSelectList.vue'
import { LoadingType } from '../../types/LoadingType'
import useAuth from '../../composables/useAuth'
import { getEnumValue } from '../../utils/enum-helper'
import CompanyAutoComplete from '../forms/global/CompanyAutoComplete.vue'
import ProjectListItem from './ProjectListItem.vue'

interface Data {
  initialLoad: boolean
  search?: string
  companyId?: number
  tagType?: TagType
  loadingTypeCard: LoadingType
}

export default defineComponent({
  name: 'ProjectList',
  components: {
    Skeleton,
    ProjectListItem,
    TextInput,
    FormButton,
    TagTypeSelectList,
    CompanyAutoComplete,
  },
  props: {
    showHeader: {
      type: Boolean,
      default: false,
    },
    maxWidth: {
      type: String,
      default: 'max-w-5xl',
    },
    tags: {
      type: Array as () => string[],
      default: () => [],
    },
    modelSearch: {
      type: String,
      default: undefined,
    },
  },
  setup() {
    const { isAuthenticated } = useAuth()

    return {
      isAuthenticated,
    }
  },
  data(): Data {
    return {
      initialLoad: true,
      search: undefined,
      companyId: undefined,
      tagType: undefined,
      loadingTypeCard: LoadingType.CARD,
    }
  },
  computed: {
    projects(): Project[] {
      return useProjectStore().projects
    },
    loading(): boolean {
      return useProjectStore().projectsLoading || this.initialLoad
    },
    filteredProjects(): Project[] {
      let projects: Project[] = [...this.projects]

      if (!isEmpty(this.search)) {
        projects = projects.filter((project) => {
          if (!this.search) return true

          return (
            project.title.toLowerCase().includes(this.search.toLowerCase()) ||
            project.shortDescription.toLowerCase().includes(this.search.toLowerCase()) ||
            project.description.toLowerCase().includes(this.search.toLowerCase())
          )
        })
      }

      if (!isEmpty(this.modelSearch) && (this.modelSearch?.length ?? 0) > 3) {
        projects = projects.filter((project) => {
          if (!this.modelSearch) return true

          return (
            project.title.toLowerCase().includes(this.modelSearch.toLowerCase()) ||
            project.shortDescription.toLowerCase().includes(this.modelSearch.toLowerCase()) ||
            project.description.toLowerCase().includes(this.modelSearch.toLowerCase())
          )
        })
      }

      if (this.companyId) {
        projects = projects.filter((project) => project.companyId === this.companyId)
      }

      if (this.tagType) {
        projects = projects.filter((project) => {
          return project.tags.some((tag) => tag.type === this.tagType)
        })
      }

      if (this.tags.length > 0) {
        projects = projects.filter((project) => {
          return project.tags.some((tag) => this.tags.includes(tag.title))
        })
      }

      return projects
    },
    wrapperClass(): object {
      return {
        [this.maxWidth]: true,
      }
    },
  },
  async mounted() {
    const route = this.$route
    this.search = route.query.search?.toString() || this.search
    this.companyId = route.query.company ? parseInt(route.query.company.toString()) : undefined
    this.tagType = (route.query.type ? getEnumValue(TagType, route.query.type.toString()) : undefined) || this.tagType

    await useProjectStore().getProjects()
  },
  methods: {
    updateQueryParams() {
      this.$router.push({
        path: this.$route.path,
        query: {
          search: !isEmpty(this.search) ? this.search : undefined,
          company: this.companyId ? this.companyId : undefined,
          type: this.tagType ? this.tagType : undefined,
        },
      })
    },
  },
  watch: {
    projects() {
      this.initialLoad = false
    },
    search() {
      this.updateQueryParams()
    },
    companyId() {
      this.updateQueryParams()
    },
    tagType() {
      this.updateQueryParams()
    },
  },
})
</script>
