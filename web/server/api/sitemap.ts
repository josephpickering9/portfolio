import { OpenAPI, ProjectService } from '../../api'

export default defineSitemapEventHandler(async (e) => {
  try {
    OpenAPI.BASE = process.env.NUXT_PUBLIC_API_BASE ?? useRuntimeConfig().public.apiBase ?? ''
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
