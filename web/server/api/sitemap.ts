import { OpenAPI, ProjectService } from '../../api'

export default defineSitemapEventHandler(async (e) => {
  try {
    OpenAPI.BASE = process.env.NUXT_PUBLIC_API_BASE ?? 'http://localhost:5105'
    const projects = await ProjectService.getProject()

    return projects.map((p) => ({
      loc: `/projects/${p.id}`,
    }))
  } catch (e) {
    // eslint-disable-next-line no-console
    console.error('sitemap error', e)
    return []
  }
})
