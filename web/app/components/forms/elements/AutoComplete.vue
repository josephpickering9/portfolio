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
          class="input w-full pr-8"
          :class="inputClass"
          @input="handleInputDebounce"
          @focus="focus"
          @blur="blur"
          @keyup.enter.stop.prevent="onEnter"
          @keydown.arrow-up.stop.prevent="onArrowUp"
          @keydown.arrow-down.stop.prevent="onArrowDown"
          @keydown.escape="close"
        >
      </div>
      <div v-else class="flex items-center">
        <div class="input flex w-full items-center pr-8" :class="inputClass">
          <slot name="selectedItem">
            <span v-if="value" class="truncate">{{ value.title }}</span>
          </slot>
        </div>
        <button type="button" tabindex="-1" class="absolute right-4 text-xs" @mousedown.stop.prevent="clear">X</button>
      </div>
      <div
        v-show="showOptions && searchResults.length"
        ref="results"
        tabindex="0"
        :class="resultsClass"
        class="input absolute z-50 my-2 h-auto max-h-60 w-full min-w-72 overflow-hidden overflow-y-auto rounded-md p-0 shadow"
      >
        <ul class="m-0 w-full list-none bg-base-100 p-0">
          <li
            v-for="(result, index) in searchResults"
            :key="index"
            ref="item"
            class="m-0 cursor-pointer select-none p-4 hover:bg-gray-700"
            :class="{ 'bg-gray-700': highlighted === index }"
            role="option"
            :aria-selected="highlighted === index"
            tabindex="0"
            @mousedown.stop.prevent="handleSelect(result)"
            @mouseenter="setHighlighted(index)"
            @focus="setHighlighted(index)"
          >
            <slot name="item" :item="result">
              {{ result.title }}
            </slot>
          </li>
          <li v-if="!searchResults.length && showNoResults" class="px-3 py-2 text-center">No Matching Results</li>
        </ul>
      </div>
    </div>
  </FormElementContainer>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted, onBeforeUnmount, nextTick } from 'vue'
import { debounce, type DebouncedFunc, isEmpty } from 'lodash-es'
import type { SearchItem } from '~/types/SearchItem'
import FormElementContainer from '~/app/components/forms/elements/FormElementContainer.vue'

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

// Emits
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
const results = ref<HTMLElement | null>(null)
const item = ref<HTMLElement[]>([])
const value = ref<SearchItem | undefined>(props.modelValue)
const search = ref('')
const showOptions = ref(false)
const inputPosition = ref({ top: 0, height: 0 })
const showResultsAbove = ref(false)
const highlighted = ref(-1)

const searchResults = computed((): SearchItem[] => {
  const results = isEmpty(search.value)
    ? props.data
    : props.data.filter((item) => {
        return item.title.toLowerCase().includes(search.value.toLowerCase())
      })

  const ordered = showResultsAbove.value ? results.reverse() : results

  return ordered.slice(0, 100)
})

const handleInputDebounce = computed((): DebouncedFunc<() => any> => {
  return debounce(handleInput, 0)
})

const resultsClass = computed(() => ({
  'bottom-full': showResultsAbove.value,
  'top-full': !showResultsAbove.value,
}))

const inputClass = computed(() => ({
  'input-xs': props.size === 'xs',
  'input-sm': props.size === 'sm',
  'input-md': props.size === 'md',
  'input-lg': props.size === 'lg',
}))

function handleInput() {
  emit('search', search.value)
  showOptions.value = true
  checkScrollPosition()
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
    checkScrollPosition()
  }

  emit('focus')
}

function blur() {
  showOptions.value = false
  emit('blur')
}

function onEnter() {
  if (highlighted.value !== -1) {
    handleSelect(searchResults.value[highlighted.value])
  } else if (value.value) {
    handleSelect(value.value)
  }
}

function onArrowUp() {
  incrementHighlighted()
}

function onArrowDown() {
  showOptions.value = true
  decrementHighlighted()
}

function setHighlighted(index: number) {
  highlighted.value = index
}

function incrementHighlighted() {
  if (showResultsAbove.value) {
    if (highlighted.value > 0) {
      highlighted.value--
    } else {
      highlighted.value = searchResults.value.length - 1
    }
  } else if (highlighted.value > 0) {
    highlighted.value--
  } else {
    highlighted.value = searchResults.value.length - 1
  }

  checkScrollPosition()
}

function decrementHighlighted() {
  if (showResultsAbove.value) {
    if (highlighted.value < searchResults.value.length - 1) {
      highlighted.value++
    } else {
      highlighted.value = 0
    }
  } else if (highlighted.value < searchResults.value.length - 1) {
    highlighted.value++
  } else {
    highlighted.value = 0
  }

  checkScrollPosition()
}

function calculateInputPosition() {
  const inputElement = input.value
  if (inputElement) {
    const rect = inputElement.getBoundingClientRect()
    inputPosition.value.top = rect.top
    inputPosition.value.height = rect.height
    adjustResultsPosition()
  }
}

function adjustResultsPosition() {
  const spaceAbove = inputPosition.value.top
  const spaceBelow = window.innerHeight - inputPosition.value.top - inputPosition.value.height
  showResultsAbove.value = spaceBelow < 250 && spaceAbove > spaceBelow
}

function checkScrollPosition() {
  nextTick(() => {
    const resultsElement = results.value
    if (resultsElement) {
      if (highlighted.value === -1) {
        resultsElement.scrollTop = showResultsAbove.value ? resultsElement.scrollHeight : 0
      }

      const highlightedElement = item.value[highlighted.value]
      if (highlightedElement) {
        const itemTop = highlightedElement.offsetTop
        const itemBottom = itemTop + highlightedElement.offsetHeight
        const containerScrollTop = resultsElement.scrollTop
        const containerBottom = containerScrollTop + resultsElement.offsetHeight

        if (showResultsAbove.value) {
          if (itemTop < containerScrollTop || itemBottom > containerBottom) {
            resultsElement.scrollTop = itemBottom - resultsElement.offsetHeight
          }
        } else if (itemTop < containerScrollTop || itemBottom > containerBottom) {
          resultsElement.scrollTop = itemTop
        }
      }
    }
  })
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
  calculateInputPosition()
  window.addEventListener('resize', calculateInputPosition)
})

onBeforeUnmount(() => {
  window.removeEventListener('resize', calculateInputPosition)
})

// Watch methods
watch(() => props.modelValue, (newValue) => {
  if (value.value?.value === newValue?.value) return

  value.value = newValue
})

watch(value, (newValue) => {
  if (newValue?.value === props.modelValue?.value) return

  emit('update:modelValue', newValue)
})

watch(() => props.modelSearch, (newValue) => {
  search.value = newValue
})

watch(search, (newValue) => {
  emit('update:modelSearch', newValue)
})

watch(searchResults, () => {
  highlighted.value = -1

  if (searchResults.value.length === 1) highlighted.value = 0
})
</script>
