<template>
  <div class="flex w-full flex-col items-center">
    <TagList />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import useMeta from '../../composables/useMeta'
import TagList from '../../components/lists/TagList.vue'

export default defineComponent({
  // eslint-disable-next-line vue/match-component-file-name
  name: 'Tags',
  components: { TagList },
  setup() {
    const { updateMeta } = useMeta()

    updateMeta({ title: 'Tags', description: 'View all of the tags associated with projects' })
  },
})
</script>
