<template>
  <Transition
    enter-active-class="transition ease-out duration-200"
    enter-from-class="transform opacity-0 scale-95"
    enter-to-class="transform opacity-100 scale-100"
    leave-active-class="transition ease-in duration-75"
    leave-from-class="transform opacity-100 scale-100"
    leave-to-class="transform opacity-0 scale-95"
  >
    <div
      v-show="show && results.length"
      ref="container"
      tabindex="0"
      :class="[resultsClass, 'absolute z-50 my-1 max-h-60 w-full min-w-[320px] overflow-hidden overflow-y-auto rounded-xl bg-base-100 p-1 shadow-lg ring-1 ring-base-content/10']"
    >
      <ul class="m-0 w-full list-none p-0">
        <li
          v-for="(result, index) in results"
          :key="index"
          ref="items"
          class="group relative m-0 flex cursor-pointer select-none items-center rounded-lg px-3 py-2 transition-colors hover:bg-base-200"
          :class="{ 'bg-base-200 text-base-content': highlighted === index, 'text-base-content/80': highlighted !== index }"
          role="option"
          :aria-selected="highlighted === index"
          tabindex="0"
          @mousedown.stop.prevent="$emit('select', result)"
          @click.stop.prevent="$emit('select', result)"
          @mouseenter="$emit('highlight', index)"
          @focus="$emit('highlight', index)"
        >
          <slot name="item" :item="result">
            {{ result.title }}
          </slot>
        </li>
        <li v-if="!results.length && showNoResults" class="px-3 py-2 text-center text-sm text-base-content/60">
          No Matching Results
        </li>
      </ul>
    </div>
  </Transition>
</template>

<script setup lang="ts">
import { ref, computed, nextTick, watch } from 'vue'
import type { SearchItem } from '~/types/SearchItem'

interface Props {
  show: boolean
  results: SearchItem[]
  highlighted: number
  showNoResults?: boolean
  showResultsAbove?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  showNoResults: false,
  showResultsAbove: false,
})

const emit = defineEmits<{
  'select': [item: SearchItem]
  'highlight': [index: number]
}>()

const container = ref<HTMLElement | null>(null)
const items = ref<HTMLElement[]>([])

const resultsClass = computed(() => ({
  'bottom-full': props.showResultsAbove,
  'top-full': !props.showResultsAbove,
}))

function scrollToHighlighted() {
  nextTick(() => {
    if (!container.value) return

    if (props.highlighted === -1) {
      container.value.scrollTop = props.showResultsAbove ? container.value.scrollHeight : 0
      return
    }

    const highlightedElement = items.value[props.highlighted]
    if (!highlightedElement) return

    const itemTop = highlightedElement.offsetTop
    const itemBottom = itemTop + highlightedElement.offsetHeight
    const containerScrollTop = container.value.scrollTop
    const containerBottom = containerScrollTop + container.value.offsetHeight

    if (props.showResultsAbove) {
      if (itemTop < containerScrollTop || itemBottom > containerBottom) {
        container.value.scrollTop = itemBottom - container.value.offsetHeight
      }
    } else if (itemTop < containerScrollTop || itemBottom > containerBottom) {
      container.value.scrollTop = itemTop
    }
  })
}

watch(() => props.highlighted, scrollToHighlighted)

defineExpose({ scrollToHighlighted })
</script>
