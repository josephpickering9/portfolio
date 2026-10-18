import { ImageType } from '@api'
import type { Project, ProjectImage, Tag, TagType } from '@api'

const TAG_SUMMARY_COUNT = 4

export function getCardImage(project: Project): string {
  const firstDesktop = getImagesByType(project, ImageType.DESKTOP)[0]
  return project.cardUrl || project.bannerUrl || firstDesktop?.image || ''
}

export function getHeroImage(project: Project): string {
  const firstDesktop = getImagesByType(project, ImageType.DESKTOP)[0]
  return firstDesktop?.image || project.bannerUrl || project.cardUrl || ''
}

export function getTagSummary(project: Project): string {
  return project.tags
    .slice(0, TAG_SUMMARY_COUNT)
    .map((tag) => tag.title)
    .join(', ')
}

export function padNumber(value: number): string {
  return String(value).padStart(2, '0')
}

export function pickBySlug(projects: Project[], slugs: string[]): Project[] {
  return slugs.map((slug) => projects.find((project) => project.slug === slug)).filter((project) => project !== undefined)
}

export function filterProjects(projects: Project[], search: string, type?: TagType): Project[] {
  const query = search.trim().toLowerCase()

  return projects.filter((project) => {
    if (type && !project.tags.some((tag) => tag.type === type)) return false
    if (!query) return true

    const haystack = [project.title, project.shortDescription, ...project.tags.map((tag) => tag.title)].join(' ').toLowerCase()
    return haystack.includes(query)
  })
}

export function getTagTypesInUse(projects: Project[]): Set<TagType> {
  return new Set(projects.flatMap((project) => project.tags.map((tag) => tag.type)))
}

export function getNextProject(projects: Project[], slug: string): Project | undefined {
  const index = projects.findIndex((project) => project.slug === slug)
  if (index === -1 || projects.length < 2) return undefined
  return projects[(index + 1) % projects.length]
}

export interface TagUsage {
  tag: Tag
  count: number
}

export function countProjectsByTag(projects: Project[]): TagUsage[] {
  const usage = new Map<string, TagUsage>()
  projects.forEach((project) =>
    project.tags.forEach((tag) => {
      const entry = usage.get(tag.id) ?? { tag, count: 0 }
      entry.count++
      usage.set(tag.id, entry)
    }),
  )
  return [...usage.values()].sort((a, b) => b.count - a.count || a.tag.title.localeCompare(b.tag.title))
}

export function filterTagUsages(usages: TagUsage[], search: string, type?: TagType): TagUsage[] {
  const query = search.trim().toLowerCase()
  return usages.filter((usage) => (!type || usage.tag.type === type) && usage.tag.title.toLowerCase().includes(query))
}

export function getImagesByType(project: Project, type: ImageType): ProjectImage[] {
  return project.images.filter((image) => image.type === type).sort((a, b) => (a.order ?? 0) - (b.order ?? 0))
}

export function getHost(url?: string | null): string {
  return (url ?? '').replace(/^https?:\/\//, '').replace(/\/$/, '')
}

export default {
  filterProjects,
  getTagTypesInUse,
  getNextProject,
  countProjectsByTag,
  filterTagUsages,
  getImagesByType,
  getHost,
  getCardImage,
  getHeroImage,
  getTagSummary,
  padNumber,
  pickBySlug,
}
