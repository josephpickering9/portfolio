<template>
  <div v-if="tag" class="input input-bordered flex w-full items-center justify-between p-4">
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

<script lang="ts">
import { defineComponent, type PropType } from 'vue'
import type { Tag as TagModel } from '../../../api/models/Tag'
import Tag from '../../tags/Tag.vue'
import FormButton from '../../forms/elements/FormButton.vue'
import useAuth from '../../../composables/useAuth'

export default defineComponent({
  name: 'TagListItem',
  components: { Tag, FormButton },
  props: {
    tag: {
      type: Object as PropType<TagModel>,
      required: true,
    },
  },
  setup() {
    const { isAuthenticated } = useAuth()

    return {
      isAuthenticated,
    }
  },
})
</script>
../../../api/models/Tag../../../composables/useAuth
