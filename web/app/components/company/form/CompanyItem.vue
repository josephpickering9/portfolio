<template>
  <div v-if="company" class="flex w-full items-center gap-3">
    <div class="flex-shrink-0 overflow-hidden rounded-full bg-base-100 shadow-sm ring-1 ring-base-content/5" :class="imageClass">
      <img :src="imageUrl" :alt="`${company.name} Logo`" class="h-full w-full object-cover">
    </div>
    <div v-if="!showLink || isEmpty(company.website) || company.website === '#'" class="truncate font-medium leading-none" :class="textClass">
      {{ company.name }}
    </div>
    <a 
      v-else 
      :href="company.website ?? '#'" 
      target="_blank" 
      rel="noreferrer" 
      class="group flex items-center gap-1.5 truncate font-medium leading-none hover:text-primary"
      :class="textClass"
    >
      <span class="truncate">{{ company.name }}</span>
      <Icon name="heroicons:arrow-top-right-on-square" class="h-3.5 w-3.5 opacity-50 transition-opacity group-hover:opacity-100" />
    </a>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { isEmpty } from 'lodash-es'
import type { Company } from '@api'
import { getImageUrl } from '~/utils/image-helper'

interface Props {
  company?: Company | null
  showLink?: boolean
  size?: string
}

const props = withDefaults(defineProps<Props>(), {
  company: null,
  showLink: false,
  size: 'lg',
})

const imageUrl = computed((): string => {
  if (!props.company?.logo) return ''

  return getImageUrl(props.company.logo) ?? ''
})

const textClass = computed(() => ({
  'text-xs': props.size === 'xs',
  'text-sm': props.size === 'sm',
  'text-md': props.size === 'md',
  'text-lg': props.size === 'lg',
}))

const imageClass = computed(() => ({
  'w-3 h-3': props.size === 'xs',
  'w-4 h-4': props.size === 'sm',
  'w-5 h-5': props.size === 'md',
  'w-6 h-6': props.size === 'lg',
}))
</script>
