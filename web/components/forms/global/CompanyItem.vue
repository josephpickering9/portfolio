<template>
  <div v-if="company" class="flex w-full items-center gap-2">
    <img :src="imageUrl" class="m-0 h-6 w-6 rounded-full" />
    <span v-if="!showLink" class="m-0 text-lg font-semibold">{{ company.name }}</span>
    <div v-else class="flex items-center gap-1">
      <a :href="company.website ?? '#'" target="_blank" rel="noreferrer" class="m-0 text-lg font-semibold">
        {{ company.name }}
      </a>
      <Icon name="mdi:launch" />
    </div>
  </div>
</template>

<script lang="ts">
import type { PropType } from 'vue'
import { defineComponent } from 'vue'
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
  },
  computed: {
    imageUrl(): string {
      if (!this.company.logo) return ''

      return getImageUrl(this.company.logo) ?? ''
    },
  },
})
</script>
