<template>
  <div class="projects">
    <div v-if="loading" class="flex flex-col items-center space-y-4">
      <Skeleton />
      <Skeleton />
      <Skeleton />
    </div>
    <div v-else-if="projects.length === 0" class="flex flex-col items-center space-y-4">
      <div class="text-2xl font-bold">No projects found</div>
      <div class="text-lg">Try searching for something else</div>
    </div>
    <div v-else class="flex flex-col items-center space-y-4">
      <ProjectListItem v-for="project in projects" :key="project.id" :project="project" />
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'
import Skeleton from '../loading/Skeleton.vue'
import { useProjectStore } from '../../store/ProjectStore'
import type { Project } from '../../api/models/Project'
import ProjectListItem from './ProjectListItem.vue'

interface Data {
  initialLoad: boolean
}

export default defineComponent({
  name: 'ProjectList',
  components: {
    Skeleton,
    ProjectListItem,
  },
  data(): Data {
    return {
      initialLoad: true,
    }
  },
  computed: {
    projects(): Project[] {
      return useProjectStore().projects
    },
    loading(): boolean {
      return useProjectStore().projectsLoading || this.initialLoad
    },
  },
  watch: {
    projects() {
      this.initialLoad = false
    },
  },
})
</script>
