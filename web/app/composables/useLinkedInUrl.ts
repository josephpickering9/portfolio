import { computed } from 'vue'
import type { ComputedRef } from 'vue'
import { useRuntimeConfig } from '#imports'
import { DEFAULT_LINKED_IN_URL } from '~/data/NavItems'

export default function useLinkedInUrl(): ComputedRef<string> {
  const runtimeConfig = useRuntimeConfig()
  return computed(() => runtimeConfig.public.linkedInUrl || DEFAULT_LINKED_IN_URL)
}
