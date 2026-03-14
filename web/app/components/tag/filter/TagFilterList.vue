<template>
  <div class="flex flex-col gap-1 overflow-y-auto p-1" :class="mobile ? '' : 'max-h-64'">
    <div class="px-2 pt-1 pb-2">
      <TextInput
        ref="searchInputRef"
        v-model="searchQuery"
        placeholder="Search technologies..."
        :size="mobile ? 'md' : 'sm'"
        @keydown="handleSearchKeydown"
      />
    </div>

    <div class="divider my-0" />

    <button
      v-for="(tag, index) in filteredTags"
      :key="tag.id"
      ref="itemRefs"
      type="button"
      class="btn btn-ghost focus-visible:outline-none justify-between hover:bg-base-200 h-auto"
      :class="[mobile ? 'py-3 text-base' : 'btn-sm py-2', { 'ring-2 ring-primary': focusedIndex === index }]"
      @click="selectTag(tag.id)"
      @mouseenter="focusedIndex = index"
      @keydown="handleItemKeydown"
    >
      <Tag :tag="tag" size="sm" />
      <span v-if="tagCounts[tag.title]" class="badge badge-sm badge-ghost ml-auto">
        {{ tagCounts[tag.title] }}
      </span>
    </button>
    <div v-if="filteredTags.length === 0" class="text-sm text-center py-4 opacity-50">
      No technologies found
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, nextTick } from 'vue'
import type { Tag as TagType } from '@api'
import { useTagStore } from '~/store/TagStore'
import { useProjectStore } from '~/store/ProjectStore'
import { countBy, flatMap, isEmpty, orderBy } from 'lodash-es'
import Tag from '../Tag.vue'
import TextInput from '~/components/ui/input/TextInput.vue'

interface Props {
  mobile?: boolean
}

defineProps<Props>()

const emit = defineEmits<{
  'select': [tagTitle: string]
  'close': []
}>()

const tagStore = useTagStore()
const projectStore = useProjectStore()
const searchQuery = ref('')
const focusedIndex = ref(-1) // -1 means search input is focused
const searchInputRef = ref<InstanceType<typeof TextInput> | null>(null)
const itemRefs = ref<HTMLElement[]>([])

// Calculate tag counts from projects
const tagCounts = computed((): Record<string, number> => {
  return countBy(flatMap(projectStore.projects, 'tags'), 'title')
})

const tags = computed((): TagType[] => {
  // Sort by count (descending), then by title (ascending)
  return orderBy(
    tagStore.tags,
    [(tag) => tagCounts.value[tag.title] || 0, (tag) => tag.title],
    ['desc', 'asc']
  )
})

const filteredTags = computed(() => {
  if (isEmpty(searchQuery.value)) return tags.value
  
  const query = searchQuery.value.toLowerCase()
  return tags.value.filter(tag => 
    tag.title.toLowerCase().includes(query)
  )
})

function selectTag(tagId: string) {
  emit('select', tagId)
}

function handleSearchKeydown(event: KeyboardEvent) {
  // Stop propagation to prevent main menu from handling these events
  event.stopPropagation()
  
  if (event.key === 'ArrowDown' && filteredTags.value.length > 0) {
    event.preventDefault()
    focusedIndex.value = 0
    scrollToFocusedItem()
  } else if (event.key === 'ArrowLeft' || event.key === 'ArrowRight') {
    event.preventDefault()
    emit('close')
  }
}

function handleItemKeydown(event: KeyboardEvent) {
  // Stop propagation to prevent main menu from handling these events
  event.stopPropagation()
  
  switch (event.key) {
    case 'ArrowDown':
      event.preventDefault()
      if (focusedIndex.value < filteredTags.value.length - 1) {
        focusedIndex.value++
        scrollToFocusedItem()
      }
      break
    case 'ArrowUp':
      event.preventDefault()
      if (focusedIndex.value > 0) {
        focusedIndex.value--
        scrollToFocusedItem()
      } else {
        // Return focus to search input
        focusedIndex.value = -1
        nextTick(() => {
          searchInputRef.value?.focus()
        })
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
      if (filteredTags.value[focusedIndex.value]) {
        selectTag(filteredTags.value[focusedIndex.value]!.id)
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
  if (tagStore.tags.length === 0) tagStore.getTags()
  if (projectStore.projects.length === 0) projectStore.getProjects()
  
  nextTick(() => {
    searchInputRef.value?.focus()
  })
})
</script>

