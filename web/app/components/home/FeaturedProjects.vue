<template>
  <section class="mx-auto w-full max-w-300 px-7 py-10">
    <div class="mb-9 flex flex-wrap items-baseline justify-between gap-4 border-t border-line pt-7">
      <h2 class="m-0 font-display text-[44px] font-normal tracking-[-0.01em]">Selected work</h2>
      <NuxtLink to="/projects" class="text-sm text-muted">All projects →</NuxtLink>
    </div>

    <LoadError v-if="projectStore.projectsError && !projectStore.projectsLoading" @retry="load" />

    <div v-else class="grid grid-cols-[repeat(auto-fit,minmax(min(100%,460px),1fr))] gap-7">
      <template v-if="loading">
        <Skeleton v-for="n in FEATURED_SLUGS.length" :key="n" :type="LoadingType.CARD" />
      </template>
      <template v-else>
      <NuxtLink
        v-for="(project, index) in featured"
        :key="project.id"
        :to="`/projects/${project.slug}`"
        class="flex flex-col gap-5 text-text transition-transform duration-350 ease-lift hover:-translate-y-1.5 hover:text-text"
      >
        <MockupBrowser>
          <NuxtImg
v-if="getCardImage(project)"
            format="webp"
            :src="getImageUrl(getCardImage(project))"
            :alt="project.title"
            width="556"
            height="348"
            fit="cover"
            :modifiers="{ position: isLeftAligned(project) ? 'left top' : 'top' }"
            sizes="100vw lg:556px"
            loading="lazy"
            class="block aspect-16/10 w-full object-cover"
            :class="isLeftAligned(project) ? 'object-left-top' : 'object-top'"
          />
          <div v-else class="aspect-16/10" />
        </MockupBrowser>
        <div class="flex gap-4.5">
          <span class="pt-1.5 font-mono text-[13px] text-accent">{{ padNumber(index + 1) }}</span>
          <div class="flex flex-col gap-2">
            <h3 class="m-0 text-2xl font-semibold tracking-[-0.01em]">{{ project.title }}</h3>
            <p class="m-0 leading-[1.55] text-muted">{{ project.shortDescription }}</p>
            <span class="font-mono text-xs text-muted">{{ getTagSummary(project) }}</span>
          </div>
        </div>
      </NuxtLink>
      </template>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import type { Project } from '@api'
import { useProjectStore } from '~/store/ProjectStore'
import { LoadingType } from '~/types/LoadingType'
import { getImageUrl } from '~/utils/image-helper'
import { getCardImage, getTagSummary, padNumber, pickBySlug } from '~/utils/project-helper'
import MockupBrowser from '~/components/mockup/MockupBrowser.vue'
import Skeleton from '~/components/feedback/loading/Skeleton.vue'
import LoadError from '~/components/feedback/LoadError.vue'

const FEATURED_SLUGS = ['pulse', 'ai-chatbot', 'data-extractor', 'glasgow-retina']
const LEFT_ALIGNED_SLUGS = ['pulse']

const projectStore = useProjectStore()

const loading = computed(() => projectStore.projectsLoading || (!projectStore.projectsForm.data && !projectStore.projectsError))
const featured = computed(() => pickBySlug(projectStore.projects, FEATURED_SLUGS))

function isLeftAligned(project: Project): boolean {
  return LEFT_ALIGNED_SLUGS.includes(project.slug)
}

function load() {
  projectStore.getProjects()
}

onMounted(() => {
  if (!projectStore.projectsForm.data) load()
})
</script>
