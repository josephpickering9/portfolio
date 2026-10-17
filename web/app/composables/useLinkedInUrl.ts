import { computed } from 'vue'
import { DEFAULT_LINKED_IN_URL } from '~/data/NavItems'

export default function useLinkedInUrl() {
  const runtimeConfig = useRuntimeConfig()
  return computed((): string => runtimeConfig.public.linkedInUrl || DEFAULT_LINKED_IN_URL)
}
