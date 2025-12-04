<template>
  <div v-if="tag" class="badge flex items-center space-x-1" :class="tagClass" :style="tagStyle">
    <Icon v-if="tag.icon" :name="tag.icon" />
    <span class="font-bold">{{ tag.title }}</span>
    <button v-if="clearable" class="flex items-center" @click="$emit('remove', tag)">
      <Icon name="material-symbols:delete" />
    </button>
  </div>
</template>

<script setup lang="ts">
import { computed, type StyleValue } from 'vue'
import type { Tag } from '@api'
import { TagType } from '@api'
import { setTextColourForBackground } from '~/utils/colour-helper'

interface Props {
  tag: Tag
  outline?: boolean
  clearable?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  outline: false,
  clearable: false,
})

// Emits
const emit = defineEmits<{
  remove: [tag: Tag]
}>()

const tagClass = computed(() => ({
  'badge-outline': props.outline,
  'badge-error': props.tag.type === TagType.BACKEND,
  'badge-success': props.tag.type === TagType.FRONTEND,
  'badge-default': props.tag.type === TagType.OTHER,
  'badge-primary': props.tag.type === TagType.DEV_OPS,
  'badge-neutral': props.tag.type === TagType.DEFAULT,
  'badge-accent': props.tag.type === TagType.CMS,
  'badge-info': props.tag.type === TagType.MOBILE,
  'badge-warning': props.tag.type === TagType.DATA,
  [setTextColourForBackground(props.tag.customColour ?? '')]: props.tag.customColour,
}))

const tagStyle = computed((): StyleValue => {
  if (!props.tag.customColour) return {}

  return {
    'background-color': props.tag.customColour,
    'border-color': props.tag.customColour,
  }
})
</script>
