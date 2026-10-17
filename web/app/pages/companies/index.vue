<template>
  <CompanyList />
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import CompanyList from '~/components/company/list/CompanyList.vue'

const { updateMeta } = useMeta()

updateMeta({ title: 'Companies', description: "Companies I've worked with, and the projects I delivered at each." })
</script>
