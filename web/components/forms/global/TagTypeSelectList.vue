<template>
  <SelectList v-model="value" :label="label" :options="tagTypes" :disabled="disabled" :size="size" />
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import type { PropType } from 'vue'
import SelectList from '../SelectList.vue'
import { TagType, type Tag } from '../../../api'
import { useTagStore } from '../../../store/TagStore'
import type { SelectListItem } from '../../../types/SelectListItem'
import { enumToSelectListItem } from '../../../utils/enum-helper'

interface Data {
  value: TagType
}

export default defineComponent({
  name: 'TagTypeSelectList',
  components: { SelectList },
  props: {
    label: {
      type: String,
      default: null,
    },
    modelValue: {
      type: String as PropType<TagType>,
      default: null,
    },
    disabled: {
      type: Boolean,
      default: false,
    },
    size: {
      type: String,
      default: null,
    },
  },
  emits: ['update:modelValue'],
  data(): Data {
    return {
      value: TagType.DEFAULT,
    }
  },
  computed: {
    tags(): Tag[] {
      return useTagStore().tags
    },
    tagTypes(): SelectListItem[] {
      return enumToSelectListItem(TagType)
    },
  },
  async mounted() {
    if (!this.tags.length) await useTagStore().getTags()
  },
  watch: {
    modelValue: {
      immediate: true,
      handler(value: TagType): void {
        this.value = value
      },
    },
    value(value: TagType): void {
      this.$emit('update:modelValue', value)
    },
  },
})
</script>
