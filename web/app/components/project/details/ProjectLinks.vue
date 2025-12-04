<template>
  <div class="project-links flex flex-wrap items-center gap-x-4 gap-y-2">
    <IconLink
      v-if="project.website"
      :to="project.website"
      label="Website"
      icon="material-symbols:globe"
      icon-size="1.4em"
    />
    <div class="flex items-center gap-2">
      <div v-for="(repository, index) in project.repositories" :key="index" class="flex items-center gap-2">
        <IconLink
          v-if="index === 0"
          :to="repository.url"
          :label="repository.title"
          icon="mdi:github"
          icon-size="1.4em"
        />
        <IconLink v-else :to="repository.url" :label="repository.title" />
        <span v-if="index !== project.repositories.length - 1" class="text-gray-400">/</span>
      </div>
    </div>
    <CompanyItem v-if="company" :company="company" :show-link="true" />
  </div>
</template>

<script setup lang="ts">
import type { Project, Company } from '@api'
import IconLink from '~/components/layout/navigation/IconLink.vue'
import CompanyItem from '~/components/company/form/CompanyItem.vue'

defineProps<{
  project: Project
  company?: Company
}>()
</script>
