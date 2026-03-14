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

    <div v-if="!isMobile" ref="container" class="dropdown flex-shrink-0" :class="{ 'dropdown-open': isOpen }">
      <button
        type="button"
        class="btn btn-sm gap-2"
        :class="filters.length > 0 ? 'btn-ghost' : 'btn-outline'"
        @click="toggle"
      >
        <Icon name="heroicons:funnel" size="1.2em" />
        <span>Filter</span>
      </button>

      <div
        v-if="isOpen"
        class="dropdown-content z-[1] mt-2 w-72 rounded-box bg-base-100 p-2 shadow-xl ring-1 ring-base-content/10 overflow-hidden"
        :class="dropdownPosition === 'left' ? 'dropdown-left' : 'dropdown-right'"
        @click.stop
        @keydown="handleMainMenuKeydown"
      >
        <div v-if="activeSubmenu" class="flex flex-col animate-slide-in-right h-full min-h-[300px] max-h-[400px]">
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

    <Teleport to="body">
      <Transition name="sheet-backdrop">
        <div
          v-if="isMobile && isOpen"
          class="fixed inset-0 z-50"
        >
          <Transition name="sheet-panel">
            <div
              v-if="isOpen"
              class="absolute inset-0 bg-base-100 flex flex-col"
            >
              <div
                class="flex items-center justify-between px-4 flex-shrink-0"
                style="padding-top: max(env(safe-area-inset-top, 0px), 16px); padding-bottom: 12px;"
              >
                <span class="font-semibold text-base">
                  {{ activeSubmenu ? submenuTitle : 'Filter' }}
                </span>
                <button
                  type="button"
                  class="btn btn-sm btn-ghost btn-circle"
                  @click="close"
                >
                  <Icon name="heroicons:x-mark" size="1.2em" />
                </button>
              </div>

              <div class="divider my-0 flex-shrink-0" />

              <div v-if="activeSubmenu" class="flex flex-col flex-1 overflow-hidden animate-slide-in-right" style="padding-bottom: env(safe-area-inset-bottom, 0px);">
                <div class="px-3 pt-2 flex-shrink-0">
                  <button
                    class="btn btn-sm btn-ghost gap-2 justify-start text-base-content/60 hover:text-base-content w-full justify-start"
                    @click="closeSubmenu"
                  >
                    <Icon name="heroicons:chevron-left" size="1em" />
                    Back
                  </button>
                </div>

                <div class="flex-1 overflow-y-auto px-2 pb-2">
                  <CompanyFilterList
                    v-if="activeSubmenu === FilterType.COMPANY"
                    mobile
                    @select="handleCompanySelect"
                    @close="handleSubmenuClose"
                  />
                  <TagTypeFilterList
                    v-else-if="activeSubmenu === FilterType.TAG_TYPE"
                    mobile
                    @select="handleTagTypeSelect"
                    @close="handleSubmenuClose"
                  />
                  <TagFilterList
                    v-else-if="activeSubmenu === FilterType.TAG"
                    mobile
                    @select="handleTagSelect"
                    @close="handleSubmenuClose"
                  />
                </div>
              </div>

              <div v-else class="flex flex-col flex-1 overflow-hidden" style="padding-bottom: env(safe-area-inset-bottom, 0px);">
                <div class="px-4 pb-3 pt-1 flex-shrink-0">
                  <TextInput
                    ref="mobileSearchInputRef"
                    v-model="searchValue"
                    placeholder="Search projects..."
                    size="md"
                  />
                </div>

                <div class="flex-1 overflow-y-auto px-2 pb-2">
                  <button
                    v-for="filterType in availableFilterTypes"
                    :key="filterType.value"
                    type="button"
                    class="btn btn-ghost w-full justify-start gap-4 font-normal h-auto py-4 text-base"
                    @click="openSubmenu(filterType.value)"
                  >
                    <Icon :name="filterType.icon" size="1.4em" class="text-base-content/70" />
                    <span class="flex-1 text-left">{{ filterType.label }}</span>
                    <Icon name="heroicons:chevron-right" size="1.1em" class="opacity-40" />
                  </button>
                </div>
              </div>
            </div>
          </Transition>
        </div>
      </Transition>
    </Teleport>
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
const isMobile = ref(false)

const focusedFilterTypeIndex = ref(-1)
const filterTypeRefs = ref<HTMLElement[]>([])
const searchInputRef = ref<InstanceType<typeof TextInput> | null>(null)
const mobileSearchInputRef = ref<InstanceType<typeof TextInput> | null>(null)

const filterTypes = [
  { value: FilterType.SEARCH, label: 'Search', icon: 'heroicons:magnifying-glass' },
  { value: FilterType.COMPANY, label: 'Company', icon: 'heroicons:building-office' },
  { value: FilterType.TAG_TYPE, label: 'Technology Type', icon: 'heroicons:squares-2x2' },
  { value: FilterType.TAG, label: 'Technology', icon: 'heroicons:code-bracket' },
]

const availableFilterTypes = computed(() => filterTypes.filter(ft => ft.value !== FilterType.SEARCH))

const submenuTitle = computed(() => {
  return filterTypes.find(ft => ft.value === activeSubmenu.value)?.label ?? ''
})

const companies = computed(() => companyStore.companies)

function checkMobile() {
  isMobile.value = window.innerWidth < 768
}

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
    if (!isMobile.value) {
      calculateDropdownPosition()
      focusedFilterTypeIndex.value = -1
      nextTick(() => {
        searchInputRef.value?.focus()
      })
    } else {
      nextTick(() => {
        mobileSearchInputRef.value?.focus()
      })
    }
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
    if (isMobile.value) {
      mobileSearchInputRef.value?.focus()
    } else {
      searchInputRef.value?.focus()
    }
  })
}

function handleSubmenuClose() {
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
  if (!isMobile.value && container.value && !container.value.contains(event.target as Node)) {
    close()
  }
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
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  window.removeEventListener('resize', checkMobile)
  document.removeEventListener('click', handleClickOutside)
  document.body.style.overflow = ''
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

.sheet-backdrop-enter-active,
.sheet-backdrop-leave-active {
  transition: opacity 0.2s ease;
}
.sheet-backdrop-enter-from,
.sheet-backdrop-leave-to {
  opacity: 0;
}

.sheet-panel-enter-active,
.sheet-panel-leave-active {
  transition: transform 0.28s cubic-bezier(0.32, 0.72, 0, 1);
}
.sheet-panel-enter-from,
.sheet-panel-leave-to {
  transform: translateX(100%);
}
</style>
