import type { Company, ImageType, Project, ProjectImage, ProjectSummary, Tag, TagType } from '@api'
import { formatDateRange } from '~/utils/date-helper'
import { includesSearch } from '~/utils/string-helper'

const TAG_SUMMARY_COUNT = 4

export function toProjectSummary(project: Project): ProjectSummary {
  return {
    id: project.id,
    title: project.title,
    slug: project.slug,
    shortDescription: project.shortDescription,
    companyId: project.companyId,
    startDate: project.startDate,
    endDate: project.endDate,
    tags: project.tags,
    cardImageUrl: project.cardImageUrl,
  }
}

export function getTagSummary(project: ProjectSummary): string {
  return project.tags
    .slice(0, TAG_SUMMARY_COUNT)
    .map((tag) => tag.title)
    .join(', ')
}

export function pickBySlug(projects: ProjectSummary[], slugs: string[]): ProjectSummary[] {
  return slugs.map((slug) => projects.find((project) => project.slug === slug)).filter((project) => project !== undefined)
}

export function filterProjects(projects: ProjectSummary[], search: string, type?: TagType): ProjectSummary[] {
  return projects.filter((project) => {
    if (type && !project.tags.some((tag) => tag.type === type)) return false
    return includesSearch([project.title, project.shortDescription, ...project.tags.map((tag) => tag.title)].join(' '), search)
  })
}

export function getTagTypesInUse(projects: ProjectSummary[]): Set<TagType> {
  return new Set(projects.flatMap((project) => project.tags.map((tag) => tag.type)))
}

export function getNextProject(projects: ProjectSummary[], slug: string): ProjectSummary | undefined {
  const index = projects.findIndex((project) => project.slug === slug)
  if (index === -1 || projects.length < 2) return undefined
  return projects[(index + 1) % projects.length]
}

export interface TagUsage {
  tag: Tag
  count: number
}

export function countProjectsByTag(projects: ProjectSummary[]): TagUsage[] {
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
  return usages.filter((usage) => (!type || usage.tag.type === type) && includesSearch(usage.tag.title, search))
}

export function getImagesByType(project: Project, type: ImageType): ProjectImage[] {
  return project.images.filter((image) => image.type === type).sort((a, b) => (a.order ?? 0) - (b.order ?? 0))
}

export function countProjectsUsingTag(projects: ProjectSummary[], tagId?: string): number {
  return projects.filter((project) => project.tags.some((tag) => tag.id === tagId)).length
}

export function getProjectsForCompany(projects: ProjectSummary[], companyId: string): ProjectSummary[] {
  return projects.filter((project) => project.companyId === companyId)
}

const PERSONAL_COMPANY = 'Personal'
const PERSONAL_PROJECT_LABEL = 'Personal project'

export function getProjectEyebrow(project: ProjectSummary, companies: Company[], isSiteCaseStudy: boolean): string {
  const companyName = companies.find((company) => company.id === project.companyId)?.name
  const owner = !companyName || companyName === PERSONAL_COMPANY ? PERSONAL_PROJECT_LABEL : companyName
  if (isSiteCaseStudy) return `Case study / ${owner}`
  return [owner, formatDateRange(project.startDate, project.endDate)].filter(Boolean).join(' / ')
}
