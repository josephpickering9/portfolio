<template>
  <div class="flex w-full flex-col items-center px-4">
    <ProjectForm :id="Number($route.params.id)" />
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import useMeta from '../../../composables/useMeta'
import ProjectForm from '../../../components/forms/global/ProjectForm.vue'

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
