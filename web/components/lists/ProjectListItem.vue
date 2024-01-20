<template>
  <NuxtLink v-if="project" :to="`/projects/${project.id}`" class="card w-full max-w-3xl bg-base-100 shadow-xl">
    <div class="card-body">
      <div class="flex items-center gap-4">
        <img v-if="image" :src="image" class="h-6 w-6 rounded-full" />
        <h2 class="card-title">{{ project.title }}</h2>
        <small>{{ project.year }}</small>
      </div>
      <p>{{ project.shortDescription }}</p>
      <div class="card-actions justify-end">
        <div v-for="(tag, index) in project.tags" :key="index" class="badge badge-secondary">
          {{ tag.title }}
        </div>
      </div>
    </div>
  </NuxtLink>
</template>

<script lang="ts">
import { defineComponent, type PropType } from 'vue'
import type { Project } from '../../api/models/Project'
import { useRuntimeConfig } from '#imports'

export default defineComponent({
  name: 'ProjectListItem',
  props: {
    project: {
      type: Object as PropType<Project>,
      required: true,
    },
  },
  computed: {
    image(): string {
      if (!this.project?.image) return undefined

      return `${useRuntimeConfig().public.apiBase}/media/uploads/${this.project.image}`
    },
  },
})
</script>
