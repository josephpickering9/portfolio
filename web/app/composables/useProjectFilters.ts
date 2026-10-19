import { computed } from 'vue'
import type { Project } from '@api'
import { useSearchFilters } from '~/composables/useSearchFilters'
import { filterProjects, getTagTypesInUse } from '~/utils/project-helper'

export function useProjectFilters(projects: () => Project[]) {
  const { search, type, clearFilters } = useSearchFilters()

  const filteredProjects = computed(() => filterProjects(projects(), search.value, type.value))
  const availableTypes = computed(() => getTagTypesInUse(projects()))

  return { search, type, filteredProjects, availableTypes, clearFilters }
}
