<template>
  <div class="flex flex-col gap-2">
    <AutoComplete
      v-model:model-search="search"
      :data="tagItems"
      :label="label"
      :show-selected="false"
      :open-on-focus="openOnFocus"
      :placeholder="placeholder"
      @select="selectTag"
      @blur="$emit('blur')"
      @focus="$emit('focus')"
    >
      <template #item="{ item }">
        <Tag :tag="tags.find((tag) => tag.title === item.title) ?? defaultTag(item.title)" />
      </template>
    </AutoComplete>
    <div class="flex min-h-8 items-center px-4">
      <div v-if="convertedTags && convertedTags.length" class="flex flex-wrap items-center gap-2">
        <Tag v-for="(tag, index) in convertedTags" :key="index" :tag="tag" clearable @remove="removeTag" />
      </div>
      <div v-else-if="showEmptyMessage" class="flex items-center text-sm italic text-gray-500">No tags added</div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import type { SearchItem } from '@types/SearchItem'
import { TagType, type Tag as TagModel } from '@api'
import { useTagStore } from '~/store/TagStore'
import AutoComplete from '~/components/ui/input/AutoComplete.vue'
import Tag from '~/components/tag/Tag.vue'

interface Props {
  label?: string | undefined
  modelValue?: string[]
  modelSearch?: string
  placeholder?: string | undefined
  showEmptyMessage?: boolean
  openOnFocus?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  label: undefined,
  modelValue: () => [],
  modelSearch: '',
  placeholder: undefined,
  showEmptyMessage: true,
  openOnFocus: true,
})

// Emits
const emit = defineEmits<{
  'update:modelValue': [value: string[]]
  'update:modelSearch': [value: string | undefined]
  'focus': []
  'blur': []
}>()

const tagStore = useTagStore()

const value = ref<string[]>([])
const search = ref<string | undefined>(undefined)

const tags = computed((): TagModel[] => {
  return tagStore.tags
})

const tagItems = computed((): SearchItem[] => {
  return tags.value
    .filter((tag) => !convertedTags.value.some((c) => c.title === tag.title))
    .map((tag) => ({
      title: tag.title,
      value: tag.id,
    }))
})

const convertedTags = computed((): TagModel[] => {
  return value.value.map((title) => {
    const tag = tags.value.find((t) => t.title === title)
    if (tag) return tag

    return {
      id: 0,
      title,
      slug: title.toLowerCase().replace(/\s+/g, '-'),
      type: TagType.DEFAULT,
      icon: undefined,
      customColour: undefined,
    }
  })
})

function selectTag(tag: SearchItem | undefined) {
  if (!tag) return
  value.value.push(tag.title)
  emit('update:modelValue', value.value)
}

function removeTag(tag: TagModel) {
  value.value = value.value.filter((t) => t !== tag.title)
  emit('update:modelValue', value.value)
}

function defaultTag(title: string): TagModel {
  return {
    id: 0,
    title,
    slug: title.toLowerCase().replace(/\s+/g, '-'),
    type: TagType.DEFAULT,
    icon: undefined,
    customColour: undefined,
  }
}

onMounted(async () => {
  if (tags.value.length === 0) await tagStore.getTags()
})

// Watch methods
watch(() => props.modelValue, (newValue) => {
  value.value = newValue
})

watch(value, (newValue) => {
  emit('update:modelValue', newValue)
})

watch(() => props.modelSearch, (newValue) => {
  search.value = newValue
})

watch(search, (newValue) => {
  emit('update:modelSearch', newValue)
})
</script>

