<template>
  <CaseStudySection
    v-for="(section, index) in sections"
    :key="section.key"
    :label="`${padNumber(index + 1)} / ${section.title}`"
    :stacked="section.key === 'gallery'"
  >
    <div v-if="section.key === 'overview'" class="flex flex-col gap-8">
      <div class="rich-text max-w-170 text-[17px] leading-[1.7]" data-testid="project-description" v-html="project.description" />
      <ProjectStats :stats="project.stats" />
    </div>
    <div v-else-if="section.key === 'decisions'" class="max-w-170 rounded-card bg-surface2 px-6 py-5.5 text-[17px] leading-[1.7] text-text">
      {{ project.keyDecision }}
    </div>
    <p v-else-if="section.key === 'problem'" class="m-0 max-w-170 text-[17px] leading-[1.7] text-muted">{{ project.problemStatement }}</p>
    <p v-else-if="section.key === 'outcome'" class="m-0 max-w-170 text-[17px] leading-[1.7] text-muted">{{ project.outcome }}</p>
    <ProjectTags v-else-if="section.key === 'stack'" :tags="project.tags" />
    <ProjectGallery
      v-else-if="section.key === 'gallery'"
      :title="project.title"
      :desktop-images="desktopImages"
      :mobile-images="mobileImages"
      :framed="project.showMockup"
    />
  </CaseStudySection>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { ImageType } from '@api'
import type { Project } from '@api'
import { getImagesByType } from '~/utils/project-helper'
import { padNumber } from '~/utils/string-helper'
import CaseStudySection from './CaseStudySection.vue'
import ProjectStats from './ProjectStats.vue'
import ProjectTags from './ProjectTags.vue'
import ProjectGallery from './ProjectGallery.vue'

type SectionKey = 'overview' | 'problem' | 'decisions' | 'outcome' | 'stack' | 'gallery'

interface Props {
  project: Project
}

const props = defineProps<Props>()

const desktopImages = computed(() => getImagesByType(props.project, ImageType.DESKTOP).map((image) => image.image))
const mobileImages = computed(() => getImagesByType(props.project, ImageType.MOBILE).map((image) => image.image))

const sections = computed(() => {
  const candidates: { key: SectionKey; title: string; visible: boolean }[] = [
    { key: 'overview', title: 'Overview', visible: Boolean(props.project.description) },
    { key: 'problem', title: 'Problem', visible: Boolean(props.project.problemStatement) },
    { key: 'decisions', title: 'Decisions', visible: Boolean(props.project.keyDecision) },
    { key: 'outcome', title: 'Outcome', visible: Boolean(props.project.outcome) },
    { key: 'stack', title: 'Stack', visible: props.project.tags.length > 0 },
    { key: 'gallery', title: 'Gallery', visible: desktopImages.value.length + mobileImages.value.length > 0 },
  ]
  return candidates.filter((section) => section.visible)
})
</script>
