<template>
  <FormElementContainer :label="label">
    <div class="relative">
      <div v-if="!value || !showSelected" class="flex items-center">
        <input
          ref="input"
          v-model="search"
          :type="type"
          :placeholder="placeholder"
          :required="required"
          :disabled="disabled"
          class="input input-bordered w-full pr-10 focus:input-primary"
          :class="inputClass"
          @input="handleInputDebounce"
          @focus="focus"
          @blur="blur"
          @keyup.enter.stop.prevent="onEnter"
          @keydown.arrow-up.stop.prevent="onArrowUp"
          @keydown.arrow-down.stop.prevent="onArrowDown"
          @keydown.escape="close"
        >
        <div v-if="search" class="absolute right-3 flex items-center">
          <button
            type="button"
            tabindex="-1"
            class="flex h-5 w-5 items-center justify-center rounded-full text-base-content/40 hover:bg-base-200 hover:text-base-content"
            @mousedown.stop.prevent="clear"
          >
            <Icon name="heroicons:x-mark" size="14" />
          </button>
        </div>
      </div>

      <div v-else class="flex items-center">
        <div
          class="input input-bordered flex w-full items-center justify-between bg-base-100 pr-3"
          :class="inputClass"
        >
          <slot name="selectedItem">
            <span v-if="value" class="truncate font-medium">{{ value.title }}</span>
          </slot>
          <button
            type="button"
            tabindex="-1"
            class="flex h-5 w-5 items-center justify-center rounded-full text-base-content/40 hover:bg-base-200 hover:text-base-content"
            @mousedown.stop.prevent="clear"
          >
            <Icon name="heroicons:x-mark" size="14" />
          </button>
        </div>
      </div>

      <AutoCompleteDropdown
        :show="showOptions"
        :results="searchResults"
        :highlighted="highlighted"
        :show-no-results="showNoResults"
        :show-results-above="showResultsAbove"
        @select="handleSelect"
        @highlight="setHighlighted"
      >
        <template #item="{ item }">
          <slot name="item" :item="item" />
        </template>
      </AutoCompleteDropdown>
    </div>
  </FormElementContainer>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onBeforeUnmount } from 'vue'
import { debounce, type DebouncedFunc, isEmpty } from 'lodash-es'
import type { SearchItem } from '~/types/SearchItem'
import FormElementContainer from '~/components/ui/form/FormElementContainer.vue'
import AutoCompleteDropdown from './AutoCompleteDropdown.vue'

interface Props {
  label?: string
  modelValue?: SearchItem | undefined
  modelSearch?: string
  data?: SearchItem[]
  showNoResults?: boolean
  placeholder?: string
  type?: string
  required?: boolean
  disabled?: boolean
  showSelected?: boolean
  openOnFocus?: boolean
  size?: string
}

const props = withDefaults(defineProps<Props>(), {
  label: undefined,
  modelValue: undefined,
  modelSearch: '',
  data: () => [],
  showNoResults: false,
  placeholder: undefined,
  type: 'text',
  required: false,
  disabled: false,
  showSelected: true,
  openOnFocus: true,
  size: 'md',
})

const emit = defineEmits<{
  'update:modelValue': [value: SearchItem | undefined]
  'update:modelSearch': [value: string]
  'search': [value: string]
  'select': [value: SearchItem | undefined]
  'focus': []
  'blur': []
  'keyup.enter': []
}>()

const input = ref<HTMLInputElement | null>(null)
const value = ref<SearchItem | undefined>(props.modelValue)
const search = ref('')
const showOptions = ref(false)
const inputPosition = ref({ top: 0, height: 0 })
const showResultsAbove = ref(false)
const highlighted = ref(-1)

const searchResults = computed((): SearchItem[] => {
  const results = isEmpty(search.value)
    ? props.data
    : props.data.filter((item) => item.title.toLowerCase().includes(search.value.toLowerCase()))

  const ordered = showResultsAbove.value ? [...results].reverse() : results

  return ordered.slice(0, 100)
})

const handleInputDebounce = computed((): DebouncedFunc<() => void> => debounce(handleInput, 0))

const inputClass = computed(() => ({
  'input-xs': props.size === 'xs',
  'input-sm': props.size === 'sm',
  'input-md': props.size === 'md',
  'input-lg': props.size === 'lg',
}))

function handleInput() {
  emit('search', search.value)
  showOptions.value = true
  adjustResultsPosition()
}

function handleSelect(item: SearchItem) {
  emit('select', item)
  emit('update:modelValue', item)
  showOptions.value = false
  search.value = ''
}

function focus() {
  if (props.openOnFocus && searchResults.value.length) {
    showOptions.value = true
    adjustResultsPosition()
  }
  emit('focus')
}

function blur() {
  showOptions.value = false
  emit('blur')
}

function onEnter() {
  if (highlighted.value !== -1) {
    handleSelect(searchResults.value[highlighted.value]!)
  } else if (value.value) {
    handleSelect(value.value)
  }
}

function onArrowUp() {
  if (showResultsAbove.value) {
    highlighted.value = highlighted.value < searchResults.value.length - 1
      ? highlighted.value + 1
      : 0
  } else {
    highlighted.value = highlighted.value > 0
      ? highlighted.value - 1
      : searchResults.value.length - 1
  }
}

function onArrowDown() {
  showOptions.value = true
  if (showResultsAbove.value) {
    highlighted.value = highlighted.value > 0
      ? highlighted.value - 1
      : searchResults.value.length - 1
  } else {
    highlighted.value = highlighted.value < searchResults.value.length - 1
      ? highlighted.value + 1
      : 0
  }
}

function setHighlighted(index: number) {
  highlighted.value = index
}

function adjustResultsPosition() {
  const inputElement = input.value
  if (!inputElement) return
  const rect = inputElement.getBoundingClientRect()
  inputPosition.value = { top: rect.top, height: rect.height }
  const spaceBelow = window.innerHeight - rect.top - rect.height
  showResultsAbove.value = spaceBelow < 250 && rect.top > spaceBelow
}

function clear() {
  value.value = undefined
  emit('select', undefined)
  emit('update:modelValue', undefined)
}

function close() {
  showOptions.value = false
}

onMounted(() => {
  adjustResultsPosition()
  window.addEventListener('resize', adjustResultsPosition)
})

onBeforeUnmount(() => {
  window.removeEventListener('resize', adjustResultsPosition)
})

watch(() => props.modelValue, (newValue) => {
  if (value.value?.value === newValue?.value) return
  value.value = newValue
})

watch(value, (newValue) => {
  if (newValue?.value === props.modelValue?.value) return
  emit('update:modelValue', newValue)
})

watch(() => props.modelSearch, (newValue) => { search.value = newValue })
watch(search, (newValue) => { emit('update:modelSearch', newValue) })

watch(searchResults, () => {
  highlighted.value = searchResults.value.length === 1 ? 0 : -1
})
</script>
