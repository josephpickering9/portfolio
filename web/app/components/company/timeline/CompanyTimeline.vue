<template>
  <ol class="m-0 ml-1.5 list-none border-l border-line p-0 pt-1">
    <template v-if="loading">
      <li v-for="n in SKELETON_COUNT" :key="n" class="pb-13 pl-9">
        <Skeleton />
      </li>
    </template>
    <li v-for="company in sortedCompanies" v-else :key="company.id" class="relative flex flex-col gap-3.5 pb-13 pl-9">
      <span
        class="absolute top-4 -left-1.75 size-3.25 rounded-full border-2 border-accent bg-bg shadow-[0_0_16px_var(--glow)]"
        aria-hidden="true"
      />
      <div class="flex flex-wrap items-center gap-4">
        <div class="flex size-12 shrink-0 items-center justify-center overflow-hidden rounded-full border border-line bg-surface2">
          <NuxtImg
v-if="company.logo"
            format="webp"
            :src="getImageUrl(company.logo)"
            :alt="company.name"
            width="48"
            height="48"
            fit="cover"
            class="size-full object-cover"
          />
          <Icon v-else name="mdi:domain" size="22" class="text-muted" />
        </div>
        <div class="flex flex-col gap-1">
          <component
            :is="company.website ? 'a' : 'span'"
            :href="company.website || undefined"
            :target="company.website ? '_blank' : undefined"
            :rel="company.website ? 'noopener noreferrer' : undefined"
            class="font-display text-[32px] leading-none text-text"
          >
            {{ company.name }}
          </component>
          <span class="font-mono text-[13px] text-muted">{{ formatDateRange(company.startDate, company.endDate) || UNDATED_LABEL }}</span>
        </div>
        <ClientOnly>
          <NuxtLink v-if="isAuthenticated" :to="`/companies/${company.id}/update`" class="ml-auto text-sm text-muted">Edit</NuxtLink>
        </ClientOnly>
      </div>
      <div class="rich-text max-w-170 leading-[1.7]" v-html="company.description" />
      <div v-if="projectsByCompany(company.id).length" class="flex flex-wrap gap-2">
        <NuxtLink
          v-for="project in projectsByCompany(company.id)"
          :key="project.id"
          :to="`/projects/${project.slug}`"
          class="rounded-full bg-surface2 px-3 py-1.5 text-[13px] text-text transition-colors duration-200 hover:bg-accent hover:text-accent-ink"
        >
          {{ project.title }}
        </NuxtLink>
      </div>
    </li>
  </ol>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { Company, Project } from '@api'
import { getImageUrl } from '~/utils/image-helper'
import { formatDateRange } from '~/utils/date-helper'
import { sortCompaniesNewestFirst } from '~/utils/company-helper'
import useAuth from '~/composables/useAuth'
import Skeleton from '~/components/feedback/loading/Skeleton.vue'

const SKELETON_COUNT = 3
const UNDATED_LABEL = 'Side projects and freelance work'

const props = defineProps<{
  companies: Company[]
  projects: Project[]
  loading: boolean
}>()

const { isAuthenticated } = useAuth()

const sortedCompanies = computed(() => sortCompaniesNewestFirst(props.companies))

function projectsByCompany(companyId: string): Project[] {
  return props.projects.filter((project) => project.companyId === companyId)
}
</script>
