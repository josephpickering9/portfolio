<template>
  <div ref="container" class="dropdown flex-shrink-0" :class="{ 'dropdown-open': isOpen }">
    <button
      type="button"
      class="btn btn-sm gap-2"
      :class="filtersCount > 0 ? 'btn-ghost' : 'btn-outline'"
      @click="$emit('toggle')"
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
          @click="$emit('submenu:close')"
        >
          <Icon name="heroicons:chevron-left" size="1em" />
          Back
        </button>

        <div class="flex-1 overflow-y-auto">
          <CompanyFilterList
            v-if="activeSubmenu === FilterType.COMPANY"
            @select="$emit('company:select', $event)"
            @close="$emit('close')"
          />
          <TagTypeFilterList
            v-else-if="activeSubmenu === FilterType.TAG_TYPE"
            @select="$emit('tagType:select', $event)"
            @close="$emit('close')"
          />
          <TagFilterList
            v-else-if="activeSubmenu === FilterType.TAG"
            @select="$emit('tag:select', $event)"
            @close="$emit('close')"
          />
        </div>
      </div>

      <div v-else class="flex flex-col gap-1">
        <div class="px-2 py-1">
          <TextInput
            ref="searchInputRef"
            :model-value="searchValue"
            placeholder="Search projects..."
            size="sm"
            @update:model-value="$emit('update:searchValue', $event)"
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
            @click="$emit('submenu:open', filterType.value)"
          >
            <Icon :name="filterType.icon" size="1.2em" class="text-base-content/70" />
            <span class="flex-1 text-left">{{ filterType.label }}</span>
            <Icon name="heroicons:chevron-right" size="1em" class="opacity-50" />
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, nextTick, onMounted, onUnmounted, watch } from 'vue'
import { FilterType } from '~/types/FilterType'
import type { TagTypeValue } from '~/types/TagTypeValue'
import TextInput from '~/components/ui/input/TextInput.vue'
import CompanyFilterList from '~/components/company/filter/CompanyFilterList.vue'
import TagTypeFilterList from '~/components/tag/filter/TagTypeFilterList.vue'
import TagFilterList from '~/components/tag/filter/TagFilterList.vue'

interface FilterTypeItem {
  value: string
  label: string
  icon: string
}

interface Props {
  isOpen: boolean
  activeSubmenu: string | null
  searchValue: string
  availableFilterTypes: FilterTypeItem[]
  filtersCount: number
}

const props = defineProps<Props>()

const emit = defineEmits<{
  'toggle': []
  'close': []
  'update:searchValue': [value: string]
  'submenu:open': [filterType: string]
  'submenu:close': []
  'company:select': [companyId: string]
  'tagType:select': [tagType: TagTypeValue]
  'tag:select': [tagId: string]
}>()

const container = ref<HTMLElement | null>(null)
const searchInputRef = ref<InstanceType<typeof TextInput> | null>(null)
const filterTypeRefs = ref<HTMLElement[]>([])
const focusedFilterTypeIndex = ref(-1)
const dropdownPosition = ref<'left' | 'right'>('right')

function calculateDropdownPosition() {
  if (container.value) {
    const rect = container.value.getBoundingClientRect()
    const dropdownWidth = 280
    const buffer = 16
    const spaceOnRight = window.innerWidth - rect.right
    dropdownPosition.value = spaceOnRight < (dropdownWidth + buffer) ? 'left' : 'right'
  }
}

function focusSearch() {
  nextTick(() => searchInputRef.value?.focus())
}

function handleSearchKeydown(event: KeyboardEvent) {
  if (event.key === 'ArrowDown' && props.availableFilterTypes.length > 0) {
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
      } else if (focusedFilterTypeIndex.value < props.availableFilterTypes.length - 1) {
        focusedFilterTypeIndex.value++
      }
      scrollToFocusedFilterType()
      break
    case 'ArrowUp':
      event.preventDefault()
      if (focusedFilterTypeIndex.value === 0) {
        focusedFilterTypeIndex.value = -1
        nextTick(() => searchInputRef.value?.focus())
      } else if (focusedFilterTypeIndex.value > 0) {
        focusedFilterTypeIndex.value--
        scrollToFocusedFilterType()
      }
      break
    case 'ArrowLeft':
    case 'ArrowRight':
    case 'Enter':
      event.preventDefault()
      if (focusedFilterTypeIndex.value >= 0 && props.availableFilterTypes[focusedFilterTypeIndex.value]) {
        emit('submenu:open', props.availableFilterTypes[focusedFilterTypeIndex.value]!.value)
      }
      break
    case 'Escape':
      event.preventDefault()
      emit('close')
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

function handleClickOutside(event: MouseEvent) {
  if (container.value && !container.value.contains(event.target as Node)) {
    emit('close')
  }
}

watch(() => props.isOpen, (open) => {
  if (open) {
    calculateDropdownPosition()
    focusedFilterTypeIndex.value = -1
    focusSearch()
  }
})

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})

defineExpose({ focusSearch })
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
