import { defineEventHandler } from 'h3'
import { useRuntimeConfig } from 'nitropack/runtime'

interface SitemapProject {
  slug: string
}

export default defineEventHandler(async (event) => {
  try {
    const { apiBase } = useRuntimeConfig(event).public
    const projects = await $fetch<SitemapProject[]>(`${apiBase}/project`)
    return projects.map((project) => ({ loc: `/projects/${project.slug}` }))
  } catch (error: unknown) {
    // eslint-disable-next-line no-console
    console.error('sitemap error', error)
    return []
  }
})
