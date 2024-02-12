<template>
  <div class="flex w-full flex-col items-center px-4">
    <ProjectList :show-header="true" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import ProjectList from '../../components/lists/ProjectList.vue'
import useMeta from '../../composables/useMeta'

export default defineComponent({
  // eslint-disable-next-line vue/match-component-file-name
  name: 'Projects',
  components: { ProjectList },
  setup() {
    const { updateMeta } = useMeta()

    updateMeta({ title: 'Projects', description: "All of the projects that I've been involved in" })
  },
})
</script>
