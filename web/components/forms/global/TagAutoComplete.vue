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

<script lang="ts">
import { defineComponent } from 'vue'
import type { PropType } from 'vue'
import type { SearchItem } from '../../../types/SearchItem'
import { TagType, type Tag as TagModel } from '../../../api'
import { useTagStore } from '../../../store/TagStore'
import AutoComplete from '../AutoComplete.vue'
import Tag from '../../tags/Tag.vue'

interface Data {
  value: string[]
  search?: string
}

export default defineComponent({
  name: 'TagAutoComplete',
  components: { AutoComplete, Tag },
  props: {
    label: {
      type: String,
      default: null,
    },
    modelValue: {
      type: Array as PropType<string[]>,
      default: () => [],
    },
    modelSearch: {
      type: String,
      default: '',
    },
    placeholder: {
      type: String,
      default: null,
    },
    showEmptyMessage: {
      type: Boolean,
      default: true,
    },
    openOnFocus: {
      type: Boolean,
      default: true,
    },
  },
  emits: ['update:modelValue', 'update:modelSearch', 'focus', 'blur'],
  data(): Data {
    return {
      value: [],
      search: undefined,
    }
  },
  computed: {
    tags(): TagModel[] {
      return useTagStore().tags
    },
    tagItems(): SearchItem[] {
      return this.tags
        .filter((tag) => !this.convertedTags.some((c) => c.title === tag.title))
        .map((tag) => {
          return {
            title: tag.title,
            value: tag.id,
          }
        })
    },
    convertedTags(): TagModel[] {
      return this.value.map((title) => {
        const tag = this.tags.find((t) => t.title === title)
        if (tag) return tag

        return {
          id: 0,
          title,
          type: TagType.DEFAULT,
          projects: [],
        }
      })
    },
  },
  async mounted() {
    if (this.tags.length === 0) await useTagStore().getTags()
  },
  methods: {
    selectTag(tag: SearchItem) {
      this.value.push(tag.title)
      this.$emit('update:modelValue', this.value)
    },
    removeTag(tag: TagModel) {
      this.value = this.value.filter((t) => t !== tag.title)
      this.$emit('update:modelValue', this.value)
    },
    defaultTag(title: string): TagModel {
      return {
        id: 0,
        title,
        type: TagType.DEFAULT,
        projects: [],
      }
    },
  },
  watch: {
    modelValue() {
      this.value = this.modelValue
    },
    value() {
      this.$emit('update:modelValue', this.value)
    },
    modelSearch() {
      this.search = this.modelSearch
    },
    search() {
      this.$emit('update:modelSearch', this.search)
    },
  },
})
</script>
