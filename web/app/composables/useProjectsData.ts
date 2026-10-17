import type { NuxtApp } from '#app'
import type { Pinia } from 'pinia'
import { useLazyAsyncData } from '#imports'
import { useProjectStore } from '~/store/ProjectStore'

const PROJECTS_KEY = 'projects'

async function loadProjects(nuxtApp: NuxtApp): Promise<boolean> {
  const projectStore = useProjectStore(nuxtApp.$pinia as Pinia)
  if (!projectStore.projectsForm.data) await projectStore.getProjects()
  return true
}

export default function useProjectsData() {
  return useLazyAsyncData(PROJECTS_KEY, loadProjects)
}
