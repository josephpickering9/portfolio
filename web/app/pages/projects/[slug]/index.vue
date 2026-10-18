<template>
  <div class="mx-auto flex w-full max-w-300 flex-col gap-16 px-7 pt-16 pb-24">
    <template v-if="loading">
      <div class="flex flex-col gap-5.5" aria-hidden="true" data-testid="skeleton">
        <div class="skeleton-pulse h-3.5 w-28 rounded-md bg-surface2" />
        <div class="skeleton-pulse h-20 w-3/4 rounded-md bg-surface2" />
        <div class="skeleton-pulse h-5 w-1/2 rounded-md bg-surface2" />
      </div>
      <div class="skeleton-pulse aspect-video rounded-2xl border border-line bg-surface" />
    </template>

    <LoadError
      v-else-if="projectStore.projectError && !project"
      message="Couldn't load this project. Check your connection and try again."
      @retry="refresh"
    />

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

      <NuxtLink v-if="isThisSite" to="/" class="self-start text-[15px] text-accent" data-testid="back-to-selected-work">
        ← Back to selected work
      </NuxtLink>
      <NextProject v-else-if="nextProject" :project="nextProject" />
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed, onBeforeUnmount, watch } from 'vue'
import { useRoute } from 'vue-router'
import type { Project } from '@api'
import { createError, showError, useAsyncData } from '#imports'
import { useProjectStore } from '~/store/ProjectStore'
import { useCompanyStore } from '~/store/CompanyStore'
import useMeta from '~/composables/useMeta'
import useProjectsData from '~/composables/useProjectsData'
import useCompaniesData from '~/composables/useCompaniesData'
import { useImageUrl } from '~/composables/useImageUrl'
import { getNextProject, getProjectEyebrow } from '~/utils/project-helper'
import { HTTP_NOT_FOUND } from '~/types/errors'
import LoadError from '~/components/feedback/LoadError.vue'
import ProjectHeader from '~/components/project/details/ProjectHeader.vue'
import ProjectMockup from '~/components/project/details/ProjectMockup.vue'
import ProjectCaseStudy from '~/components/project/details/ProjectCaseStudy.vue'
import WorkExperienceCaseStudy from '~/components/project/details/WorkExperienceCaseStudy.vue'
import NextProject from '~/components/project/details/NextProject.vue'

const THIS_SITE_SLUG = 'work-experience'

const route = useRoute()
const projectStore = useProjectStore()
const companyStore = useCompanyStore()
const { updateMeta } = useMeta()
const { getImageUrl } = useImageUrl()

const slug = computed(() => String(route.params['slug']))

const projectsData = useProjectsData()
const companiesData = useCompaniesData()
const projectData = useAsyncData(() => `project:${slug.value}`, loadProject, { lazy: true })

if (import.meta.server) await Promise.all([projectData, projectsData, companiesData])

const { error, refresh } = projectData

const project = computed(() => (projectStore.project?.slug === slug.value ? projectStore.project : undefined))
const loading = computed(() => projectStore.projectLoading || (!project.value && !projectStore.projectError))
const isThisSite = computed(() => slug.value === THIS_SITE_SLUG)
const nextProject = computed(() => getNextProject(projectStore.projects, slug.value))
const eyebrow = computed(() =>
  project.value ? getProjectEyebrow(project.value, companyStore.companies, isThisSite.value) : '',
)

async function loadProject(): Promise<Project | null> {
  const loaded = await projectStore.getProjectBySlug(slug.value)
  if (projectStore.projectNotFound) throw createError({ statusCode: HTTP_NOT_FOUND, statusMessage: 'Project not found' })
  return loaded ?? null
}

updateMeta(() => ({
  title: project.value?.title,
  description: project.value?.shortDescription,
  image: project.value?.cardUrl ? getImageUrl(project.value.cardUrl) : undefined,
}))

onBeforeUnmount(() => projectStore.clearProject(slug.value))

watch(
  error,
  (value) => {
    if (value?.statusCode === HTTP_NOT_FOUND) showError(value)
  },
  { immediate: true },
)
</script>
