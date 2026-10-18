<template>
  <div class="relative">
    <div :class="[fieldClass, 'flex flex-wrap items-center gap-1.5 py-2 focus-within:border-accent']" @click="input?.focus()">
      <span v-for="tag in selected" :key="tag.title" class="flex items-center gap-1.5 rounded-full bg-surface2 py-1.25 pr-1.5 pl-2.5 text-[13px]">
        <Icon :name="getTagIcon(tag)" />
        {{ tag.title }}
        <button
          type="button"
          class="flex size-5 items-center justify-center rounded-full text-muted hover:text-text"
          :aria-label="`Remove ${tag.title}`"
          @click.stop="remove(tag.title)"
        >
          <Icon name="mdi:close" />
        </button>
      </span>
      <input
        :id="id"
        ref="input"
        v-model="query"
        type="text"
        role="combobox"
        :aria-expanded="showSuggestions"
        aria-autocomplete="list"
        :placeholder="placeholder"
        :disabled="disabled"
        class="min-w-35 flex-1 border-0 bg-transparent px-1 py-1.5 text-[15px] text-text outline-none"
        @focus="focused = true"
        @blur="focused = false"
        @keydown.enter.prevent="addTopMatch"
        @keydown.backspace="removeLastIfEmpty"
      >
    </div>
    <ul
      v-if="showSuggestions"
      role="listbox"
      class="absolute top-full right-0 left-0 z-10 m-0 mt-1.5 max-h-65 list-none overflow-auto rounded-xl border border-line bg-surface2 p-1.5 shadow-[0_24px_60px_-20px_oklch(0_0_0/0.6)]"
    >
      <li v-for="tag in suggestions" :key="tag.id">
        <button
          type="button"
          role="option"
          :aria-selected="false"
          class="flex w-full items-center gap-2.5 rounded-lg px-2.5 py-2.25 text-left text-sm text-text hover:bg-surface"
          @mousedown.prevent="add(tag.title)"
        >
          <Icon :name="getTagIcon(tag)" />
          <span class="flex-1">{{ tag.title }}</span>
          <span class="font-mono text-[11px] text-muted uppercase">{{ tag.type }}</span>
        </button>
      </li>
    </ul>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useTagStore } from '~/store/TagStore'
import { useFormField } from '~/composables/useFormField'
import { getTagIcon } from '~/utils/tag-helper'
import { includesSearch } from '~/utils/string-helper'

interface Props {
  modelValue: string[]
  placeholder?: string
  disabled?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: 'Search by tag name',
  disabled: false,
})

const emit = defineEmits<{ (e: 'update:modelValue', value: string[]): void }>()

const MAX_SUGGESTIONS = 8

const tagStore = useTagStore()
const { id, fieldClass } = useFormField()

const input = ref<HTMLInputElement | null>(null)
const query = ref('')
const focused = ref(false)

const selected = computed(() =>
  props.modelValue.map((title) => tagStore.tags.find((tag) => tag.title === title) ?? { title, type: undefined, icon: undefined }),
)

const suggestions = computed(() =>
  tagStore.tags
    .filter((tag) => !props.modelValue.includes(tag.title) && includesSearch(tag.title, query.value))
    .slice(0, MAX_SUGGESTIONS),
)

const showSuggestions = computed(() => focused.value && suggestions.value.length > 0)

function add(title: string) {
  emit('update:modelValue', [...props.modelValue, title])
  query.value = ''
}

function remove(title: string) {
  emit('update:modelValue', props.modelValue.filter((value) => value !== title))
}

function addTopMatch() {
  const top = suggestions.value[0]
  if (top) add(top.title)
}

function removeLastIfEmpty() {
  const last = props.modelValue[props.modelValue.length - 1]
  if (!query.value && last) remove(last)
}

onMounted(() => tagStore.ensureTags())
</script>
