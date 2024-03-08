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
          class="input input-bordered w-full pr-8"
          :class="inputClass"
          @input="handleInputDebounce"
          @focus="focus"
          @blur="blur"
          @keyup.enter.stop.prevent="onEnter"
          @keydown.arrow-up.stop.prevent="onArrowUp"
          @keydown.arrow-down.stop.prevent="onArrowDown"
          @keydown.escape="close"
        />
      </div>
      <div v-else class="flex items-center">
        <div class="input input-bordered flex w-full items-center pr-8" :class="inputClass">
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
        class="input input-bordered absolute z-50 my-2 h-auto max-h-60 w-full min-w-72 overflow-hidden overflow-y-auto rounded-md p-0 shadow"
      >
        <ul class="m-0 w-full list-none bg-base-100 p-0">
          <li
            v-for="(item, index) in searchResults"
            :key="index"
            ref="item"
            class="m-0 cursor-pointer select-none p-4 hover:bg-gray-700"
            :class="{ 'bg-gray-700': highlighted === index }"
            role="option"
            :aria-selected="highlighted === index"
            tabindex="0"
            @mousedown.stop.prevent="handleSelect(item)"
            @mouseenter="setHighlighted(index)"
            @focus="setHighlighted(index)"
          >
            <slot name="item" :item="item">
              {{ item.title }}
            </slot>
          </li>
          <li v-if="!searchResults.length && showNoResults" class="px-3 py-2 text-center">No Matching Results</li>
        </ul>
      </div>
    </div>
  </FormElementContainer>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import type { PropType } from 'vue'
import { debounce, type DebouncedFunc, isEmpty } from 'lodash-es'
import type { SearchItem } from '../../types/SearchItem'
import FormElementContainer from './FormElementContainer.vue'

interface Data {
  value?: SearchItem
  search: string
  showOptions: boolean
  inputPosition: { top: number; height: number }
  showResultsAbove: boolean
  highlighted: number
  itemHighlighted: number
}

