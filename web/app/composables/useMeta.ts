import { shallowRef, toValue, watch } from 'vue'
import type { MaybeRefOrGetter } from 'vue'
import { useHead, useRuntimeConfig, useSeoMeta } from '#imports'
import { formatPageTitle } from '~/utils/meta-helper'

const DEFAULT_META_IMAGE = '/meta.png'

interface Meta {
  title?: string
  description?: string
  image?: string
}

export default function useMeta() {
  const runtimeConfig = useRuntimeConfig()

  function updateMeta(meta: MaybeRefOrGetter<Meta>) {
    const defaultImage = `${runtimeConfig.public.base}${DEFAULT_META_IMAGE}`
    const resolved = shallowRef(toValue(meta))
    watch(
      () => toValue(meta),
      (value) => (resolved.value = value),
    )

    const fullTitle = () => formatPageTitle(resolved.value.title)
    const description = () => resolved.value.description
    const image = () => resolved.value.image || defaultImage

    useHead({ title: fullTitle })

    useSeoMeta({
      title: fullTitle,
      ogTitle: fullTitle,
      description,
      ogDescription: description,
      ogImage: image,
      twitterImage: image,
      twitterCard: 'summary_large_image',
    })
  }

  return { updateMeta }
}
