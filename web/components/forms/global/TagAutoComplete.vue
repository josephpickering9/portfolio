<template>
  <div class="flex flex-col gap-2">
    <AutoComplete :data="tagItems" :label="label" placeholder="Search" @select="selectTag">
      <template #item="{ item }">
        <Tag :tag="tags.find((tag) => tag.title === item.title) ?? defaultTag(item.title)" />
      </template>
    </AutoComplete>
    <div class="flex min-h-8 items-center px-4">
      <div v-if="convertedTags && convertedTags.length" class="flex items-center gap-2">
        <Tag v-for="(tag, index) in convertedTags" :key="index" :tag="tag" clearable @remove="removeTag" />
      </div>
      <div v-else class="flex items-center text-sm italic text-gray-500">No tags added</div>
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
  },
  emits: ['update:modelValue'],
  data(): Data {
    return {
      value: [],
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
    selectTag(tag: string) {
      this.value.push(tag)
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
  },
})
</script>