export default defineComponent({
  name: 'AutoComplete',
  components: { FormElementContainer },
  props: {
    label: {
      type: String,
      default: null,
    },
    modelValue: {
      type: Object as PropType<SearchItem>,
      default: undefined,
    },
    modelSearch: {
      type: String,
      default: '',
    },
    data: {
      type: Array as PropType<SearchItem[]>,
      default: () => [],
    },
    showNoResults: {
      type: Boolean,
      default: false,
    },
    placeholder: {
      type: String,
      default: null,
    },
    type: {
      type: String,
      default: 'text',
    },
    required: {
      type: Boolean,
      default: false,
    },
    disabled: {
      type: Boolean,
      default: false,
    },
    showSelected: {
      type: Boolean,
      default: true,
    },
    openOnFocus: {
      type: Boolean,
      default: true,
    },
    size: {
      type: String,
      default: 'md',
    },
  },
  emits: ['update:modelValue', 'update:modelSearch', 'search', 'select', 'focus', 'blur', 'keyup.enter'],
  data(): Data {
    return {
      value: this.modelValue,
      search: '',
      showOptions: false,
      inputPosition: { top: 0, height: 0 },
      showResultsAbove: false,
      highlighted: -1,
      itemHighlighted: -1,
    }
  },
  computed: {
    searchResults(): SearchItem[] {
      const results = isEmpty(this.search)
        ? this.data
        : this.data.filter((item) => {
            return item.title.toLowerCase().includes(this.search.toLowerCase())
          })

      const ordered = this.showResultsAbove ? results.reverse() : results

      return ordered.slice(0, 100)
    },
    handleInputDebounce(): DebouncedFunc<() => any> {
      return debounce(this.handleInput, 0)
    },
    resultsClass(): object {
      return {
        'bottom-full': this.showResultsAbove,
        'top-full': !this.showResultsAbove,
      }
    },
    inputClass(): object {
      return {
        'input-xs': this.size === 'xs',
        'input-sm': this.size === 'sm',
        'input-md': this.size === 'md',
        'input-lg': this.size === 'lg',
      }
    },
  },
  mounted() {
    this.calculateInputPosition()
    window.addEventListener('resize', this.calculateInputPosition)
  },
  beforeUnmount() {
    window.removeEventListener('resize', this.calculateInputPosition)
  },
  methods: {
    handleInput() {
      this.$emit('search', this.search)
      this.showOptions = true
      this.checkScrollPosition()
    },
    handleSelect(item: SearchItem) {
      this.$emit('select', item)
      this.$emit('update:modelValue', item)
      this.value = item
      this.showOptions = false
      this.search = ''
    },
    focus() {
      if (this.openOnFocus && this.searchResults.length) {
        this.showOptions = true
        this.checkScrollPosition()
      }

      this.$emit('focus')
    },
    blur() {
      this.showOptions = false
      this.$emit('blur')
    },
    onEnter() {
      if (this.highlighted !== -1) {
        this.handleSelect(this.searchResults[this.highlighted])
      } else if (this.value) {
        this.handleSelect(this.value)
      }
    },
    onArrowUp() {
      this.incrementHighlighted()
    },
    onArrowDown() {
      this.showOptions = true
      this.decrementHighlighted()
    },
    setHighlighted(index: number) {
      this.highlighted = index
    },
    incrementHighlighted() {
      if (this.showResultsAbove) {
        if (this.highlighted > 0) {
          this.highlighted--
        } else {
          this.highlighted = this.searchResults.length - 1
        }
      } else if (this.highlighted > 0) {
        this.highlighted--
      } else {
        this.highlighted = this.searchResults.length - 1
      }

      this.checkScrollPosition()
    },
    decrementHighlighted() {
      if (this.showResultsAbove) {
        if (this.highlighted < this.searchResults.length - 1) {
          this.highlighted++
        } else {
          this.highlighted = 0
        }
      } else if (this.highlighted < this.searchResults.length - 1) {
        this.highlighted++
      } else {
        this.highlighted = 0
      }

      this.checkScrollPosition()
    },
    calculateInputPosition() {
      const inputElement = this.$refs.input as HTMLElement
      if (inputElement) {
        const rect = inputElement.getBoundingClientRect()
        this.inputPosition.top = rect.top
        this.inputPosition.height = rect.height
        this.adjustResultsPosition()
      }
    },
    adjustResultsPosition() {
      const spaceAbove = this.inputPosition.top
      const spaceBelow = window.innerHeight - this.inputPosition.top - this.inputPosition.height
      this.showResultsAbove = spaceBelow < 250 && spaceAbove > spaceBelow
    },
    checkScrollPosition() {
      this.$nextTick(() => {
        const resultsElement = this.$refs.results as HTMLElement
        if (resultsElement) {
          if (this.highlighted === -1) {
            resultsElement.scrollTop = this.showResultsAbove ? resultsElement.scrollHeight : 0
          }

          // @ts-ignore
          const highlightedItem = this.$refs.item[this.highlighted] as HTMLElement
          if (highlightedItem) {
            const itemTop = highlightedItem.offsetTop
            const itemBottom = itemTop + highlightedItem.offsetHeight
            const containerScrollTop = resultsElement.scrollTop
            const containerBottom = containerScrollTop + resultsElement.offsetHeight

            if (this.showResultsAbove) {
              if (itemTop < containerScrollTop || itemBottom > containerBottom) {
                resultsElement.scrollTop = itemBottom - resultsElement.offsetHeight
              }
            } else if (itemTop < containerScrollTop || itemBottom > containerBottom) {
              resultsElement.scrollTop = itemTop
            }
          }
        }
      })
    },
    clear() {
      this.value = undefined
      this.$emit('select', undefined)
      this.$emit('update:modelValue', undefined)
    },
    close() {
      this.showOptions = false
    },
  },
  watch: {
    modelValue() {
      if (this.value?.value === this.modelValue?.value) return

      this.value = this.modelValue
    },
    value() {
      if (this.value?.value === this.modelValue?.value) return

      this.$emit('update:modelValue', this.value)
    },
    modelSearch() {
      this.search = this.modelSearch
    },
    search() {
      this.$emit('update:modelSearch', this.search)
    },
    searchResults() {
      this.highlighted = -1

      if (this.searchResults.length === 1) this.highlighted = 0
    },
  },
})
</script>
