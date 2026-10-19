import type { Project } from '@api'

const SCHEMA_CONTEXT = 'https://schema.org'

export interface PersonDetails {
  name: string
  url: string
  jobTitle: string
  image: string
  email: string
  sameAs: string[]
}

export interface ProjectPage {
  url: string
  image?: string
  author: PersonDetails
}

export function serializeJsonLd(data: object): string {
  return JSON.stringify(data).replace(/</g, '\\u003c')
}

export function buildPersonJsonLd(person: PersonDetails): object {
  return {
    '@context': SCHEMA_CONTEXT,
    '@type': 'Person',
    name: person.name,
    url: person.url,
    jobTitle: person.jobTitle,
    image: person.image,
    email: `mailto:${person.email}`,
    sameAs: person.sameAs.filter(Boolean),
  }
}

export function buildProjectJsonLd(project: Project, page: ProjectPage): object {
  return {
    '@context': SCHEMA_CONTEXT,
    '@type': 'CreativeWork',
    name: project.title,
    headline: project.title,
    description: project.shortDescription,
    url: page.url,
    ...(page.image && { image: page.image }),
    dateCreated: project.startDate,
    ...(project.endDate && { dateModified: project.endDate }),
    keywords: project.tags.map((tag) => tag.title).join(', '),
    ...(project.website && { sameAs: project.website }),
    author: { '@type': 'Person', name: page.author.name, url: page.author.url },
  }
}
