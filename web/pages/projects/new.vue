<template>
  <div class="flex w-full flex-col items-center space-y-4">
    <ProjectForm />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import ProjectForm from '../../components/forms/global/ProjectForm.vue'

export default defineComponent({
  // eslint-disable-next-line vue/match-component-file-name
  name: 'NewProject',
  components: {
    ProjectForm,
  },
})
</script>
