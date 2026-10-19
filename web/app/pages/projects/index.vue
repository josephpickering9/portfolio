<template>
  <ProjectList />
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import useProjectsData from '~/composables/useProjectsData'
import ProjectList from '~/components/project/list/ProjectList.vue'

const { updateMeta } = useMeta()
const projectsData = useProjectsData()

if (import.meta.server) await projectsData

updateMeta({ title: 'Projects', description: "All of the projects that I've been involved in" })
</script>
