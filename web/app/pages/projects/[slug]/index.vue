<template>
  <div class="mx-auto flex w-full max-w-300 flex-col gap-16 px-7 pt-16 pb-24">
    <template v-if="loading">
      <div class="flex flex-col gap-5.5" aria-hidden="true">
        <div class="skeleton-pulse h-3.5 w-28 rounded-md bg-surface2" />
        <div class="skeleton-pulse h-20 w-3/4 rounded-md bg-surface2" />
        <div class="skeleton-pulse h-5 w-1/2 rounded-md bg-surface2" />
      </div>
      <div class="skeleton-pulse aspect-video rounded-2xl border border-line bg-surface" />
    </template>

    <LoadError v-else-if="projectStore.projectError && !project" message="Couldn't load this project. Check your connection and try again." @retry="load" />

    <template v-else-if="project">
      <ProjectHeader
        :project="project"
        :eyebrow="eyebrow"
        :back-to="isThisSite ? '/' : '/projects'"
        :back-label="isThisSite ? 'Selected work' : 'All projects'"
      />
      <ProjectMockup :project="project" />

      <WorkExperienceCaseStudy v-if="isThisSite" />
      <ProjectCaseStudy v-else :project="project" />

      <NuxtLink v-if="isThisSite" to="/" class="self-start text-[15px] text-accent">← Back to selected work</NuxtLink>
      <NextProject v-else-if="nextProject" :project="nextProject" />
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useProjectStore } from '~/store/ProjectStore'
import { useCompanyStore } from '~/store/CompanyStore'
import useMeta from '~/composables/useMeta'
import { getImageUrl } from '~/utils/image-helper'
import { formatDateRange } from '~/utils/date-helper'
import { getNextProject } from '~/utils/project-helper'
import LoadError from '~/components/feedback/LoadError.vue'
import ProjectHeader from '~/components/project/details/ProjectHeader.vue'
import ProjectMockup from '~/components/project/details/ProjectMockup.vue'
import ProjectCaseStudy from '~/components/project/details/ProjectCaseStudy.vue'
import WorkExperienceCaseStudy from '~/components/project/details/WorkExperienceCaseStudy.vue'
import NextProject from '~/components/project/details/NextProject.vue'

const THIS_SITE_SLUG = 'work-experience'
const PERSONAL_COMPANY = 'Personal'
const HTTP_NOT_FOUND = 404

const route = useRoute()
const projectStore = useProjectStore()
const companyStore = useCompanyStore()
const { updateMeta } = useMeta()

const slug = computed(() => route.params['slug'] as string)
const project = computed(() => (projectStore.project?.slug === slug.value ? projectStore.project : undefined))
const loading = computed(() => projectStore.projectLoading || (!project.value && !projectStore.projectError))
const isThisSite = computed(() => slug.value === THIS_SITE_SLUG)
const nextProject = computed(() => getNextProject(projectStore.projects, slug.value))

const eyebrow = computed(() => {
  const current = project.value
  if (!current) return ''

  const companyName = companyStore.companies.find((company) => company.id === current.companyId)?.name
  const owner = !companyName || companyName === PERSONAL_COMPANY ? 'Personal project' : companyName
  if (isThisSite.value) return `Case study / ${owner}`
  return [owner, formatDateRange(current.startDate, current.endDate)].filter(Boolean).join(' / ')
})

async function load() {
  const loaded = await projectStore.getProjectBySlug(slug.value)
  if (projectStore.projectForm.status === HTTP_NOT_FOUND) {
    showError({ statusCode: HTTP_NOT_FOUND, statusMessage: 'Project not found' })
    return
  }
  if (!loaded || projectStore.projectError) return

  updateMeta({
    title: loaded.title,
    description: loaded.shortDescription,
    image: loaded.cardUrl ? getImageUrl(loaded.cardUrl) : undefined,
  })
}

onMounted(() => {
  if (!project.value) load()
  if (!projectStore.projectsForm.data) projectStore.getProjects()
  if (!companyStore.companies.length) companyStore.getCompanies()
})

watch(slug, (value) => {
  if (value) load()
})

onBeforeUnmount(() => {
  projectStore.projectForm.data = undefined
})
</script>
