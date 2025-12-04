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

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import type { SearchItem } from '~/types/SearchItem'
import { ListIcon } from '~/app/assets/data/icons'
import AutoComplete from '~/app/components/forms/elements/AutoComplete.vue'
import IconListItem from '~/app/components/lists/icon/IconListItem.vue'

interface Props {
  label?: string | null
  modelValue?: string | null
}

const props = withDefaults(defineProps<Props>(), {
  label: undefined,
  modelValue: undefined,
})

// Emits
const emit = defineEmits<{
  'update:modelValue': [value: string | undefined]
}>()

const value = ref<SearchItem | undefined>(undefined)

const iconItems = computed((): SearchItem[] => {
  return ListIcon.map((icon) => ({
    title: icon,
    value: icon,
  }))
})

function selectIcon(item?: SearchItem): void {
  emit('update:modelValue', item?.title)
}

// Watch methods
watch(() => props.modelValue, (newValue) => {
  value.value = newValue ? { title: newValue, value: newValue } : undefined
})

watch(value, (newValue) => {
  emit('update:modelValue', newValue?.title)
})
</script>
