import { computed, toValue } from 'vue'
import type { ComputedRef, MaybeRefOrGetter } from 'vue'
import { useHead } from '#imports'
import useLinkedInUrl from '~/composables/useLinkedInUrl'
import { EMAIL, GITHUB_URL } from '~/data/NavItems'
import { JOB_TITLE, SITE_URL } from '~/data/Site'
import { SITE_NAME } from '~/utils/meta-helper'
import { serializeJsonLd } from '~/utils/structured-data-helper'
import type { PersonDetails } from '~/utils/structured-data-helper'

export function usePersonDetails(): ComputedRef<PersonDetails> {
  const linkedInUrl = useLinkedInUrl()

  return computed(() => ({
    name: SITE_NAME,
    url: SITE_URL,
    jobTitle: JOB_TITLE,
    image: `${SITE_URL}/joe.png`,
    email: EMAIL,
    sameAs: [GITHUB_URL, linkedInUrl.value],
  }))
}

export function useStructuredData(data: MaybeRefOrGetter<object | undefined>) {
  useHead({
    script: computed(() => {
      const value = toValue(data)
      return value ? [{ key: 'structured-data', type: 'application/ld+json', innerHTML: serializeJsonLd(value) }] : []
    }),
  })
}
