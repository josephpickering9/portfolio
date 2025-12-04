<template>
  <div v-if="tag" class="input flex w-full items-center justify-between p-4">
    <Tag :tag="tag" />
    <FormButton
      v-if="isAuthenticated"
      size="xs"
      label="Edit"
      icon="material-symbols:edit"
      :href="`/tags/${tag.id}/update`"
    />
  </div>
</template>

<script setup lang="ts">
import type { Tag as TagModel } from '@api/models/Tag'
import useAuth from '~/composables/useAuth'
import Tag from '~/components/tag/Tag.vue'
import FormButton from '~/components/ui/form/FormButton.vue'

interface Props {
  tag: TagModel
}

defineProps<Props>()

const { isAuthenticated } = useAuth()
</script>
../../../api/models/Tag../../../composables/useAuth
