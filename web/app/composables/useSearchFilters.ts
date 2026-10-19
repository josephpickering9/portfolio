import { ref } from 'vue'
import type { TagType } from '@api'
import { useQuerySync } from '~/composables/useQuerySync'
import { parseTagType } from '~/utils/tag-helper'

export function useSearchFilters() {
  const search = ref('')
  const type = ref<TagType | undefined>()

  useQuerySync(search, { key: 'q', initialValue: '' })
  useQuerySync(type, { key: 'type', initialValue: undefined, transform: parseTagType, serialize: (value) => value })

  function clearFilters() {
    search.value = ''
    type.value = undefined
  }

  return { search, type, clearFilters }
}
