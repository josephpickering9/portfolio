import { useHead, useSeoMeta, useRuntimeConfig } from '#imports'

export default function useMeta() {
  function updateMeta({ title, description, image }: { title?: string; description?: string; image?: string }) {
    const defaultMeta = `${useRuntimeConfig().public.base}/meta.png`
    const fullTitle = title ? `${title} | Joseph Pickering` : 'Joseph Pickering'

    useHead({
      title: fullTitle,
    })

    useSeoMeta({
      title: fullTitle,
      ogTitle: fullTitle,
      description,
      ogDescription: description,
      image: image || defaultMeta,
      ogImage: image || defaultMeta,
      twitterImage: image || defaultMeta,
      twitterCard: 'summary_large_image',
    })
  }

  return { updateMeta }
}
