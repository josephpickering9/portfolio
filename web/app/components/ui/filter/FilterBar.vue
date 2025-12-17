<template>
  <div class="flex flex-wrap items-center gap-2">
    <FilterChip
      v-for="(filter, index) in filters"
      :key="`${filter.type}-${index}`"
      :filter="filter"
      @remove="removeFilter(index)"
    />

    <div ref="container" class="dropdown" :class="{ 'dropdown-open': isOpen }">
      <button
        type="button"
        class="btn btn-sm gap-2"
        :class="filters.length > 0 ? 'btn-ghost' : 'btn-outline'"
        @click="toggle"
      >
        <Icon name="heroicons:funnel" size="1.2em" />
        <span>{{ filters.length > 0 ? 'Add Filter' : 'Filter' }}</span>
      </button>

      <div 
        v-if="isOpen" 
        class="dropdown-content z-[1] mt-2 w-[90vw] md:w-72 rounded-box bg-base-100 p-2 shadow-xl ring-1 ring-base-content/10 overflow-hidden" 
        :class="dropdownPosition === 'left' ? 'dropdown-left' : 'dropdown-right'" 
        @click.stop 
        @keydown="handleMainMenuKeydown"
      >
        
        <!-- Submenu View -->
        <div v-if="activeSubmenu" class="flex flex-col animate-slide-in-right h-full min-h-[300px] max-h-[60vh] md:max-h-[400px]">
          <button 
            class="btn btn-sm btn-ghost gap-2 justify-start mb-2 px-1 text-base-content/60 hover:text-base-content"
            @click="closeSubmenu"
          >
            <Icon name="heroicons:chevron-left" size="1em" />
            Back
          </button>
          
          <div class="flex-1 overflow-y-auto">
            <CompanyFilterList
              v-if="activeSubmenu === FilterType.COMPANY"
              @select="handleCompanySelect"
              @close="handleSubmenuClose"
            />

            <TagTypeFilterList
              v-else-if="activeSubmenu === FilterType.TAG_TYPE"
              @select="handleTagTypeSelect"
              @close="handleSubmenuClose"
            />

            <TagFilterList
              v-else-if="activeSubmenu === FilterType.TAG"
              @select="handleTagSelect"
              @close="handleSubmenuClose"
            />
          </div>
        </div>

        <!-- Main Menu View -->
        <div v-else class="flex flex-col gap-1">
          <div class="px-2 py-1">
            <TextInput
              ref="searchInputRef"
              v-model="searchValue"
              placeholder="Search projects..."
              size="sm"
              @keydown="handleSearchKeydown"
            />
          </div>

          <div class="divider my-0" />

          <div
            v-for="(filterType, index) in availableFilterTypes"
            :key="filterType.value"
            class="relative"
          >
            <button
              ref="filterTypeRefs"
              type="button"
              class="btn btn-sm btn-ghost focus-visible:outline-none w-full justify-start gap-3 font-normal py-3 h-auto"
              :class="{ 'ring-2 ring-primary': focusedFilterTypeIndex === index }"
              @click="openSubmenu(filterType.value)"
            >
              <Icon :name="filterType.icon" size="1.2em" class="text-base-content/70" />
              <span class="flex-1 text-left">{{ filterType.label }}</span>
              <Icon name="heroicons:chevron-right" size="1em" class="opacity-50" />
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onUnmounted, nextTick } from 'vue'
import { Icon } from '#components'
import { useCompanyStore } from '~/store/CompanyStore'
import { useTagStore } from '~/store/TagStore'
import FilterChip from './FilterChip.vue'
import TextInput from '~/components/ui/input/TextInput.vue'
import CompanyFilterList from '~/components/company/filter/CompanyFilterList.vue'
import TagTypeFilterList from '~/components/tag/filter/TagTypeFilterList.vue'
import TagFilterList from '~/components/tag/filter/TagFilterList.vue'
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
const dropdownPosition = ref<'left' | 'right'>('right')
const container = ref<HTMLElement | null>(null)

const focusedFilterTypeIndex = ref(-1) // -1 means search is focused
const filterTypeRefs = ref<HTMLElement[]>([])
const searchInputRef = ref<InstanceType<typeof TextInput> | null>(null)

const filterTypes = [
  { value: FilterType.SEARCH, label: 'Search', icon: 'heroicons:magnifying-glass' },
  { value: FilterType.COMPANY, label: 'Company', icon: 'heroicons:building-office' },
  { value: FilterType.TAG_TYPE, label: 'Tag Type', icon: 'heroicons:squares-2x2' },
  { value: FilterType.TAG, label: 'Tag', icon: 'heroicons:hashtag' },
]

const availableFilterTypes = computed(() => filterTypes.filter(ft => ft.value !== FilterType.SEARCH))

const companies = computed(() => companyStore.companies)

