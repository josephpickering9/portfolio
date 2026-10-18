import { shallowRef, toValue, watch, type MaybeRefOrGetter } from 'vue'

interface Meta {
  title?: string
  description?: string
  image?: string
}

export default function useMeta() {
  function updateMeta(meta: MaybeRefOrGetter<Meta>) {
    const defaultMeta = `${useRuntimeConfig().public.base}/meta.png`
    const resolved = shallowRef(toValue(meta))
    watch(() => toValue(meta), (value) => (resolved.value = value))

    const fullTitle = () => (resolved.value.title ? `${resolved.value.title} | Joseph Pickering` : 'Joseph Pickering')
    const description = () => resolved.value.description
    const image = () => resolved.value.image || defaultMeta

    useHead({
      title: fullTitle,
    })

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
