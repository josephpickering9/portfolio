<template>
  <div class="flex w-full flex-col items-center">
    <ProjectList :show-header="true" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import ProjectList from '../../components/lists/ProjectList.vue'

export default defineComponent({
  // eslint-disable-next-line vue/match-component-file-name
  name: 'Projects',
  components: { ProjectList },
})
</script>
