<template>
  <div class="flex w-full flex-col items-center px-4">
    <ProjectList :show-header="true" />
  </div>
</template>

<script setup lang="ts">
import ProjectList from '~/app/components/lists/project/ProjectList.vue'
import useMeta from '~/app/composables/useMeta'

const { updateMeta } = useMeta()

updateMeta({ title: 'Projects', description: "All of the projects that I've been involved in" })
</script>
