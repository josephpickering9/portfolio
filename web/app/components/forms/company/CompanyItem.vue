<template>
  <div v-if="company" class="flex items-center gap-2">
    <img :src="imageUrl" :alt="`${company.name} Logo`" class="m-0 rounded-full" :class="imageClass" >
    <span v-if="!showLink || isEmpty(company.website) || company.website === '#'" class="m-0" :class="textClass">
      {{ company.name }}
    </span>
    <div v-else class="flex items-center gap-1">
      <a :href="company.website ?? '#'" target="_blank" rel="noreferrer" class="m-0">
        {{ company.name }}
      </a>
      <Icon name="mdi:launch" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { isEmpty } from 'lodash-es'
import type { Company } from '~/api'
import { getImageUrl } from '~/app/utils/image-helper'

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
