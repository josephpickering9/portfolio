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
import { ListIcon } from '~/assets/data/icons'
import AutoComplete from '~/components/ui/input/AutoComplete.vue'
import IconListItem from '~/components/repository/IconListItem.vue'

interface Props {
  label?: string | null
  modelValue?: string | null
}

const props = withDefaults(defineProps<Props>(), {
  label: undefined,
  modelValue: undefined,
})


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


watch(() => props.modelValue, (newValue) => {
  value.value = newValue ? { title: newValue, value: newValue } : undefined
})

watch(value, (newValue) => {
  emit('update:modelValue', newValue?.title)
})
</script>
