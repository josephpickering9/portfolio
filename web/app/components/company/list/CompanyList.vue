<template>
  <div class="companies prose w-full max-w-5xl space-y-8">
    <div class="flex flex-col items-center justify-between gap-4 md:flex-row">
      <h1 class="m-0">Companies</h1>
      <div class="flex items-center gap-4">
        <TextInput v-model="search" class="w-full md:max-w-48" size="sm" placeholder="Search" :disabled="loading" />
        <ClientOnly>
          <FormButton
            v-if="isAuthenticated"
            label="Add Company"
            type="primary"
            size="sm"
            href="/companies/new"
            :disabled="loading"
          />
        </ClientOnly>
      </div>
    </div>
    <div v-if="loading" class="flex flex-col items-center space-y-4">
      <Skeleton />
      <Skeleton />
      <Skeleton />
    </div>
    <div v-else-if="Object.keys(filteredCompanies).length === 0" class="flex flex-col justify-start space-y-4">
      <h2>No companies found</h2>
      <div class="text-lg">Try searching for something else</div>
    </div>
    <div v-else class="flex flex-col items-center justify-start space-y-4">
      <CompanyListItem v-for="(company, index) in filteredCompanies" :key="index" :company="company" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { isEmpty } from 'lodash-es'
import { useRoute, useRouter } from 'vue-router'
import { useCompanyStore } from '~/store/CompanyStore'
import type { Company } from '@api/models/Company'
import useAuth from '~/composables/useAuth'
import Skeleton from '~/components/feedback/loading/Skeleton.vue'
import TextInput from '~/components/ui/input/TextInput.vue'
import FormButton from '~/components/ui/form/FormButton.vue'
import CompanyListItem from './CompanyListItem.vue'

const route = useRoute()
const router = useRouter()
const { isAuthenticated } = useAuth()
const companyStore = useCompanyStore()

const initialLoad = ref(true)
const search = ref('')

const companies = computed((): Company[] => {
  return companyStore.companies
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
  await companyStore.getCompanies()
})

// Watch methods
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
