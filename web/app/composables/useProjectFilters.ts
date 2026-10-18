import { computed, ref } from 'vue'
import type { Project, TagType } from '@api'
import { tagTypes } from '~/data/TagTypes'
import { useQuerySync } from '~/composables/useQuerySync'
import { filterProjects, getTagTypesInUse } from '~/utils/project-helper'

function toTagType(value: string | undefined): TagType | undefined {
  return tagTypes.find((type) => type.value === value)?.value
}

export function useProjectFilters(projects: () => Project[]) {
  const search = ref('')
  const type = ref<TagType | undefined>()

  useQuerySync(search, { key: 'q', initialValue: '' })
  useQuerySync(type, { key: 'type', initialValue: undefined, transform: toTagType, serialize: (value) => value })

  const filteredProjects = computed(() => filterProjects(projects(), search.value, type.value))
  const availableTypes = computed(() => getTagTypesInUse(projects()))

  function clearFilters() {
    search.value = ''
    type.value = undefined
  }

  return { search, type, filteredProjects, availableTypes, clearFilters }
}
