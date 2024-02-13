<template>
  <div v-if="company" class="textarea textarea-bordered flex w-full justify-between bg-base-100 p-4 shadow-xl">
    <div class="flex w-full flex-col space-y-4">
      <div class="flex items-center gap-2">
        <img v-if="company.logo" :src="logo" class="m-0 h-6 w-6" />
        <h2 v-if="isEmpty(company.website) || company.website === '#'" class="m-0">
          {{ company.name }}
        </h2>
        <div v-else class="flex items-center gap-1">
          <a :href="company.website ?? '#'" target="_blank" rel="noreferrer" class="m-0">
            <h2 class="card-title m-0">{{ company.name }}</h2>
          </a>
          <Icon name="mdi:launch" />
        </div>
      </div>
      <p class="pb-2 text-sm italic">{{ company.description }}</p>
    </div>
    <FormButton
      v-if="isAuthenticated"
      size="xs"
      label="Edit"
      icon="material-symbols:edit"
      :href="`/companies/${company.id}/update`"
    />
  </div>
</template>

<script lang="ts">
import { defineComponent, type PropType } from 'vue'
import { isEmpty } from 'lodash-es'
import type { Company as CompanyModel } from '../../api/models/Company'
import { getImageUrl } from '../../utils/image-helper'
import FormButton from '../forms/FormButton.vue'
import useAuth from '../../composables/useAuth'
import { Icon } from '#components'

export default defineComponent({
  name: 'CompanyListItem',
  components: { FormButton, Icon },
  props: {
    company: {
      type: Object as PropType<CompanyModel>,
      required: true,
    },
  },
  setup() {
    const { isAuthenticated } = useAuth()

    return {
      isAuthenticated,
    }
  },
  computed: {
    logo(): string {
      if (!this.company?.logo) return 'https://via.placeholder.com/320x200'

      return getImageUrl(this.company.logo)
    },
  },
  methods: {
    isEmpty,
  },
})
</script>
