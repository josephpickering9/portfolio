<template>
  <div class="flex flex-grow justify-center overflow-hidden">
    <Skeleton v-if="loading" />
    <div v-else-if="project" class="relative flex w-full flex-grow flex-col gap-2 overflow-hidden">
      <ProjectHeader :project="project" :company="company" />

      <div class="prose mx-auto flex w-full max-w-7xl flex-col justify-center gap-10 overflow-hidden px-6 py-8 md:mt-0 md:px-8">
        <ProjectTags :tags="project.tags" />

        <div class="divider m-0"/>

        <h2 class="m-0 text-2xl font-light italic leading-relaxed text-base-content/80">{{ project.shortDescription }}</h2>
        <div class="project-description text-lg leading-relaxed" v-html="project.description" />

        <ProjectGallery
          v-model:desktop-images="desktopImages"
          v-model:mobile-images="mobileImages"
        />

        <ProjectMockup
          v-if="project.showMockup"
          :website-url="project.website || undefined"
        />
      </div>
      <RelatedProjects :projects="relatedProjects" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, onBeforeUnmount } from 'vue'
import { useRoute } from 'vue-router'
import { sortBy } from 'lodash-es'
import { ImageType } from '@api'
import { useProjectStore } from '~/store/ProjectStore'
import { useCompanyStore } from '~/store/CompanyStore'
import useMeta from '~/composables/useMeta'
import { getImageUrl } from '~/utils/image-helper'
import Skeleton from '~/components/feedback/loading/Skeleton.vue'
import ProjectHeader from '~/components/project/details/ProjectHeader.vue'
import ProjectTags from '~/components/project/details/ProjectTags.vue'
import ProjectGallery from '~/components/project/details/ProjectGallery.vue'
import ProjectMockup from '~/components/project/details/ProjectMockup.vue'
import RelatedProjects from '~/components/project/details/RelatedProjects.vue'

const route = useRoute()
const projectStore = useProjectStore()
const companyStore = useCompanyStore()
const { updateMeta } = useMeta()

const slug = computed(() => route.params.slug as string)

const project = computed(() => projectStore.project)
const loading = computed(() => projectStore.projectLoading)
const relatedProjects = computed(() => projectStore.relatedProjects)
const companies = computed(() => companyStore.companies)

const company = computed(() => {
  if (!project.value?.companyId) return undefined
  return companies.value.find((c) => c.id === project.value?.companyId)
})

const desktopImages = computed({
  get: () => {
    if (!project.value) return []
    return sortBy(
      project.value.images.filter((image) => image.type === ImageType.DESKTOP),
      'order',
    ).map((image) => image.image)
  },
  set: () => {
    // Read-only computed for now as we don't update images from here
  }
})

const mobileImages = computed({
  get: () => {
    if (!project.value) return []
    return sortBy(
      project.value.images.filter((image) => image.type === ImageType.MOBILE),
      'order',
    ).map((image) => image.image)
  },
  set: () => {
    // Read-only computed for now as we don't update images from here
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
