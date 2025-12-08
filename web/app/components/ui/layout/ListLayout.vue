<template>
  <div class="w-full animate-fade-in-up" :class="maxWidth">
    <div class="mb-12 flex gap-6 flex-row items-end justify-between">
      <div class="flex flex-col gap-2">
        <h1>{{ title }}</h1>
        <p v-if="description" class="text-lg text-base-content/60">
          {{ description }}
        </p>
      </div>

      <div class="flex flex-col gap-3 sm:flex-row sm:items-center">
        <slot name="actions" />
      </div>
    </div>

    <div v-if="loading" class="mt-12 grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
      <slot name="loading">
        <Skeleton v-for="n in 6" :key="n" class="h-32 w-full rounded-xl" />
      </slot>
    </div>

    <div v-else-if="isEmpty" class="flex flex-col items-center justify-center py-20 text-center">
      <slot name="empty">
        <div class="mb-4 rounded-full bg-base-200 p-4">
          <Icon name="heroicons:inbox" class="h-12 w-12 text-base-content/40" />
        </div>
        <h2 class="text-xl font-bold text-base-content">{{ emptyTitle }}</h2>
        <p class="mt-2 text-base-content/60">{{ emptyDescription }}</p>
      </slot>
    </div>

    <div v-else class="space-y-12">
      <slot />
    </div>
  </div>
</template>

<script setup lang="ts">
import Skeleton from '~/components/feedback/loading/Skeleton.vue'

interface Props {
  title: string
  description?: string
  loading?: boolean
  isEmpty?: boolean
  emptyTitle?: string
  emptyDescription?: string
  maxWidth?: string
}

withDefaults(defineProps<Props>(), {
  description: undefined,
  loading: false,
  isEmpty: false,
  emptyTitle: 'No items found',
  emptyDescription: 'Try adjusting your filters or search terms.',
  maxWidth: 'max-w-7xl',
})
</script>
