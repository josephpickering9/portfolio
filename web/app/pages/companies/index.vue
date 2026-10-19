<template>
  <CompanyList />
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import useCompaniesData from '~/composables/useCompaniesData'
import useProjectsData from '~/composables/useProjectsData'
import CompanyList from '~/components/company/list/CompanyList.vue'

const { updateMeta } = useMeta()
const companiesData = useCompaniesData()
const projectsData = useProjectsData()

if (import.meta.server) await Promise.all([companiesData, projectsData])

updateMeta({ title: 'Companies', description: "Companies I've worked with, and the projects I delivered at each." })
</script>
