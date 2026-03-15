<template>
  <div class="flex flex-col gap-1 p-1" @keydown="handleKeydown">
    <button
      v-for="(tagType, index) in tagTypes"
      :key="tagType.value"
      ref="itemRefs"
      type="button"
      class="btn btn-ghost focus-visible:outline-none justify-start gap-2 hover:bg-base-200 h-auto"
      :class="[mobile ? 'py-3 text-base' : 'btn-sm py-2', { 'ring-2 ring-primary': focusedIndex === index }]"
      @click="selectTagType(tagType)"
      @mouseenter="focusedIndex = index"
    >
      <Icon :name="tagType.icon" size="1.2em" :class="tagType.color" />
      <span>{{ tagType.label }}</span>
    </button>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, nextTick } from 'vue'
import { Icon } from '#components'
import type { TagTypeValue } from '~/types/TagTypeValue';
import { tagTypes } from '~/data/TagTypes';

interface Props {
  mobile?: boolean
}

defineProps<Props>()

const emit = defineEmits<{
  'select': [tagType: TagTypeValue]
  'close': []
}>()

const focusedIndex = ref(0)
const itemRefs = ref<HTMLElement[]>([])

function selectTagType(tagType: TagTypeValue) {
  emit('select', tagType)
}

function handleKeydown(event: KeyboardEvent) {
  // Stop propagation to prevent main menu from handling these events
  event.stopPropagation()
  
  switch (event.key) {
    case 'ArrowDown':
      event.preventDefault()
      focusedIndex.value = Math.min(focusedIndex.value + 1, tagTypes.length - 1)
      scrollToFocusedItem()
      break
    case 'ArrowUp':
      event.preventDefault()
      focusedIndex.value = Math.max(focusedIndex.value - 1, 0)
      scrollToFocusedItem()
      break
    case 'ArrowRight':
      event.preventDefault()
      emit('close')
      break
    case 'ArrowLeft':
      event.preventDefault()
      emit('close')
      break
    case 'Enter': {
      event.preventDefault()
      const tagType = tagTypes[focusedIndex.value]
      if (tagType) selectTagType(tagType)
      break
    }
  }
}

function scrollToFocusedItem() {
  nextTick(() => {
    const focusedElement = itemRefs.value[focusedIndex.value]
    if (focusedElement) {
      focusedElement.scrollIntoView({ block: 'nearest', behavior: 'smooth' })
    }
  })
}

onMounted(() => {
  // Focus first tag type on mount
  focusedIndex.value = 0
  nextTick(() => {
    const firstElement = itemRefs.value[0]
    if (firstElement) {
      firstElement.focus()
    }
  })
})
</script>
