<template>
  <div class="group relative">
    <component
      :is="preview ? 'div' : NuxtLink"
      :to="preview ? undefined : { path: '/projects', query: { q: tag.title } }"
      class="flex h-full items-center gap-3 rounded-xl border bg-surface p-4 text-text transition-[border-color,box-shadow] duration-200"
      :class="[
        highlighted ? 'border-accent shadow-[0_16px_40px_-20px_var(--glow)]' : 'border-line',
        { 'hover:border-accent hover:text-text hover:shadow-[0_16px_40px_-20px_var(--glow)]': !preview },
      ]"
    >
      <Icon :name="icon" size="22" class="shrink-0" />
      <span class="flex min-w-0 flex-1 flex-wrap items-baseline justify-between gap-x-3 gap-y-0.5">
        <span class="font-medium break-words">{{ tag.title || 'Technology' }}</span>
        <span class="ml-auto font-mono text-xs whitespace-nowrap text-muted">{{ count }} {{ count === 1 ? 'project' : 'projects' }}</span>
      </span>
    </component>
    <ClientOnly>
      <NuxtLink
        v-if="isAuthenticated && !preview"
        :to="`/technologies/${tag.id}/update`"
        class="absolute -top-2 -right-2 flex size-7 items-center justify-center rounded-full border border-line bg-surface2 text-muted opacity-0 transition-opacity group-focus-within:opacity-100 group-hover:opacity-100"
        :aria-label="`Edit ${tag.title}`"
      >
        <Icon name="heroicons:pencil" size="14" />
      </NuxtLink>
    </ClientOnly>
  </div>
</template>

<script setup lang="ts">
import { computed, resolveComponent } from 'vue'
import type { Tag } from '@api'
import { tagTypes } from '~/data/TagTypes'
import useAuth from '~/composables/useAuth'

interface Props {
  tag: Pick<Tag, 'id' | 'title' | 'type' | 'icon'>
  count?: number
  preview?: boolean
  highlighted?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  count: 0,
  preview: false,
  highlighted: false,
})

const { isAuthenticated } = useAuth()
const NuxtLink = resolveComponent('NuxtLink')

const icon = computed(() => props.tag.icon || tagTypes.find((type) => type.value === props.tag.type)?.icon || 'heroicons:tag')
</script>
