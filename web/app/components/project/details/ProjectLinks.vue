<template>
  <div class="project-links flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
    <div class="flex flex-wrap gap-3">
      <a
        v-if="project.website"
        :href="project.website"
        target="_blank"
        rel="noreferrer"
        class="btn btn-primary btn-sm gap-2"
      >
        <Icon name="material-symbols:globe" size="1.2em" />
        Website
      </a>
      
      <template v-if="project.repositories.length">
        <a 
          v-for="(repo, index) in project.repositories" 
          :key="index"
          :href="repo.url"
          target="_blank"
          rel="noreferrer"
          class="btn btn-outline btn-sm gap-2"
        >
          <Icon name="mdi:github" size="1.2em" />
          {{ repo.title }}
        </a>
      </template>
    </div>

    <div v-if="company" class="flex items-center">
      <span class="mr-2 text-xs font-bold uppercase tracking-wider text-base-content/50">At</span>
      <CompanyItem :company="company" :show-link="true" size="sm" />
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Project, Company } from '@api'
import CompanyItem from '~/components/company/form/CompanyItem.vue'

defineProps<{
  project: Project
  company?: Company
}>()
</script>
