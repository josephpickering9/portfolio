<template>
  <div v-if="company" class="textarea textarea-bordered flex w-full justify-between bg-base-100 p-4 shadow-xl">
    <div class="flex w-full flex-col space-y-4">
      <div class="flex items-center gap-2">
        <img v-if="company.logo" :src="logo" :alt="`${company.name} Logo`" class="m-0 h-6 w-6" >
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
      <div v-if="dateRange || duration" class="flex items-center gap-2 text-xs italic text-base-content/60">
        <p v-if="dateRange" class="m-0">{{ dateRange }}</p>
        <span v-if="dateRange && duration" class="text-base-content/40">•</span>
        <p v-if="duration" class="m-0">{{ duration }}</p>
      </div>
      <div class="pb-2 text-sm italic" v-html="company.description" />
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
import { format, formatDuration, intervalToDuration, parseISO } from 'date-fns'
import type { Company as CompanyModel } from '@api'
import { getImageUrl } from '~/utils/image-helper'
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

const dateRange = computed((): string => {
  const start = props.company.startDate ? format(parseISO(props.company.startDate), 'MMM yyyy') : ''
  const end = props.company.endDate ? format(parseISO(props.company.endDate), 'MMM yyyy') : 'Present'

  if (!start) return ''

  return `${start} - ${end}`
})

const duration = computed((): string => {
  if (!props.company.startDate || !props.company.endDate) return ''

  const interval = intervalToDuration({
    start: parseISO(props.company.startDate),
    end: parseISO(props.company.endDate),
  })

  return formatDuration(interval, { format: ['years', 'months'] })
})
</script>
