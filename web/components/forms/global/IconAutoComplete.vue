<template>
  <AutoComplete v-model="value" :data="iconItems" :label="label" placeholder="Search" @select="selectIcon">
    <template #item="{ item }">
      <IconListItem :icon="item.title" />
    </template>
    <template #selectedItem>
      <IconListItem v-if="value" :icon="value.title" />
    </template>
  </AutoComplete>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import type { SearchItem } from '../../../types/SearchItem'
import { ListIcon } from '../../../assets/data/icons'
import AutoComplete from '../AutoComplete.vue'
import IconListItem from '../../lists/IconListItem.vue'

interface Data {
  value?: SearchItem
}

export default defineComponent({
  name: 'IconAutoComplete',
  components: { AutoComplete, IconListItem },
  props: {
    label: {
      type: String,
      default: null,
    },
    modelValue: {
      type: String,
      default: null,
    },
  },
  emits: ['update:modelValue'],
  data(): Data {
    return {
      value: undefined,
    }
  },
  computed: {
    iconItems(): SearchItem[] {
      return ListIcon.map((icon) => {
        return {
          title: icon,
          value: icon,
        }
      })
    },
  },
  methods: {
    selectIcon(item?: SearchItem): void {
      this.$emit('update:modelValue', item?.title)
    },
  },
  watch: {
    modelValue() {
      this.value = { title: this.modelValue, value: this.modelValue }
    },
    value() {
      this.$emit('update:modelValue', this.value?.title)
    },
  },
})
</script>
