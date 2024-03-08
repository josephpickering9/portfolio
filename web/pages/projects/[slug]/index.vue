<template>
  <div class="flex flex-grow justify-center overflow-hidden">
    <Skeleton v-if="loading" />
    <div v-else-if="project" class="relative flex w-full flex-grow flex-col gap-2 overflow-hidden">
      <!-- TODO: dark/light gradient -->
      <div class="header-container header-container-dark" :style="bannerStyle">
        <div class="prose z-10 flex w-full max-w-full items-center justify-between gap-4 overflow-hidden px-8">
          <div class="flex flex-col gap-x-4 gap-y-2 md:flex-row md:items-center">
            <div class="flex items-center justify-start space-x-2 md:hidden">
              <img v-if="project.logoUrl" :src="getImageUrl(project.logoUrl)" class="m-0 h-10 w-10 rounded-full" />
              <small class="text-white">{{ project.year }}</small>
            </div>
            <img
              v-if="project.logoUrl"
              :src="getImageUrl(project.logoUrl)"
              class="m-0 hidden h-10 w-10 rounded-full md:flex"
            />
            <h1 ref="title" class="title text-gray-400">{{ project.title }}</h1>
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
        <div class="flex flex-col gap-6" :class="headerClass">
          <div v-if="Object.keys(groupedTags).length > 0" class="project-tags flex flex-wrap items-center gap-x-4 gap-y-1">
            <div v-for="(typeTags, type) in groupedTags" :key="type">
              <small class="m-0 italic">{{ type }}</small>
              <div class="flex flex-wrap gap-2">
                <Tag v-for="(tag, index) in typeTags" :key="index" :tag="tag" />
              </div>
            </div>
          </div>
          <div class="project-links flex flex-wrap items-center gap-x-4 gap-y-2">
            <IconLink
              v-if="project.website"
              :to="project.website"
              label="Website"
              icon="material-symbols:globe"
              icon-size="1.4em"
            />
            <div class="flex items-center gap-2">
              <div v-for="(repository, index) in project.repositories" :key="index" class="flex items-center gap-2">
                <IconLink
                  v-if="index === 0"
                  :to="repository.url"
                  :label="repository.title"
                  icon="mdi:github"
                  icon-size="1.4em"
                />
                <IconLink v-else :to="repository.url" :label="repository.title" />
                <span v-if="index !== project.repositories.length - 1" class="text-gray-400">/</span>
              </div>
            </div>
            <CompanyItem v-if="company" :company="company" :show-link="true" />
          </div>
        </div>

        <h2 class="m-0 text-xl font-normal italic">{{ project.shortDescription }}</h2>
        <div class="project-description" v-html="project.description" />

        <Carousel
          v-if="desktopImages.length"
          v-model="desktopImages"
          title="Desktop"
          width="1007"
          height="508"
          :show-arrows="true"
        />
        <Carousel
          v-if="mobileImages.length"
          v-model="mobileImages"
          title="Mobile"
          width="335"
          height="673"
          :show-arrows="true"
        />

        <div
          v-if="project.website && project.showMockup"
          class="-mx-4 flex flex-col gap-4 rounded-box bg-neutral p-4 md:mx-0"
        >
          <h3 class="m-0">Mockup</h3>
          <div class="flex flex-col items-center gap-4 md:flex-row">
            <MockupBrowser class="hidden md:block" :url="project.website" />
            <MockupPhone :url="project.website" />
          </div>
        </div>
      </div>
      <div class="mx-auto mt-2 flex w-full max-w-7xl flex-col justify-center gap-4 overflow-hidden px-8">
        <div v-if="relatedProjects.length" class="-mx-4 flex flex-col gap-4 rounded-box bg-neutral p-4 md:mx-0">
          <div class="prose"><h3 class="m-0">Related Projects</h3></div>
          <div class="mx-auto flex items-center justify-center">
            <ProjectList :set-projects="relatedProjects" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts">
import { useRoute } from 'vue-router'
import { sortBy } from 'lodash-es'
import { ImageType, type Company, type Project, type Tag as TagModel } from '../../../api'
import { useProjectStore } from '../../../store/ProjectStore'
import Skeleton from '../../../components/loading/Skeleton.vue'
import Tag from '../../../components/tags/Tag.vue'
import IconLink from '../../../components/navigation/IconLink.vue'
import MockupBrowser from '../../../components/mockup/MockupBrowser.vue'
import MockupPhone from '../../../components/mockup/MockupPhone.vue'
import CompanyItem from '../../../components/forms/company/CompanyItem.vue'
import { useCompanyStore } from '../../../store/CompanyStore'
import useMeta from '../../../composables/useMeta'
import { getImageUrl } from '../../../utils/image-helper'
import useAuth from '../../../composables/useAuth'
import Carousel from '../../../components/layouts/Carousel.vue'
import ProjectList from '../../../components/lists/project/ProjectList.vue'
import { defineNuxtComponent } from '#app'

export default defineNuxtComponent({
  name: 'Project',
  components: { Skeleton, Tag, IconLink, MockupBrowser, MockupPhone, CompanyItem, Carousel, ProjectList },
  async setup() {
    const { isAuthenticated } = useAuth()

    const projectSlug = useRoute().params.slug as string
    const initialProject = useProjectStore().project
    if (!initialProject || initialProject?.slug !== projectSlug) await useProjectStore().getProjectBySlug(projectSlug)

    const project = useProjectStore().project
    if (!project) return { isAuthenticated }

    await useProjectStore().getRelatedProjects(project.id)

    const companies = useCompanyStore().companies
    if (!companies.length) await useCompanyStore().getCompanies()

    useMeta().updateMeta({
      title: project.title,
      description: project.shortDescription,
      image: project.cardUrl ? getImageUrl(project.cardUrl) : undefined,
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
    relatedProjects(): Project[] {
      return useProjectStore().relatedProjects
    },
    relatedProjectsLoading(): boolean {
      return useProjectStore().relatedProjectsLoading
    },
    companies(): Company[] {
      return useCompanyStore().companies
    },
    company(): Company | undefined {
      if (!this.project?.companyId) return undefined

      return this.companies.find((company) => company.id === this.project?.companyId)
    },
    desktopImages(): string[] {
      if (!this.project) return []

      return sortBy(
        this.project.images.filter((image) => image.type === ImageType.DESKTOP),
        'order',
      ).map((image) => image.image)
    },
    mobileImages(): string[] {
      if (!this.project) return []

      return sortBy(
        this.project.images.filter((image) => image.type === ImageType.MOBILE),
        'order',
      ).map((image) => image.image)
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
    bannerStyle(): object {
      if (!this.project?.bannerUrl) return {}

      return {
        backgroundImage: `url(${getImageUrl(this.project.bannerUrl)})`,
      }
    },
    headerClass(): object {
      const length = this.project?.tags.length ?? 0

      return {
        'xl:flex-row xl:items-end xl:justify-between': length < 8,
        'xl:flex-col xl:items-start xl:justify-start': length >= 800,
      }
    },
  },
  async mounted() {
    if (!this.slug) return
    if (this.project?.slug !== this.slug) await useProjectStore().getProjectBySlug(this.slug)
  },
  methods: {
    getImageUrl,
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

.project-description li p,
.project-description ul ul,
.project-description ol ol {
  @apply m-0;
}

.project-description ul ul li {
  @apply mt-1;
}
</style>
