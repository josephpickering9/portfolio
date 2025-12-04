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

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import type { SearchItem } from '~/types/SearchItem'
import type { Company } from '~/api'
import { useCompanyStore } from '~/app/store/CompanyStore'
import AutoComplete from '~/app/components/forms/elements/AutoComplete.vue'
import CompanyItem from './CompanyItem.vue'

interface Props {
  modelValue?: number | null | undefined
  label?: string | null
  disabled?: boolean
  placeholder?: string | undefined
  size?: string
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: undefined,
  label: undefined,
  disabled: false,
  placeholder: undefined,
  size: 'md',
})

// Emits
const emit = defineEmits<{
  'update:modelValue': [value: number | undefined]
}>()

const companyStore = useCompanyStore()

const value = ref<SearchItem | undefined>(undefined)

const companies = computed((): Company[] => {
  return companyStore.companies
})

const companyItems = computed((): SearchItem[] => {
  return companies.value.map((company) => ({
    title: company.name,
    value: company.id,
  }))
})

const selectedCompany = computed((): Company | undefined => {
  if (!value.value) return undefined

  return companies.value.find((company) => company.id === value.value?.value)
})

const selectedSize = computed((): string => {
  switch (props.size) {
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
})

function setValue() {
  value.value = companyItems.value.find((item) => item.value === props.modelValue)
}

onMounted(async () => {
  if (companies.value.length === 0) await companyStore.getCompanies()

  if (props.modelValue) setValue()
})

// Watch methods
watch(() => props.modelValue, () => {
  if (value.value?.value === props.modelValue) return

  setValue()
})

watch(value, () => {
  emit('update:modelValue', value.value?.value as number | undefined)
})
</script>
