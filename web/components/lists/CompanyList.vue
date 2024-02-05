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

<script lang="ts">
import { defineComponent } from 'vue'
import { isEmpty } from 'lodash-es'
import Skeleton from '../loading/Skeleton.vue'
import { useCompanyStore } from '../../store/CompanyStore'
import type { Company } from '../../api/models/Company'
import TextInput from '../forms/TextInput.vue'
import FormButton from '../forms/FormButton.vue'
import useAuth from '../../composables/useAuth'
import CompanyListItem from './CompanyListItem.vue'

interface Data {
  initialLoad: boolean
  search: string
}

export default defineComponent({
  name: 'CompanyList',
  components: {
    Skeleton,
    CompanyListItem,
    TextInput,
    FormButton,
  },
  setup() {
    const { isAuthenticated } = useAuth()

    return {
      isAuthenticated,
    }
  },
  data(): Data {
    return {
      initialLoad: true,
      search: '',
    }
  },
  computed: {
    companies(): Company[] {
      return useCompanyStore().companies
    },
    loading(): boolean {
      return useCompanyStore().companiesLoading || this.initialLoad
    },
    filteredCompanies(): Company[] {
      let companies = this.companies

      if (!isEmpty(this.search)) {
        companies = companies.filter((company) => company.name.toLowerCase().includes(this.search.toLowerCase()))
      }

      return companies
    },
  },
  async mounted() {
    await useCompanyStore().getCompanies()
  },
  watch: {
    companies() {
      this.initialLoad = false
    },
  },
})
</script>
