<template>
  <div class="flex w-full flex-col items-center space-y-4">
    <ProjectForm />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import ProjectForm from '../../components/forms/global/ProjectForm.vue'
import useMeta from '../../composables/useMeta'

export default defineComponent({
  // eslint-disable-next-line vue/match-component-file-name
  name: 'NewProject',
  components: {
    ProjectForm,
  },
  setup() {
    const { updateMeta } = useMeta()

    updateMeta({ title: 'New Project', description: 'Create a new project' })

    // eslint-disable-next-line no-undef
    definePageMeta({ middleware: 'auth' })
  },
})
</script>
