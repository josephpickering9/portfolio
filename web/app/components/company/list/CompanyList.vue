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
      <div class="group relative">
        <div class="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
          <Icon name="heroicons:magnifying-glass" class="h-5 w-5 text-base-content/40 group-hover:text-primary" />
        </div>
        <TextInput
          v-model="search"
          class="w-full md:max-w-48"
          size="md"
          placeholder="Search companies..."
          :disabled="loading"
        />
      </div>

      <ClientOnly>
        <FormButton
          v-if="isAuthenticated"
          label="Add Company"
          icon="heroicons:plus"
          type="primary"
          size="md"
          href="/companies/new"
          :disabled="loading"
          class="w-full shadow-lg shadow-primary/20 sm:w-auto"
        />
      </ClientOnly>
    </template>

    <CompanyTimeline :companies="filteredCompanies" :projects="projects" />
  </ListLayout>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { isEmpty } from 'lodash-es'
import { useRoute, useRouter } from 'vue-router'
import { useCompanyStore } from '~/store/CompanyStore'
import { useProjectStore } from '~/store/ProjectStore'
import type { Company } from '@api/models/Company'
import useAuth from '~/composables/useAuth'
import ListLayout from '~/components/ui/layout/ListLayout.vue'
import TextInput from '~/components/ui/input/TextInput.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import CompanyTimeline from '~/components/company/timeline/CompanyTimeline.vue'

const route = useRoute()
const router = useRouter()
const { isAuthenticated } = useAuth()
const companyStore = useCompanyStore()
const projectStore = useProjectStore()

const initialLoad = ref(true)
const search = ref('')

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

function setValues() {
  search.value = route.query.search?.toString() || search.value
}

function updateQueryParams() {
  router.push({
    path: route.path,
    query: {
      search: !isEmpty(search.value) ? search.value : undefined,
    },
  })
}

onMounted(async () => {
  setValues()
  await Promise.all([
    companyStore.getCompanies(),
    projectStore.getProjects(),
  ])
})


watch(companies, () => {
  initialLoad.value = false
})

watch(() => route.query, () => {
  setValues()
})

watch(search, () => {
  updateQueryParams()
})
</script>
