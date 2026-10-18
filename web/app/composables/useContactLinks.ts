import { computed } from 'vue'
import type { ComputedRef } from 'vue'
import useLinkedInUrl from '~/composables/useLinkedInUrl'
import { EMAIL, GITHUB_URL } from '~/data/NavItems'

export interface ContactLink {
  label: string
  href: string
  icon: string
  external: boolean
}

export function useContactLinks(): ComputedRef<ContactLink[]> {
  const linkedInUrl = useLinkedInUrl()

  return computed(() => [
    { label: 'LinkedIn', href: linkedInUrl.value, icon: 'mdi:linkedin', external: true },
    { label: 'GitHub', href: GITHUB_URL, icon: 'mdi:github', external: true },
    { label: 'Email me', href: `mailto:${EMAIL}`, icon: 'heroicons:envelope', external: false },
  ])
}
