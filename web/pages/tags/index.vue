<template>
  <div class="flex w-full flex-col items-center">
    <TagList />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import TagList from '../../components/lists/TagList.vue'

export default defineComponent({
  // eslint-disable-next-line vue/match-component-file-name
  name: 'Tags',
  components: { TagList },
})
</script>
