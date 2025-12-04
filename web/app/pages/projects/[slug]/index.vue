<template>
  <div class="flex flex-grow justify-center overflow-hidden">
    <Skeleton v-if="loading" />
    <div v-else-if="project" class="relative flex w-full flex-grow flex-col gap-2 overflow-hidden">
      <!-- TODO: dark/light gradient -->
      <div class="header-container header-container-dark" :style="bannerStyle">
        <div class="prose z-10 flex w-full max-w-full items-center justify-between gap-4 overflow-hidden px-8">
          <div class="flex flex-col gap-x-4 gap-y-2 md:flex-row md:items-center">
            <div class="flex items-center justify-start space-x-2 md:hidden">
              <img
                v-if="project.logoUrl"
                :src="getImageUrl(project.logoUrl)"
                :alt="`${project.title} Logo`"
                class="m-0 h-10 w-10 rounded-full"
              >
              <small class="text-white">{{ project.year }}</small>
            </div>
            <img
              v-if="project.logoUrl"
              :src="getImageUrl(project.logoUrl)"
              :alt="`${project.title} Logo`"
              class="m-0 hidden h-10 w-10 rounded-full md:flex"
            >
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

<script setup lang="ts">
import { computed, onMounted, onBeforeUnmount } from 'vue'
import { useRoute } from 'vue-router'
import { sortBy } from 'lodash-es'
import { ImageType, type Tag as TagModel } from '~/api'
import { useProjectStore } from '~/app/store/ProjectStore'
import { useCompanyStore } from '~/app/store/CompanyStore'
import useMeta from '~/app/composables/useMeta'
import { getImageUrl } from '~/app/utils/image-helper'
import useAuth from '~/app/composables/useAuth'
import Skeleton from '~/app/components/loading/Skeleton.vue'
import Tag from '~/app/components/tags/Tag.vue'
import IconLink from '~/app/components/navigation/IconLink.vue'
import MockupBrowser from '~/app/components/mockup/MockupBrowser.vue'
import MockupPhone from '~/app/components/mockup/MockupPhone.vue'
import CompanyItem from '~/app/components/forms/company/CompanyItem.vue'
import Carousel from '~/app/components/layouts/Carousel.vue'
import ProjectList from '~/app/components/lists/project/ProjectList.vue'

const route = useRoute()
const { isAuthenticated } = useAuth()
const projectStore = useProjectStore()
const companyStore = useCompanyStore()
const { updateMeta } = useMeta()

const slug = computed(() => route.params.slug as string)

const project = computed(() => projectStore.project)
const loading = computed(() => projectStore.projectLoading)
const error = computed(() => projectStore.projectError)
const relatedProjects = computed(() => projectStore.relatedProjects)
const relatedProjectsLoading = computed(() => projectStore.relatedProjectsLoading)
const companies = computed(() => companyStore.companies)

const company = computed(() => {
  if (!project.value?.companyId) return undefined
  return companies.value.find((c) => c.id === project.value?.companyId)
})

const desktopImages = computed(() => {
  if (!project.value) return []
  return sortBy(
    project.value.images.filter((image) => image.type === ImageType.DESKTOP),
    'order',
  ).map((image) => image.image)
})

const mobileImages = computed(() => {
  if (!project.value) return []
  return sortBy(
    project.value.images.filter((image) => image.type === ImageType.MOBILE),
    'order',
  ).map((image) => image.image)
})

const groupedTags = computed(() => {
  if (!project.value) return {}
  const tags = project.value.tags
  const groupedAndSorted = tags.reduce((acc: any, tag: TagModel) => {
    const type = (acc[tag.type] = acc[tag.type] || [])
    type.push(tag)
    acc[tag.type].sort((a: TagModel, b: TagModel) => a.title.localeCompare(b.title))
    return acc
  }, {})
  const sortedGroupTitles = Object.keys(groupedAndSorted).sort((a, b) => a.localeCompare(b))
  return sortedGroupTitles.reduce((acc: any, title: string) => {
    acc[title] = groupedAndSorted[title]
    return acc
  }, {})
})

const bannerStyle = computed(() => {
  if (!project.value?.bannerUrl) return {}
  return {
    backgroundImage: `url(${getImageUrl(project.value.bannerUrl)})`,
  }
})

const headerClass = computed(() => {
  const length = project.value?.tags.length ?? 0
  return {
    'xl:flex-row xl:items-end xl:justify-between': length < 8,
    'xl:flex-col xl:items-start xl:justify-start': length >= 800,
  }
})

onMounted(async () => {
  if (!slug.value) return

  if (!project.value || project.value.slug !== slug.value) {
    await projectStore.getProjectBySlug(slug.value)
  }

  if (project.value) {
    await projectStore.getRelatedProjects(project.value.id)

    updateMeta({
      title: project.value.title,
      description: project.value.shortDescription,
      image: project.value.cardUrl ? getImageUrl(project.value.cardUrl) : undefined,
    })
  }

  if (!companies.value.length) {
    await companyStore.getCompanies()
  }
})

onBeforeUnmount(() => {
  projectStore.project = undefined
})
</script>

<style scoped>
.header-container {
  position: relative;
  display: flex;
  height: 18rem;
  flex-direction: column;
  justify-content: flex-end;
  background-size: cover;
  background-position: center;
  padding-top: 2rem;
  padding-bottom: 2rem;
}

.header-container::after {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  display: block;
  height: 100%;
  width: 100%;
  background-image: linear-gradient(to bottom, transparent, var(--fallback-b1, oklch(var(--b1))));
}

.header-container-light::after {
  --tw-gradient-to: rgb(156 163 175);
}

.header-container-dark::after {
  --tw-gradient-to: rgb(0 0 0);
}

.title {
  position: relative;
  margin: 0;
  font-size: 2.25rem;
  line-height: 2.5rem;
}
</style>

<style>
.project-description p {
  margin-top: 0;
}

.project-description li p,
.project-description ul ul,
.project-description ol ol {
  margin: 0;
}

.project-description ul ul li {
  margin-top: 0.25rem;
}
</style>
