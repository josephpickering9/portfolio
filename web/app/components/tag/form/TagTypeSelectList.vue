<template>
  <SelectList
    v-model="value"
    :label="label"
    :options="tagTypes"
    :disabled="disabled"
    :size="size"
    :placeholder="placeholder"
    :clearable="clearable"
  />
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { TagType, type Tag } from '@api'
import { useTagStore } from '~/store/TagStore'
import type { SelectListItem } from '@types/SelectListItem'
import { enumToSelectListItem } from '~/utils/enum-helper'
import SelectList from '~/components/ui/input/SelectList.vue'

interface Props {
  label?: string | null
  modelValue?: TagType | null
  placeholder?: string | null
  disabled?: boolean
  size?: string | null
  clearable?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  label: undefined,
  modelValue: undefined,
  placeholder: undefined,
  disabled: false,
  size: null,
  clearable: true,
})

// Emits
const emit = defineEmits<{
  'update:modelValue': [value: TagType]
}>()

const tagStore = useTagStore()

const value = ref<TagType>(TagType.DEFAULT)

const tags = computed((): Tag[] => {
  return tagStore.tags
})

const tagTypes = computed((): SelectListItem[] => {
  return enumToSelectListItem(TagType)
})

onMounted(async () => {
  if (!tags.value.length) await tagStore.getTags()
})

// Watch methods
watch(() => props.modelValue, (newValue: TagType | null) => {
  if (newValue) value.value = newValue
}, { immediate: true })

watch(value, (newValue: TagType) => {
  emit('update:modelValue', newValue)
})
</script>
