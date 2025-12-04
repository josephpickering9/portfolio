<template>
  <div class="flex w-full flex-col items-center px-4">
    <CompanyList />
  </div>
</template>

<script setup lang="ts">
import useMeta from '~/composables/useMeta'
import CompanyList from '~/components/company/list/CompanyList.vue'

const { updateMeta } = useMeta()

updateMeta({ title: 'Companies', description: "View all of the companies that I've done work for" })
</script>
