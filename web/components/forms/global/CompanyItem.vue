<template>
  <div v-if="company" class="flex w-full items-center gap-2">
    <img :src="imageUrl" class="m-0 rounded-full" :class="imageClass" />
    <span
      v-if="!showLink || isEmpty(company.website) || company.website === '#'"
      class="m-0 font-semibold"
      :class="textClass"
    >
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

<script lang="ts">
import type { PropType } from 'vue'
import { defineComponent } from 'vue'
import { isEmpty } from 'lodash-es'
import type { Company } from '../../../api'
import { getImageUrl } from '../../../utils/image-helper'
import { Icon } from '#components'

export default defineComponent({
  name: 'CompanyItem',
  components: { Icon },
  props: {
    company: {
      type: Object as PropType<Company>,
      default: () => null,
    },
    showLink: {
      type: Boolean,
      default: false,
    },
    size: {
      type: String,
      default: 'lg',
    },
  },
  computed: {
    imageUrl(): string {
      if (!this.company.logo) return ''

      return getImageUrl(this.company.logo) ?? ''
    },
    textClass(): object {
      return {
        'text-xs': this.size === 'xs',
        'text-sm': this.size === 'sm',
        'text-md': this.size === 'md',
        'text-lg': this.size === 'lg',
      }
    },
    imageClass(): object {
      return {
        'w-3 h-3': this.size === 'xs',
        'w-4 h-4': this.size === 'sm',
        'w-5 h-5': this.size === 'md',
        'w-6 h-6': this.size === 'lg',
      }
    },
  },
  methods: {
    isEmpty,
  },
})
</script>
