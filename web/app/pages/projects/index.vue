<template>
  <ProjectList />
</template>

<script setup lang="ts">
import ProjectList from '~/components/project/list/ProjectList.vue'
import useMeta from '~/composables/useMeta'

const { updateMeta } = useMeta()

updateMeta({ title: 'Projects', description: "All of the projects that I've been involved in" })
</script>
