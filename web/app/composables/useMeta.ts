import { shallowRef, toValue, watch } from 'vue'
import type { MaybeRefOrGetter } from 'vue'
import { useRoute } from 'vue-router'
import { useHead, useSeoMeta } from '#imports'
import { SITE_URL } from '~/data/Site'
import { formatPageTitle, SITE_NAME } from '~/utils/meta-helper'

const DEFAULT_META_IMAGE = `${SITE_URL}/meta.png`

interface Meta {
  title?: string
  description?: string
  image?: string
}

export default function useMeta() {
  const route = useRoute()

  function updateMeta(meta: MaybeRefOrGetter<Meta>) {
    const resolved = shallowRef(toValue(meta))
    watch(
      () => toValue(meta),
      (value) => (resolved.value = value),
    )

    const fullTitle = () => formatPageTitle(resolved.value.title)
    const description = () => resolved.value.description
    const image = () => resolved.value.image || DEFAULT_META_IMAGE
    const canonical = () => `${SITE_URL}${route.path}`

    useHead({ title: fullTitle, link: [{ rel: 'canonical', href: canonical }] })

    useSeoMeta({
      title: fullTitle,
      ogTitle: fullTitle,
      description,
      ogDescription: description,
      ogUrl: canonical,
      ogType: 'website',
      ogSiteName: SITE_NAME,
      ogImage: image,
      twitterImage: image,
      twitterCard: 'summary_large_image',
    })
  }

  return { updateMeta }
}
