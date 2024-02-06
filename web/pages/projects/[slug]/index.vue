<template>
  <div class="flex flex-grow overflow-hidden">
    <Skeleton v-if="loading" />
    <div v-else-if="project" class="relative flex w-full flex-grow flex-col gap-2 overflow-hidden">
      <!-- TODO: dark/light gradient -->
      <div class="header-container header-container-dark" :style="headerStyle">
        <div class="prose z-10 flex w-full max-w-full items-center justify-between gap-4 overflow-hidden px-8">
          <div class="flex flex-col gap-x-4 gap-y-2 md:flex-row md:items-center">
            <div class="flex items-center justify-start space-x-2 md:hidden">
              <img v-if="project.image" :src="getImageUrl(project.image)" class="m-0 h-10 w-10 rounded-full" />
              <small class="text-white">{{ project.year }}</small>
            </div>
            <img v-if="project.image" :src="getImageUrl(project.image)" class="m-0 hidden h-10 w-10 rounded-full md:flex" />
            <h1 ref="title" class="title">{{ project.title }}</h1>
            <small class="hidden text-white md:flex">{{ project.year }}</small>
          </div>
          <ClientOnly>
            <NuxtLink
              v-if="isAuthenticated"
              :to="`/projects/${$route.params.slug}/update`"
              class="absolute right-8 top-6 text-xs md:relative md:right-auto md:top-auto"
            >
              Edit
            </NuxtLink>
          </ClientOnly>
        </div>
      </div>

      <div class="prose mx-auto flex w-full max-w-7xl flex-col justify-center gap-4 overflow-hidden px-8 md:mt-0">
        <div class="flex flex-col gap-6 lg:flex-row lg:items-end lg:justify-between">
          <div v-if="Object.keys(groupedTags).length > 0" class="flex flex-wrap items-center gap-x-4 gap-y-1">
            <div v-for="(typeTags, type) in groupedTags" :key="type">
              <small class="m-0 italic">{{ type }}</small>
              <div class="flex flex-wrap gap-2">
                <Tag v-for="(tag, index) in typeTags" :key="index" :tag="tag" />
              </div>
            </div>
          </div>
          <div class="flex items-center gap-6">
            <IconLink
              v-if="project.website"
              :to="project.website"
              label="Website"
              icon="material-symbols:globe"
              icon-size="1.5em"
            />
            <!-- <IconLink v-if="project.repository" :to="project.repository" label="GitHub" icon="mdi:github" /> -->
            <CompanyItem v-if="company" :company="company" :show-link="true" />
          </div>
        </div>

        <h2 class="m-0 text-xl font-normal italic">{{ project.shortDescription }}</h2>
        <div class="project-description" v-html="project.description" />

        <div v-if="project.website" class="flex flex-col items-center gap-4 md:flex-row">
          <MockupBrowser class="hidden md:block" :url="project.website" />
          <MockupPhone :url="project.website" />
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { useRoute } from 'vue-router'
import type { Company, Project, Tag as TagModel } from '../../../api'
import { useProjectStore } from '../../../store/ProjectStore'
import Skeleton from '../../../components/loading/Skeleton.vue'
import Tag from '../../../components/tags/Tag.vue'
import IconLink from '../../../components/navigation/IconLink.vue'
import MockupBrowser from '../../../components/mockup/MockupBrowser.vue'
import MockupPhone from '../../../components/mockup/MockupPhone.vue'
import CompanyItem from '../../../components/forms/global/CompanyItem.vue'
import { useCompanyStore } from '../../../store/CompanyStore'
import useMeta from '../../../composables/useMeta'
import { getImageUrl } from '../../../utils/image-helper'
import useAuth from '../../../composables/useAuth'
import { getShadeFromOklsh } from '../../../utils/colour-helper'
import { defineNuxtComponent } from '#app'

export default defineNuxtComponent({
  name: 'Project',
  components: { Skeleton, Tag, IconLink, MockupBrowser, MockupPhone, CompanyItem },
  async setup() {
    const { isAuthenticated } = useAuth()

    const projectId = Number(useRoute().params.id)
    const initialProject = useProjectStore().project
    if (!initialProject || initialProject?.id !== projectId) await useProjectStore().getProject(projectId)

    const project = useProjectStore().project
    if (!project) return { isAuthenticated }

    useMeta().updateMeta({
      title: project.title,
      description: project.shortDescription,
      image: project.backgroundImage ? getImageUrl(project.backgroundImage) : undefined,
    })

    return {
      isAuthenticated,
    }
  },
  computed: {
    slug(): string {
      return useRoute().params.slug as string
    },
    project(): Project | undefined {
      return useProjectStore().project
    },
    loading(): boolean {
      return useProjectStore().projectLoading
    },
    error(): string | undefined {
      return useProjectStore().projectError
    },
    companies(): Company[] {
      return useCompanyStore().companies
    },
    company(): Company | undefined {
      if (!this.project?.companyId) return undefined

      return this.companies.find((company) => company.id === this.project?.companyId)
    },
    groupedTags(): Record<string, TagModel[]> {
      if (!this.project) return {}

      const tags = this.project.tags

      const groupedAndSorted = tags.reduce((acc: any, tag: TagModel) => {
        const type = (acc[tag.type] = acc[tag.type] || [])
        type.push(tag)
        acc[tag.type].sort((a: TagModel, b: TagModel) => a.title.localeCompare(b.title))
        return acc
      }, {})

      const sortedGroupTitles = Object.keys(groupedAndSorted).sort((a, b) => a.localeCompare(b))
      const sortedGroupedTags = sortedGroupTitles.reduce((acc: any, title: string) => {
        acc[title] = groupedAndSorted[title]
        return acc
      }, {})

      return sortedGroupedTags
    },
    headerClass(): object {
      if (!process.client) return {}

      const oklsh = getComputedStyle(document.getElementsByClassName('title')[0]).getPropertyValue('--tw-prose-headings')
      return {
        [`header-container-${getShadeFromOklsh(oklsh)}`]: true,
      }
    },
    headerStyle(): object {
      if (!this.project?.backgroundImage) return {}

      return {
        backgroundImage: `url(${getImageUrl(this.project.backgroundImage)})`,
      }
    },
  },
  async mounted() {
    if (!this.slug) return
    if (this.project?.slug !== this.slug) await useProjectStore().getProjectBySlug(this.slug)
  },
  beforeUnmount() {
    useProjectStore().project = undefined
  },
})
</script>

<style scoped>
.header-container {
  @apply relative flex h-72 flex-col justify-end bg-cover bg-center py-8;
}

.header-container::after {
  content: '';
  @apply absolute left-0 top-0 block h-full w-full bg-gradient-to-b from-transparent;
}

.header-container-light::after {
  @apply to-gray-400;
}

.header-container-dark::after {
  @apply to-black;
}

.title {
  @apply relative m-0 text-4xl;
}
</style>

<style>
.project-description p {
  @apply mt-0;
}
</style>
