<template>
  <div class="flex items-center gap-2 flex-1">
    <div class="flex flex-wrap items-center gap-2 flex-1">
      <FilterChip
        v-for="(filter, index) in filters"
        :key="`${filter.type}-${index}`"
        :filter="filter"
        @remove="removeFilter(index)"
      />
    </div>

    <FilterBarDesktopDropdown
      v-if="!isMobile"
      :is-open="isOpen"
      :active-submenu="activeSubmenu"
      :search-value="searchValue"
      :available-filter-types="availableFilterTypes"
      :filters-count="filters.length"
      @toggle="toggle"
      @close="close"
      @update:search-value="searchValue = $event"
      @submenu:open="openSubmenu"
      @submenu:close="closeSubmenu"
      @company:select="handleCompanySelect"
      @tag-type:select="handleTagTypeSelect"
      @tag:select="handleTagSelect"
    />

    <button
      v-else
      type="button"
      class="btn btn-sm gap-2 flex-shrink-0"
      :class="filters.length > 0 ? 'btn-ghost' : 'btn-outline'"
      @click="toggle"
    >
      <Icon name="heroicons:funnel" size="1.2em" />
      <span>Filter</span>
    </button>

    <FilterBarMobileSheet
      :is-open="isMobile && isOpen"
      :active-submenu="activeSubmenu"
      :search-value="searchValue"
      :available-filter-types="availableFilterTypes"
      :submenu-title="submenuTitle"
      :filters-count="filters.length"
      @close="close"
      @update:search-value="searchValue = $event"
      @submenu:open="openSubmenu"
      @submenu:close="closeSubmenu"
      @company:select="handleCompanySelect"
      @tag-type:select="handleTagTypeSelect"
      @tag:select="handleTagSelect"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted } from 'vue'
import { useCompanyStore } from '~/store/CompanyStore'
import { useTagStore } from '~/store/TagStore'
import FilterChip from './FilterChip.vue'
import FilterBarDesktopDropdown from './FilterBarDesktopDropdown.vue'
import FilterBarMobileSheet from './FilterBarMobileSheet.vue'
import type { Filter } from '~/types/Filter'
import { FilterType } from '~/types/FilterType'
import type { TagTypeValue } from '~/types/TagTypeValue'

interface Props {
  filters: Filter[]
}

const props = defineProps<Props>()

const emit = defineEmits<{
  'update:filters': [filters: Filter[]]
}>()

const companyStore = useCompanyStore()
const tagStore = useTagStore()

const isOpen = ref(false)
const searchValue = ref<string>('')
const activeSubmenu = ref<string | null>(null)
const isMobile = ref(false)

const filterTypes = [
  { value: FilterType.SEARCH, label: 'Search', icon: 'heroicons:magnifying-glass' },
  { value: FilterType.COMPANY, label: 'Company', icon: 'heroicons:building-office' },
  { value: FilterType.TAG_TYPE, label: 'Technology Type', icon: 'heroicons:squares-2x2' },
  { value: FilterType.TAG, label: 'Technology', icon: 'heroicons:code-bracket' },
]

const availableFilterTypes = computed(() => filterTypes.filter(ft => ft.value !== FilterType.SEARCH))

const submenuTitle = computed(() => filterTypes.find(ft => ft.value === activeSubmenu.value)?.label ?? '')

const companies = computed(() => companyStore.companies)

function checkMobile() {
  isMobile.value = window.innerWidth < 768
}

function toggle() {
  isOpen.value = !isOpen.value
  if (!isOpen.value) activeSubmenu.value = null
}

function close() {
  isOpen.value = false
  activeSubmenu.value = null
}

function openSubmenu(filterTypeValue: string) {
  activeSubmenu.value = filterTypeValue
}

function closeSubmenu() {
  activeSubmenu.value = null
}

function handleCompanySelect(companyId: string) {
  const company = companies.value.find(c => c.id === companyId)
  if (!company) return

  emit('update:filters', [...props.filters, {
    type: FilterType.COMPANY,
    value: companyId,
    label: 'Company',
    displayValue: company.name,
    logo: company.logo,
  }])
  close()
}

function handleTagTypeSelect(tagType: TagTypeValue) {
  emit('update:filters', [...props.filters, {
    type: FilterType.TAG_TYPE,
    value: tagType.value,
    label: 'Tag Type',
    displayValue: tagType.label,
    icon: tagType.icon,
  }])
  close()
}

function handleTagSelect(tagId: string) {
  const tag = tagStore.tags.find(t => t.id === tagId)

  emit('update:filters', [...props.filters, {
    type: FilterType.TAG,
    value: tagId,
    label: 'Tag',
    displayValue: tag?.title ?? '',
    icon: tag?.icon ?? undefined,
  }])
  close()
}

function removeFilter(index: number) {
  const updatedFilters = [...props.filters]
  const removedFilter = updatedFilters.splice(index, 1)[0]

  if (removedFilter?.type === FilterType.SEARCH) searchValue.value = ''

  emit('update:filters', updatedFilters)
}

function applySearchFilter() {
  if (!searchValue.value.trim()) {
    const searchFilterIndex = props.filters.findIndex(f => f.type === FilterType.SEARCH)
    if (searchFilterIndex !== -1) removeFilter(searchFilterIndex)
    return
  }

  const updatedFilters = [...props.filters]
  const searchFilterIndex = updatedFilters.findIndex(f => f.type === FilterType.SEARCH)

  const searchFilter: Filter = {
    type: FilterType.SEARCH,
    value: searchValue.value.trim(),
    label: 'Search',
  }

  if (searchFilterIndex !== -1) updatedFilters[searchFilterIndex] = searchFilter
  else updatedFilters.push(searchFilter)

  emit('update:filters', updatedFilters)
}

watch([isOpen, isMobile], ([open, mobile]) => {
  document.body.style.overflow = (open && mobile) ? 'hidden' : ''
})

watch(() => props.filters, (newFilters) => {
  const searchFilter = newFilters.find(f => f.type === FilterType.SEARCH)
  if (searchFilter && searchFilter.value !== searchValue.value) {
    searchValue.value = searchFilter.value
  } else if (!searchFilter && searchValue.value) {
    searchValue.value = ''
  }
}, { deep: true, immediate: true })

watch(() => searchValue.value, () => {
  applySearchFilter()
})

onMounted(() => {
  checkMobile()
  window.addEventListener('resize', checkMobile)
})

onUnmounted(() => {
  window.removeEventListener('resize', checkMobile)
  document.body.style.overflow = ''
})
</script>
