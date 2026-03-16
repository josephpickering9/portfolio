<template>
  <ul class="timeline timeline-vertical timeline-snap-icon max-md:timeline-compact">
    <li v-for="(company, index) in sortedCompanies" :key="company.id" v-reveal="{ delay: index * 30 }" class="reveal">
      <div class="timeline-middle">
        <div class="flex h-8 w-8 items-center justify-center rounded-full bg-base-100 p-1 shadow-sm ring-1 ring-base-content/10">
          <NuxtImg v-if="company.logo" :src="getImageUrl(company.logo)" :alt="`${company.name} Logo`" placeholder format="webp" class="h-full w-full object-contain" />
          <Icon v-else name="mdi:domain" class="h-4 w-4 text-base-content/50" />
        </div>
      </div>
      <div
        class="mb-10 px-4"
        :class="[
          index % 2 === 0 ? 'timeline-start' : 'timeline-end',
          'timeline-box text-base w-full border-base-content/10 bg-base-100 shadow-sm'
        ]"
      >
        <div class="flex flex-col gap-2 p-3">
          <div class="flex items-start justify-between gap-2">
            <div class="flex flex-col gap-0.5">
              <h3 class="text-lg font-bold text-primary leading-tight">
                <a v-if="company.website" :href="company.website" target="_blank" class="hover:underline">{{ company.name }}</a>
                <span v-else>{{ company.name }}</span>
              </h3>
              <div class="flex items-center gap-1.5 flex-wrap">
                <time class="font-mono text-sm text-base-content/50">{{ getDateRange(company) }}</time>
                <span v-if="getDuration(company)" class="text-sm font-mono text-base-content/50">({{ getDuration(company) }})</span>
              </div>
            </div>
            <FormButton
              v-if="isAuthenticated"
              size="xs"
              label="Edit"
              icon="material-symbols:edit"
              :href="`/companies/${company.id}/update`"
            />
          </div>

          <div
            class="company-description prose max-w-none
                   prose-p:text-base-content/80 prose-p:leading-relaxed prose-p:mb-2
                   prose-ul:mt-1 prose-ul:text-base-content/75
                   prose-li:my-0.5 prose-li:leading-relaxed"
            v-html="company.description"
          />

          <div v-if="getCompanyProjects(company.id).length" class="mt-1 flex flex-wrap gap-1.5">
            <NuxtLink
              v-for="project in getCompanyProjects(company.id)"
              :key="project.id"
              :to="`/projects/${project.slug}`"
              class="badge badge-neutral badge-sm gap-1 no-underline hover:badge-primary"
            >
              {{ project.title }}
            </NuxtLink>
            <NuxtLink
              :to="`/projects?company=${company.id}`"
              class="badge badge-ghost badge-sm gap-1 no-underline hover:badge-primary opacity-60 hover:opacity-100"
            >
              View all
            </NuxtLink>
          </div>
        </div>
      </div>
      <hr v-if="index !== sortedCompanies.length - 1" class="bg-base-content/10" >
    </li>
  </ul>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { compareDesc, parseISO } from 'date-fns'
import type { Company, Project  } from '@api'
import { getImageUrl } from '~/utils/image-helper'
import { getDateRange, getDuration } from '~/utils/date-helper'
import useAuth from '~/composables/useAuth'
import FormButton from '~/components/ui/form/FormButton.vue'

const props = defineProps<{
  companies: Company[]
  projects: Project[]
}>()

const { isAuthenticated } = useAuth()

const sortedCompanies = computed(() => {
  return [...props.companies].sort((a, b) => {
    // Sort by start date descending (newest first)
    // Handle null dates by placing them at the end
    if (!a.startDate && !b.startDate) return 0
    if (!a.startDate) return 1
    if (!b.startDate) return -1
    return compareDesc(parseISO(a.startDate), parseISO(b.startDate))
  })
})

function getCompanyProjects(companyId: string): Project[] {
  return props.projects.filter(p => p.companyId === companyId)
}

</script>

<style scoped>
.company-description {
  text-align: left;
}

.company-description :deep(ul) {
  border-top: 1px solid oklch(var(--bc) / 0.1);
  padding-top: 0.5rem;
  margin-top: 0.5rem;
  padding-left: 1.25rem;
  list-style-type: disc;
}

.company-description :deep(li) {
  padding-left: 0.25rem;
}

.company-description :deep(p:has(+ ul)) {
  margin-bottom: 0;
}
</style>
