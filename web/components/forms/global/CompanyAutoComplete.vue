<template>
  <div class="flex flex-col gap-2">
    <AutoComplete v-model="value" :data="companyItems" :label="label">
      <template #item="{ item }">
        <CompanyItem :company="companies.find((company) => company.name === item.title)" />
      </template>
      <template #selectedItem>
        <CompanyItem :company="companies.find((company) => company.id === value?.value)" />
      </template>
    </AutoComplete>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import type { SearchItem } from '../../../types/SearchItem'
import { type Company } from '../../../api'
import { useCompanyStore } from '../../../store/CompanyStore'
import AutoComplete from '../AutoComplete.vue'
import CompanyItem from './CompanyItem.vue'

interface Data {
  value?: SearchItem
}

export default defineComponent({
  name: 'CompanyAutoComplete',
  components: { AutoComplete, CompanyItem },
  props: {
    label: {
      type: String,
      default: null,
    },
    modelValue: {
      type: Number,
      default: null,
    },
  },
  emits: ['update:modelValue'],
  data(): Data {
    return {
      value: undefined,
    }
  },
  computed: {
    companies(): Company[] {
      return useCompanyStore().companies
    },
    companyItems(): SearchItem[] {
      return this.companies.map((company) => {
        return {
          title: company.name,
          value: company.id,
        }
      })
    },
    selectedCompany(): Company | undefined {
      if (!this.value) return undefined

      return this.companies.find((company) => company.id === this.value?.value)
    },
  },
  async mounted() {
    if (this.companies.length === 0) await useCompanyStore().getCompanies()
  },
  watch: {
    modelValue() {
      if (this.value?.value === this.modelValue) return

      this.value = this.companyItems.find((item) => item.value === this.modelValue)
    },
    value() {
      this.$emit('update:modelValue', this.value?.value)
    },
  },
})
</script>
