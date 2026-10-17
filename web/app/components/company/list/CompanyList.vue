<template>
  <div class="mx-auto flex w-full max-w-300 flex-col gap-12 px-7 pt-16 pb-24">
    <div class="flex flex-wrap items-end justify-between gap-6">
      <PageHeader
        eyebrow="Companies"
        title="Timeline of companies and projects"
        lede="Companies I've worked with, and the projects I delivered at each."
      />
      <ClientOnly>
        <NuxtLink
          v-if="isAuthenticated"
          to="/companies/new"
          class="flex items-center gap-2 rounded-full bg-accent px-4 py-2.25 text-sm font-medium text-accent-ink hover:text-accent-ink"
        >
          <Icon name="heroicons:plus" />
          New company
        </NuxtLink>
      </ClientOnly>
    </div>

    <LoadError
      v-if="companyStore.companiesError && !companyStore.companiesLoading"
      message="Couldn't load companies. Check your connection and try again."
      @retry="load"
    />
    <CompanyTimeline v-else :companies="companyStore.companies" :projects="projectStore.projects" :loading="loading" />
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useCompanyStore } from '~/store/CompanyStore'
import { useProjectStore } from '~/store/ProjectStore'
import useAuth from '~/composables/useAuth'
import PageHeader from '~/components/ui/layout/PageHeader.vue'
import LoadError from '~/components/feedback/LoadError.vue'
import CompanyTimeline from '~/components/company/timeline/CompanyTimeline.vue'

const { isAuthenticated } = useAuth()
const companyStore = useCompanyStore()
const projectStore = useProjectStore()

const loading = computed(
  () => companyStore.companiesLoading || (!companyStore.companiesForm.data && !companyStore.companiesError),
)

function load() {
  companyStore.getCompanies()
  if (!projectStore.projectsForm.data) projectStore.getProjects()
}

onMounted(load)
</script>
