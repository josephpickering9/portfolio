import { defineSitemapEventHandler } from '#imports'

export default defineSitemapEventHandler(async (event) => {
  try {
    const config = useRuntimeConfig(event)
    const apiBase = process.env.NUXT_PUBLIC_API_BASE ?? config.public.apiBase ?? ''
    return []

    // return projects.map((p) => ({
    //   loc: `/projects/${p.slug}`,
    // }))
  } catch (e) {
    // eslint-disable-next-line no-console
    console.error('sitemap error', e)
    return []
  }
})
