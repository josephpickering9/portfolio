<template>
  <div v-if="tag" class="badge flex items-center space-x-1" :class="tagClass" :style="tagStyle">
    <Icon v-if="tag.icon" :name="tag.icon" />
    <span class="font-bold">{{ tag.title }}</span>
    <button v-if="clearable" class="flex items-center" @click="$emit('remove', tag)">
      <Icon name="material-symbols:delete" />
    </button>
  </div>
</template>

<script lang="ts">
import { defineComponent, type PropType, ref, type StyleValue } from 'vue'
import type { Tag } from '../../api'
import { TagType } from '../../api'
import { setTextColourForBackground } from '../../utils/colour-helper'
import { Icon } from '#components'

export default defineComponent({
  name: 'Tag',
  components: { Icon },
  props: {
    tag: {
      type: Object as PropType<Tag>,
      required: true,
    },
    outline: {
      type: Boolean,
      default: false,
    },
    clearable: {
      type: Boolean,
      default: false,
    },
  },
  emits: ['remove'],
  computed: {
    tagClass(): object {
      return {
        'badge-outline': this.outline,
        'badge-error': this.tag.type === ref<TagType>(TagType.BACKEND).value,
        'badge-success': this.tag.type === ref<TagType>(TagType.FRONTEND).value,
        'badge-default': this.tag.type === ref<TagType>(TagType.OTHER).value,
        'badge-primary': this.tag.type === ref<TagType>(TagType.DEV_OPS).value,
        'badge-neutral': this.tag.type === ref<TagType>(TagType.DEFAULT).value,
        'badge-accent': this.tag.type === ref<TagType>(TagType.CMS).value,
        'badge-info': this.tag.type === ref<TagType>(TagType.MOBILE).value,
        'badge-warning': this.tag.type === ref<TagType>(TagType.DATA).value,
        [setTextColourForBackground(this.tag.customColour ?? '')]: this.tag.customColour,
      }
    },
    tagStyle(): StyleValue {
      if (!this.tag.customColour) return {}

      return {
        'background-color': this.tag.customColour,
        'border-color': this.tag.customColour,
      }
    },
  },
})
</script>
