import { describe, expect, it } from 'vitest'
import { ImageType, TagType } from '@api'
import type { Project } from '@api'
import {
  countProjectsByTag,
  countProjectsUsingTag,
  filterProjects,
  filterTagUsages,
  getImagesByType,
  getNextProject,
  getProjectEyebrow,
  getProjectsForCompany,
  getTagSummary,
  pickBySlug,
  toProjectSummary,
} from '~/utils/project-helper'

function project(overrides: Partial<Project> = {}): Project {
  return {
    id: '1',
    title: 'Project',
    shortDescription: '',
    description: '',
    startDate: '2024-01-01',
    showMockup: false,
    slug: 'project',
    images: [],
    repositories: [],
    tags: [],
    stats: [],
    ...overrides,
  }
}

function tag(title: string, type: TagType = TagType.FRONTEND) {
  return { id: title, title, type, slug: title.toLowerCase() }
}

describe('toProjectSummary', () => {
  it('keeps only the fields list views use', () => {
    const tags = [tag('Vue')]
    const detail = project({ id: '7', title: 'Pulse', slug: 'pulse', companyId: 'c1', tags, cardImageUrl: 'card.png' })

    expect(toProjectSummary(detail)).toEqual({
      id: '7',
      title: 'Pulse',
      slug: 'pulse',
      shortDescription: '',
      companyId: 'c1',
      startDate: '2024-01-01',
      endDate: undefined,
      tags,
      cardImageUrl: 'card.png',
    })
  })
})

describe('getTagSummary', () => {
  it('joins the first four tag titles', () => {
    const tags = ['Vue', 'Nuxt', '.NET', 'C#', 'Docker'].map((title) => tag(title))
    expect(getTagSummary(project({ tags }))).toBe('Vue, Nuxt, .NET, C#')
  })
})

describe('pickBySlug', () => {
  it('returns projects in slug order and skips missing slugs', () => {
    const projects = [project({ slug: 'a' }), project({ slug: 'b' })]
    expect(pickBySlug(projects, ['b', 'missing', 'a']).map((p) => p.slug)).toEqual(['b', 'a'])
  })
})

describe('filterProjects', () => {
  const projects = [
    project({ slug: 'a', title: 'Pulse', shortDescription: 'Event discovery', tags: [tag('Nuxt')] }),
    project({ slug: 'b', title: 'Data Extractor', shortDescription: 'Documents', tags: [tag('Kotlin', TagType.BACKEND)] }),
  ]

  it('returns everything with no filters', () => {
    expect(filterProjects(projects, '')).toHaveLength(2)
  })

  it('matches title, short description and tag titles case-insensitively', () => {
    expect(filterProjects(projects, 'pulse').map((p) => p.slug)).toEqual(['a'])
    expect(filterProjects(projects, 'DOCUMENTS').map((p) => p.slug)).toEqual(['b'])
    expect(filterProjects(projects, 'kotlin').map((p) => p.slug)).toEqual(['b'])
  })

  it('filters by tag type', () => {
    expect(filterProjects(projects, '', TagType.BACKEND).map((p) => p.slug)).toEqual(['b'])
  })
})

describe('getNextProject', () => {
  const projects = [project({ slug: 'a' }), project({ slug: 'b' })]

  it('returns the following project and wraps around', () => {
    expect(getNextProject(projects, 'a')?.slug).toBe('b')
    expect(getNextProject(projects, 'b')?.slug).toBe('a')
  })

  it('returns undefined for an unknown slug', () => {
    expect(getNextProject(projects, 'missing')).toBeUndefined()
  })
})

describe('countProjectsByTag', () => {
  it('counts projects per tag, most used first', () => {
    const projects = [project({ tags: [tag('Vue'), tag('Nuxt')] }), project({ tags: [tag('Vue')] })]
    expect(countProjectsByTag(projects).map((u) => [u.tag.title, u.count])).toEqual([
      ['Vue', 2],
      ['Nuxt', 1],
    ])
  })
})

describe('getImagesByType', () => {
  it('returns images of one type in order', () => {
    const images = [
      { id: '2', image: 'two.png', type: ImageType.DESKTOP, order: 2 },
      { id: 'm', image: 'mobile.png', type: ImageType.MOBILE, order: 0 },
      { id: '1', image: 'one.png', type: ImageType.DESKTOP, order: 1 },
    ]
    expect(getImagesByType(project({ images }), ImageType.DESKTOP).map((i) => i.image)).toEqual(['one.png', 'two.png'])
  })
})

describe('filterTagUsages', () => {
  const usages = [
    { tag: tag('TypeScript'), count: 9 },
    { tag: tag('JavaScript'), count: 11 },
    { tag: tag('Kotlin', TagType.BACKEND), count: 5 },
  ]

  it('returns every usage with no filters', () => {
    expect(filterTagUsages(usages, '')).toHaveLength(3)
  })

  it('matches tag titles case-insensitively, ignoring surrounding spaces', () => {
    expect(filterTagUsages(usages, '  SCRIPT ').map((u) => u.tag.title)).toEqual(['TypeScript', 'JavaScript'])
  })

  it('combines the search with the tag type', () => {
    expect(filterTagUsages(usages, 't', TagType.BACKEND).map((u) => u.tag.title)).toEqual(['Kotlin'])
  })
})

describe('countProjectsUsingTag', () => {
  it('counts the projects that include the tag', () => {
    const vue = tag('Vue')
    const projects = [project({ tags: [vue] }), project({ tags: [tag('React')] }), project({ tags: [vue] })]
    expect(countProjectsUsingTag(projects, vue.id)).toBe(2)
  })

  it('returns zero without a tag id', () => {
    expect(countProjectsUsingTag([project({ tags: [tag('Vue')] })], undefined)).toBe(0)
  })
})

describe('getProjectsForCompany', () => {
  it('returns only the projects for that company', () => {
    const projects = [project({ id: 'a', companyId: 'acme' }), project({ id: 'b', companyId: 'other' })]
    expect(getProjectsForCompany(projects, 'acme').map((p) => p.id)).toEqual(['a'])
  })
})

describe('getProjectEyebrow', () => {
  const companies = [
    { id: 'acme', name: 'Acme', slug: 'acme', description: '' },
    { id: 'me', name: 'Personal', slug: 'personal', description: '' },
  ]

  it('names the company and the date range', () => {
    const eyebrow = getProjectEyebrow(project({ companyId: 'acme', startDate: '2024-01-01', endDate: '2024-03-01' }), companies, false)
    expect(eyebrow).toBe('Acme / Jan 2024 — Mar 2024 (2 months)')
  })

  it('labels personal and company-less work as a personal project', () => {
    expect(getProjectEyebrow(project({ companyId: 'me', startDate: undefined }), companies, false)).toBe('Personal project')
    expect(getProjectEyebrow(project({ companyId: undefined, startDate: undefined }), companies, false)).toBe('Personal project')
  })

  it('uses the case-study label for this site', () => {
    expect(getProjectEyebrow(project({ companyId: 'acme' }), companies, true)).toBe('Case study / Acme')
  })
})
