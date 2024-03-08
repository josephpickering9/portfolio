<template>
  <AutoComplete
    v-model="value"
    :data="companyItems"
    :label="label"
    :placeholder="placeholder"
    :size="size"
    :disabled="disabled"
  >
    <template #item="{ item }">
      <CompanyItem :company="companies.find((company) => company.name === item.title)" />
    </template>
    <template #selectedItem>
      <CompanyItem :company="companies.find((company) => company.id === value?.value)" :size="selectedSize" />
    </template>
  </AutoComplete>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import type { SearchItem } from '../../../types/SearchItem'
import { type Company } from '../../../api'
import { useCompanyStore } from '../../../store/CompanyStore'
import AutoComplete from '../elements/AutoComplete.vue'
import CompanyItem from './CompanyItem.vue'

interface Data {
  value?: SearchItem
}

export default defineComponent({
  name: 'CompanyAutoComplete',
  components: { AutoComplete, CompanyItem },
  props: {
    modelValue: {
      type: Number as () => number | null | undefined,
      default: null,
    },
    label: {
      type: String,
      default: null,
    },
    disabled: {
      type: Boolean,
      default: false,
    },
    placeholder: {
      type: String,
      default: undefined,
    },
    size: {
      type: String,
      default: 'md',
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
    selectedSize(): string {
      switch (this.size) {
        case 'xs':
          return 'xs'
        case 'sm':
          return 'xs'
        case 'md':
          return 'sm'
        case 'lg':
          return 'md'
        default:
          return 'sm'
      }
    },
  },
  async mounted() {
    if (this.companies.length === 0) await useCompanyStore().getCompanies()

    if (this.modelValue) this.setValue()
  },
  methods: {
    setValue() {
      this.value = this.companyItems.find((item) => item.value === this.modelValue)
    },
  },
  watch: {
    modelValue() {
      if (this.value?.value === this.modelValue) return

      this.setValue()
    },
    value() {
      this.$emit('update:modelValue', this.value?.value)
    },
  },
})
</script>
