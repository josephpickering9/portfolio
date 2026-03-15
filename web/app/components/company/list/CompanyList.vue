<template>
  <ListLayout
    title="Companies"
    :loading="loading"
    :is-empty="filteredCompanies.length === 0"
    description="Timeline of companies and projects"
    empty-title="No companies found"
    empty-description="Try searching for something else"
    max-width="max-w-7xl"
  >
    <template #actions>
      <ClientOnly>
        <FormButton
          v-if="isAuthenticated"
          label="Add Company"
          icon="heroicons:plus"
          type="primary"
          size="sm"
          href="/companies/new"
          :disabled="loading"
          class="shadow-lg shadow-primary/20"
        />
      </ClientOnly>
    </template>

    <template #filters>
      <div class="group relative">
        <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
          <Icon name="heroicons:magnifying-glass" class="h-5 w-5 text-base-content/40 group-hover:text-primary" />
        </div>
        <TextInput
          v-model="search"
          class="w-full md:max-w-64"
          size="sm"
          placeholder="Search companies..."
          :disabled="loading"
        />
      </div>
    </template>

    <CompanyTimeline :companies="filteredCompanies" :projects="projects" />
  </ListLayout>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { isEmpty } from 'lodash-es'
import { useQuerySync } from '~/composables/useQuerySync'
import { useCompanyStore } from '~/store/CompanyStore'
import { useProjectStore } from '~/store/ProjectStore'
import type { Company } from '@api'
import useAuth from '~/composables/useAuth'
import ListLayout from '~/components/ui/layout/ListLayout.vue'
import TextInput from '~/components/ui/input/TextInput.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import CompanyTimeline from '~/components/company/timeline/CompanyTimeline.vue'

const { isAuthenticated } = useAuth()
const companyStore = useCompanyStore()
const projectStore = useProjectStore()

const initialLoad = ref(true)
const search = ref('')

useQuerySync(search, { key: 'search', initialValue: '' })

const companies = computed((): Company[] => {
  return companyStore.companies
})

const projects = computed(() => {
  return projectStore.projects
})

const loading = computed((): boolean => {
  return companyStore.companiesLoading || initialLoad.value
})

const filteredCompanies = computed((): Company[] => {
  let companiesFiltered = companies.value

  if (!isEmpty(search.value)) {
    companiesFiltered = companiesFiltered.filter((company) => company.name.toLowerCase().includes(search.value.toLowerCase()))
  }

  return companiesFiltered
})

onMounted(async () => {
  await Promise.all([
    companyStore.getCompanies(),
    projectStore.getProjects(),
  ])
})


watch(companies, () => {
  initialLoad.value = false
})
</script>