function calculateDropdownPosition() {
  if (container.value) {
    const rect = container.value.getBoundingClientRect()
    const dropdownWidth = 280
    const buffer = 16
    const viewportWidth = window.innerWidth
    
    const spaceOnRight = viewportWidth - rect.right
    const wouldOverflow = spaceOnRight < (dropdownWidth + buffer)
    
    dropdownPosition.value = wouldOverflow ? 'left' : 'right'
  }
}

function toggle() {
  isOpen.value = !isOpen.value
  if (!isOpen.value) {
    activeSubmenu.value = null
  } else {
    // Calculate dropdown position when opening
    calculateDropdownPosition()
    // Auto-focus search input when opening
    focusedFilterTypeIndex.value = -1
    nextTick(() => {
      searchInputRef.value?.focus()
    })
  }
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
  nextTick(() => {
    searchInputRef.value?.focus()
  })
}

function handleSubmenuClose() {
  // close() // Optional: Close entire menu on selection? Or just go back?
  // Usually for filters, you might want to select multiple or just one.
  // The existing code closed the whole menu, let's keep that behavior for 'apply' actions
  // But if it's just 'close' event from component (like a cancel), maybe just go back.
  // Checking usage: The sub-components emit 'close' usually when selection is done or cancel.
  // Let's assume selection methods handle the 'updating filters' and closing.
  // This handler might be for explicit close buttons inside components if they exist.
  // Since we have a drill-down 'Back' button, maybe we don't strictly need this unless subcomponents emit it.
  // But we bound it in template: @close="handleSubmenuClose"
  // So let's make it close everything to be safe, or just close active submenu.
  // Original behavior: close()
  close()
}

function handleSearchKeydown(event: KeyboardEvent) {
  if (event.key === 'ArrowDown' && availableFilterTypes.value.length > 0) {
    event.preventDefault()
    event.stopPropagation()
    focusedFilterTypeIndex.value = 0
    scrollToFocusedFilterType()
  }
}

function handleMainMenuKeydown(event: KeyboardEvent) {
  switch (event.key) {
    case 'ArrowDown':
      event.preventDefault()
      if (focusedFilterTypeIndex.value === -1) {
        // From search to first filter type
        focusedFilterTypeIndex.value = 0
        scrollToFocusedFilterType()
      } else if (focusedFilterTypeIndex.value < availableFilterTypes.value.length - 1) {
        focusedFilterTypeIndex.value++
        scrollToFocusedFilterType()
      }
      break
    case 'ArrowUp':
      event.preventDefault()
      if (focusedFilterTypeIndex.value === 0) {
        // From first filter type back to search
        focusedFilterTypeIndex.value = -1
        nextTick(() => {
          searchInputRef.value?.focus()
        })
      } else if (focusedFilterTypeIndex.value > 0) {
        focusedFilterTypeIndex.value--
        scrollToFocusedFilterType()
      }
      break
    case 'ArrowLeft':
    case 'ArrowRight':
    case 'Enter':
      event.preventDefault()
      if (focusedFilterTypeIndex.value >= 0 && availableFilterTypes.value[focusedFilterTypeIndex.value]) {
        openSubmenu(availableFilterTypes.value[focusedFilterTypeIndex.value]!.value)
      }
      break
    case 'Escape':
      event.preventDefault()
      close()
      break
  }
}

function scrollToFocusedFilterType() {
  nextTick(() => {
    const focusedElement = filterTypeRefs.value[focusedFilterTypeIndex.value]
    if (focusedElement) {
      focusedElement.scrollIntoView({ block: 'nearest', behavior: 'smooth' })
      focusedElement.focus()
    }
  })
}

function handleCompanySelect(companyId: string) {
  const company = companies.value.find(c => c.id === companyId)
  if (!company) return

  const newFilter: Filter = {
    type: FilterType.COMPANY,
    value: companyId,
    label: 'Company',
    displayValue: company.name,
    logo: company.logo,
  }

  emit('update:filters', [...props.filters, newFilter])
  close()
}

function handleTagTypeSelect(tagType: TagTypeValue) {
  const newFilter: Filter = {
    type: FilterType.TAG_TYPE,
    value: tagType.value,
    label: 'Tag Type',
    displayValue: tagType.label,
    icon: tagType.icon,
  }

  emit('update:filters', [...props.filters, newFilter])
  close()
}

function handleTagSelect(tagId: string) {
  const tag = tagStore.tags.find(t => t.id === tagId)
  
  const newFilter: Filter = {
    type: FilterType.TAG,
    value: tagId,
    label: 'Tag',
    displayValue: tag?.title ?? '',
    icon: tag?.icon ?? undefined,
  }

  emit('update:filters', [...props.filters, newFilter])
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

function handleClickOutside(event: MouseEvent) {
  if (container.value && !container.value.contains(event.target as Node)) {
    close()
  }
}

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
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

<style scoped>
.dropdown-left {
  right: 0;
}
.dropdown-right {
  left: 0;
}

@keyframes slideInRight {
  from { opacity: 0; transform: translateX(20px); }
  to { opacity: 1; transform: translateX(0); }
}

.animate-slide-in-right {
  animation: slideInRight 0.2s ease-out forwards;
}
</style>

