<template>
  <ul class="timeline timeline-vertical timeline-snap-icon max-md:timeline-compact">
    <li v-for="(company, index) in sortedCompanies" :key="company.id">
      <div class="timeline-middle">
        <div class="flex h-8 w-8 items-center justify-center rounded-full bg-base-100 p-1 shadow-sm ring-1 ring-base-content/10">
          <NuxtImg v-if="company.logo" :src="getImageUrl(company.logo)" :alt="`${company.name} Logo`" placeholder format="webp" class="h-full w-full object-contain" />
          <Icon v-else name="mdi:domain" class="h-4 w-4 text-base-content/50" />
        </div>
      </div>
      <div 
        class="mb-10 px-4"
        :class="[
          index % 2 === 0 ? 'timeline-start md:text-end' : 'timeline-end',
          'timeline-box w-full border-base-content/10 bg-base-100 shadow-sm'
        ]"
      >
        <div class="flex flex-col gap-1 p-2">
          <time class="font-mono text-xs italic text-base-content/60">{{ getDateRange(company) }}</time>
          <div class="flex items-center gap-2" :class="{ 'md:flex-row-reverse': index % 2 === 0 }">
             <h3 class="text-lg font-bold text-primary">
                <a v-if="company.website" :href="company.website" target="_blank" class="hover:underline">{{ company.name }}</a>
                <span v-else>{{ company.name }}</span>
             </h3>
          </div>
          <div class="text-sm" v-html="company.description" />

          <div v-if="getCompanyProjects(company.id).length" class="mt-2 flex flex-wrap gap-2" :class="{ 'justify-end': index % 2 === 0 }">
             <NuxtLink 
                v-for="project in getCompanyProjects(company.id)" 
                :key="project.id" 
                :to="`/projects/${project.slug}`"
                class="badge badge-neutral gap-1 no-underline hover:badge-primary"
             >
                {{ project.title }}
             </NuxtLink>
             <NuxtLink
                :to="`/projects?company=${company.id}`"
                class="badge badge-ghost gap-1 no-underline hover:badge-primary opacity-60 hover:opacity-100"
             >
                View all
             </NuxtLink>
          </div>
          
           <div class="mt-2 flex flex-wrap gap-2" :class="{ 'md:justify-end': index % 2 === 0 }">
             <FormButton
                v-if="isAuthenticated"
                size="xs"
                label="Edit"
                icon="material-symbols:edit"
                :href="`/companies/${company.id}/update`"
              />
           </div>
        </div>
      </div>
      <hr v-if="index !== sortedCompanies.length - 1" class="bg-base-content/10" >
    </li>
  </ul>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { compareDesc, format, parseISO } from 'date-fns'
import type { Company, Project  } from '@api'
import { getImageUrl } from '~/utils/image-helper'
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

function getDateRange(company: Company): string {
  const start = company.startDate ? format(parseISO(company.startDate), 'MMM yyyy') : ''
  const end = company.endDate ? format(parseISO(company.endDate), 'MMM yyyy') : 'Present'

  if (!start) return ''
  return `${start} - ${end}`
}
</script>
