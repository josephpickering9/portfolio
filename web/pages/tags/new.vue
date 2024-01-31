<template>
  <div class="flex w-full flex-col items-center space-y-4">
    <TagForm />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import useMeta from '../../composables/useMeta'
import TagForm from '../../components/forms/global/TagForm.vue'

export default defineComponent({
  // eslint-disable-next-line vue/match-component-file-name
  name: 'NewTag',
  components: { TagForm },
  setup() {
    const { updateMeta } = useMeta()

    updateMeta({ title: 'New Tag', description: 'Create a new tag' })
  },
})
</script>
