<template>
  <div class="flex flex-col overflow-y-auto p-1" :class="mobile ? '' : 'max-h-64'">
    <button
      v-for="(company, index) in filteredCompanies"
      :key="company.id"
      ref="itemRefs"
      type="button"
      class="btn btn-ghost focus-visible:outline-none justify-start hover:bg-base-200 h-auto"
      :class="[mobile ? 'py-3 text-base' : 'btn-sm py-2', { 'ring-2 ring-primary': focusedIndex === index }]"
      @click="selectCompany(company.id)"
      @mouseenter="focusedIndex = index"
      @keydown="handleItemKeydown"
    >
      <CompanyItem :company="company" size="sm" />
    </button>
    <div v-if="filteredCompanies.length === 0" class="text-sm text-center py-4 opacity-50">
      No companies found
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, nextTick, onMounted } from 'vue'
import { orderBy } from 'lodash-es'
import type { Company } from '@api'
import { useCompanyStore } from '~/store/CompanyStore'
import CompanyItem from '../form/CompanyItem.vue'

interface Props {
  mobile?: boolean
}

defineProps<Props>()

const emit = defineEmits<{
  'select': [companyId: string]
  'close': []
}>()

const companyStore = useCompanyStore()
const focusedIndex = ref(-1)
const itemRefs = ref<HTMLElement[]>([])

const companies = computed((): Company[] => {
  return orderBy(
    companyStore.companies,
    [(c) => c.startDate || ''],
    ['desc']
  )
})

const filteredCompanies = computed(() => {  
  return companies.value
})

function selectCompany(companyId: string) {
  emit('select', companyId)
}

function handleItemKeydown(event: KeyboardEvent) {
  // Stop propagation to prevent main menu from handling these events
  event.stopPropagation()
  
  switch (event.key) {
    case 'ArrowDown':
      event.preventDefault()
      if (focusedIndex.value < filteredCompanies.value.length - 1) {
        focusedIndex.value++
        scrollToFocusedItem()
      }
      break
    case 'ArrowUp':
      event.preventDefault()
      if (focusedIndex.value > 0) {
        focusedIndex.value--
        scrollToFocusedItem()
      }
      break
    case 'ArrowRight':
      event.preventDefault()
      emit('close')
      break
    case 'ArrowLeft':
      event.preventDefault()
      emit('close')
      break
    case 'Enter':
      event.preventDefault()
      if (filteredCompanies.value[focusedIndex.value]) {
        selectCompany(filteredCompanies.value[focusedIndex.value]!.id)
      }
      break
  }
}

function scrollToFocusedItem() {
  nextTick(() => {
    if (focusedIndex.value >= 0) {
      const focusedElement = itemRefs.value[focusedIndex.value]
      if (focusedElement) {
        focusedElement.scrollIntoView({ block: 'nearest', behavior: 'smooth' })
        focusedElement.focus()
      }
    }
  })
}

onMounted(() => {
  // Auto-focus first company on mount
  if (filteredCompanies.value.length > 0) {
    focusedIndex.value = 0
    scrollToFocusedItem()
  }
})
</script>
