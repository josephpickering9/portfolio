<template>
  <div class="flex w-full flex-col items-center px-4">
    <ProjectForm :slug="$route.params.slug" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import useMeta from '../../../composables/useMeta'
import ProjectForm from '../../../components/forms/project/ProjectForm.vue'

export default defineComponent({
  // eslint-disable-next-line vue/match-component-file-name
  name: 'UpdateProject',
  components: {
    ProjectForm,
  },
  setup() {
    const { updateMeta } = useMeta()

    updateMeta({ title: 'Update Project', description: 'Update an existing project' })

    // eslint-disable-next-line no-undef
    definePageMeta({ middleware: 'auth' })
  },
})
</script>
