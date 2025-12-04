import { OpenAPI, ProjectService } from '@api/index'
import { defineSitemapEventHandler } from '#imports'

export default defineSitemapEventHandler(async (event) => {
  try {
    const config = useRuntimeConfig(event)
    OpenAPI.BASE = process.env.NUXT_PUBLIC_API_BASE ?? config.public.apiBase ?? ''
    const projects = await ProjectService.getProject()

    return projects.map((p) => ({
      loc: `/projects/${p.slug}`,
    }))
  } catch (e) {
    // eslint-disable-next-line no-console
    console.error('sitemap error', e)
    return []
  }
})
