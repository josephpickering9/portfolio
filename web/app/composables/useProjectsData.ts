import type { NuxtApp } from '#app'
import type { Pinia } from 'pinia'
import { useAsyncData } from '#imports'
import { useProjectStore } from '~/store/ProjectStore'

const PROJECTS_KEY = 'projects'

async function loadProjects(nuxtApp: NuxtApp): Promise<boolean> {
  await useProjectStore(nuxtApp.$pinia as Pinia).ensureProjects()
  return true
}

export default function useProjectsData() {
  return useAsyncData(PROJECTS_KEY, loadProjects, { lazy: true })
}
