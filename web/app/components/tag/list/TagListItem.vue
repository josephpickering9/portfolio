<template>
  <component
    :is="preview ? 'div' : NuxtLink"
    v-if="tag"
    :to="preview ? undefined : `/projects?tag=${tag.id}`"
    class="card group relative flex flex-col justify-between overflow-hidden border border-base-200 bg-base-100 p-6 shadow-sm transition-all duration-300 hover:-translate-y-1 hover:border-primary/50 hover:shadow-md"
    :class="{ 'cursor-default hover:translate-y-0 hover:shadow-sm': preview }"
  >
    <div
      class="absolute -right-12 -top-12 h-32 w-32 rounded-full opacity-0 blur-3xl transition-opacity duration-500 group-hover:opacity-10"
      :style="{ backgroundColor: tag.customColour || 'currentColor' }"
      :class="{ 'opacity-5': preview }"
    />

    <div class="z-10 flex items-center justify-between">
      <div class="flex items-center gap-3">
        <div
          class="flex h-10 w-10 items-center justify-center rounded-lg bg-base-200 text-xl shadow-inner"
          :style="{ color: tag.customColour || undefined }"
        >
          <Icon v-if="tag.icon" :name="tag.icon" />
          <Icon v-else name="heroicons:tag" />
        </div>
        <div class="flex flex-col">
          <span class="font-bold text-base-content">{{ tag.title || 'Tag Title' }}</span>
          <span class="text-xs text-base-content/60">{{ tag.type }}</span>
        </div>
      </div>
      <NuxtLink :to="`/tags/${tag.id}/update`">
        <FormButton
          v-if="isAuthenticated && !preview"
          size="xs"
          label="Edit"
          icon="material-symbols:edit"
          class="opacity-0 transition-opacity duration-200 group-hover:opacity-100"
          variant="ghost"
        />
      </NuxtLink>
    </div>
  </component>
</template>

<script setup lang="ts">
import { resolveComponent } from 'vue'
import type { Tag } from '@api'
import useAuth from '~/composables/useAuth'
import FormButton from '~/components/ui/form/FormButton.vue'

interface Props {
  tag?: Tag
  preview?: boolean
}

withDefaults(defineProps<Props>(), {
  tag: undefined,
  preview: false,
})

const { isAuthenticated } = useAuth()
const NuxtLink = resolveComponent('NuxtLink')
</script>
