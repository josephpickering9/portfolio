<template>
  <div
    v-if="tag"
    class="badge flex items-center gap-1 bg-base-200"
    :class="{ 'border-primary/50 ring-1 ring-primary/20': outline }"
  >
    <div 
      class="flex items-center justify-center rounded-full bg-base-100 shadow-sm"
      :style="{ color: tag.customColour || undefined }"
    >
       <Icon v-if="tag.icon" :name="tag.icon" size="14" />
       <Icon v-else name="heroicons:tag" size="14" />
    </div>

    <span class="truncate">{{ tag.title }}</span>

    <button
      v-if="clearable"
      type="button"
      class="ml-1 flex h-4 w-4 items-center justify-center rounded-full text-base-content/40 hover:bg-error/10 hover:text-error"
      @click.stop="$emit('remove', tag)"
    >
      <Icon name="heroicons:x-mark" size="12" />
    </button>
  </div>
</template>

<script setup lang="ts">
import type { Tag } from '@api'

interface Props {
  tag: Tag
  outline?: boolean
  clearable?: boolean
}

withDefaults(defineProps<Props>(), {
  outline: false,
  clearable: false,
})

defineEmits<{
  remove: [tag: Tag]
}>()
</script>
