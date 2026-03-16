<template>
  <div v-if="company" class="textarea textarea-bordered flex w-full justify-between bg-base-100 p-4 shadow-xl">
    <div class="flex w-full flex-col space-y-4">
      <div class="flex items-center gap-2">
        <NuxtImg v-if="company.logo" :src="logo" :alt="`${company.name} Logo`" placeholder format="webp" class="m-0 h-6 w-6" />
        <h2 v-if="isEmpty(company.website) || company.website === '#'" class="card-title m-0">
          {{ company.name }}
        </h2>
        <div v-else class="flex items-center gap-1">
          <a :href="company.website ?? '#'" target="_blank" rel="noreferrer" class="m-0">
            <h2 class="card-title m-0">{{ company.name }}</h2>
          </a>
          <Icon name="mdi:launch" />
        </div>
      </div>
      <div v-if="dateRange || duration" class="flex items-center gap-2 text-sm italic text-base-content/60">
        <p v-if="dateRange" class="m-0">{{ dateRange }}</p>
        <span v-if="dateRange && duration" class="text-base-content/40">•</span>
        <p v-if="duration" class="m-0">{{ duration }}</p>
      </div>
      <div class="prose max-w-none pb-2 italic" v-html="company.description" />
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

<script setup lang="ts">
import { computed } from 'vue'
import { isEmpty } from 'lodash-es'
import type { Company as CompanyModel } from '@api'
import { getImageUrl } from '~/utils/image-helper'
import { getDateRange, getDuration } from '~/utils/date-helper'
import useAuth from '~/composables/useAuth'
import FormButton from '~/components/ui/form/FormButton.vue'

interface Props {
  company: CompanyModel
}

const props = defineProps<Props>()

const { isAuthenticated } = useAuth()

const logo = computed((): string => {
  if (!props.company?.logo) return 'https://via.placeholder.com/320x200'

  return getImageUrl(props.company.logo)
})

const dateRange = computed(() => getDateRange(props.company))
const duration = computed(() => getDuration(props.company))
</script>
