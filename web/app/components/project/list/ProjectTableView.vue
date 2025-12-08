<template>
  <Table
    :columns="columns"
    :data="projects"
    :sort-column="sortColumn"
    :sort-direction="sortDirection"
    @sort="$emit('sort', $event)"
  >
    <!-- Logo Column -->
    <template #cell-logo="{ row }">
      <div class="flex h-10 w-10 flex-shrink-0 items-center justify-center rounded-lg bg-base-100 p-1 shadow-sm ring-1 ring-base-200">
        <img
          v-if="row.logoUrl"
          :src="getImageUrl(row.logoUrl)"
          :alt="`${row.title} Logo`"
          class="h-full w-full object-contain"
        >
        <Icon v-else name="heroicons:cube" class="h-6 w-6 text-base-content/40" />
      </div>
    </template>

    <!-- Project Title & Company Column -->
    <template #cell-project="{ row }">
      <NuxtLink
        :to="`/projects/${row.slug}`"
        class="flex flex-col gap-1 hover:text-primary transition-colors"
      >
        <div class="font-bold text-base leading-tight">{{ row.title }}</div>
        <div v-if="row.company" class="text-xs font-medium text-primary/80">
          {{ row.company.name }}
        </div>
      </NuxtLink>
    </template>

    <!-- Date Range Column -->
    <template #cell-dateRange="{ row }">
      <div class="flex flex-col text-sm text-base-content/70">
        <span>{{ formatDate(row.startDate) }}</span>
        <span class="text-xs">to {{ row.endDate ? formatDate(row.endDate) : 'Present' }}</span>
      </div>
    </template>

    <!-- Description Column -->
    <template #cell-description="{ row }">
      <p class="line-clamp-2 text-sm text-base-content/70 max-w-md">
        {{ row.shortDescription }}
      </p>
    </template>

    <!-- Tags Column -->
    <template #cell-tags="{ row }">
      <div class="flex flex-wrap gap-1.5">
        <Tag
          v-for="(tag, index) in row.tags.slice(0, 3)"
          :key="index"
          :tag="tag"
          class="bg-base-200/50"
        />
        <span
          v-if="row.tags.length > 3"
          class="flex h-6 items-center px-1.5 text-xs font-medium text-base-content/50"
        >
          +{{ row.tags.length - 3 }}
        </span>
      </div>
    </template>

    <!-- Actions Column -->
    <template #cell-actions="{ row }">
      <NuxtLink
        :to="`/projects/${row.slug}`"
        class="btn btn-ghost btn-sm"
      >
        <Icon name="heroicons:arrow-right" class="h-4 w-4" />
      </NuxtLink>
    </template>
  </Table>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { format } from 'date-fns'
import type { Project } from '@api/models/Project'
import { getImageUrl } from '~/utils/image-helper'
import Table, { type TableColumn } from '~/components/ui/table/Table.vue'
import Tag from '~/components/tag/Tag.vue'

interface Props {
  projects: Project[]
  sortColumn?: string | null
  sortDirection?: 'asc' | 'desc'
}

withDefaults(defineProps<Props>(), {
  sortColumn: null,
  sortDirection: 'asc',
})

defineEmits<{
  sort: [column: string]
}>()

const columns = computed<TableColumn[]>(() => [
  {
    key: 'logo',
    label: '',
    sortable: false,
    thClass: 'w-16',
  },
  {
    key: 'project',
    label: 'Project',
    sortable: true,
  },
  {
    key: 'dateRange',
    label: 'Date Range',
    sortable: true,
    thClass: 'hidden md:table-cell',
    tdClass: 'hidden md:table-cell',
  },
  {
    key: 'description',
    label: 'Description',
    sortable: false,
    thClass: 'hidden lg:table-cell',
    tdClass: 'hidden lg:table-cell',
  },
  {
    key: 'tags',
    label: 'Tags',
    sortable: false,
    thClass: 'hidden xl:table-cell',
    tdClass: 'hidden xl:table-cell',
  },
  {
    key: 'actions',
    label: '',
    sortable: false,
    thClass: 'w-20',
  },
])

function formatDate(dateString: string): string {
  try {
    return format(new Date(dateString), 'MMM yyyy')
  } catch {
    return dateString
  }
}
</script>
