<template>
  <TagList />
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import useProjectsData from '~/composables/useProjectsData'
import TagList from '~/components/tag/list/TagList.vue'

const { updateMeta } = useMeta()
const projectsData = useProjectsData()

if (import.meta.server) await projectsData

updateMeta({ title: 'Technologies', description: 'The languages, frameworks and services behind my projects.' })
</script>
