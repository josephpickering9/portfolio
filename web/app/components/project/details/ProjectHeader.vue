<template>
  <div class="header-container header-container-dark" :style="bannerStyle">
    <div class="prose z-10 flex w-full max-w-full items-center justify-between gap-4 overflow-hidden px-8">
      <div class="flex flex-col gap-x-4 gap-y-2 md:flex-row md:items-center">
        <div class="flex items-center justify-start space-x-2 md:hidden">
          <img
            v-if="project.logoUrl"
            :src="getImageUrl(project.logoUrl)"
            :alt="`${project.title} Logo`"
            class="m-0 h-10 w-10 rounded-full"
          >
          <small class="text-white">{{ project.year }}</small>
        </div>
        <img
          v-if="project.logoUrl"
          :src="getImageUrl(project.logoUrl)"
          :alt="`${project.title} Logo`"
          class="m-0 hidden h-10 w-10 rounded-full md:flex"
        >
        <h1 ref="title" class="title text-gray-400">{{ project.title }}</h1>
        <small class="hidden text-white md:flex">{{ project.year }}</small>
      </div>
      <ClientOnly>
        <NuxtLink
          v-if="isAuthenticated"
          :to="`/projects/${project.slug}/update`"
          class="absolute right-8 top-6 text-xs md:relative md:right-auto md:top-auto"
        >
          Edit
        </NuxtLink>
      </ClientOnly>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import type { Project } from '@api'
import { getImageUrl } from '~/utils/image-helper'
import useAuth from '~/composables/useAuth'

const props = defineProps<{
  project: Project
}>()

const { isAuthenticated } = useAuth()

const bannerStyle = computed(() => {
  if (!props.project.bannerUrl) return {}
  return {
    backgroundImage: `url(${getImageUrl(props.project.bannerUrl)})`,
  }
})
</script>

<style scoped>
.header-container {
  position: relative;
  display: flex;
  height: 18rem;
  flex-direction: column;
  justify-content: flex-end;
  background-size: cover;
  background-position: center;
  padding-top: 2rem;
  padding-bottom: 2rem;
}

.header-container::after {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  display: block;
  height: 100%;
  width: 100%;
  background-image: linear-gradient(to bottom, transparent, var(--fallback-b1, oklch(var(--b1))));
}

.header-container-light::after {
  --tw-gradient-to: rgb(156 163 175);
}

.header-container-dark::after {
  --tw-gradient-to: rgb(0 0 0);
}

.title {
  position: relative;
  margin: 0;
  font-size: 2.25rem;
  line-height: 2.5rem;
}
</style>
