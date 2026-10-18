import { describe, expect, it } from 'vitest'
import { ImageType, TagType } from '@api'
import type { Project } from '@api'
import {
  countProjectsByTag,
  filterProjects,
  filterTagUsages,
  getCardImage,
  getHeroImage,
  getHost,
  getImagesByType,
  getNextProject,
  getTagSummary,
  padNumber,
  pickBySlug,
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

describe('getCardImage', () => {
  it('prefers the card image', () => {
    expect(getCardImage(project({ cardUrl: 'card.png', bannerUrl: 'banner.png' }))).toBe('card.png')
  })

  it('falls back to the banner, then the first desktop image', () => {
    expect(getCardImage(project({ bannerUrl: 'banner.png' }))).toBe('banner.png')

    const images = [
      { id: 'm', image: 'mobile.png', type: ImageType.MOBILE },
      { id: 'd', image: 'desktop.png', type: ImageType.DESKTOP },
    ]
    expect(getCardImage(project({ images }))).toBe('desktop.png')
  })

  it('returns an empty string when there are no images', () => {
    expect(getCardImage(project())).toBe('')
  })
})

describe('getTagSummary', () => {
  it('joins the first four tag titles', () => {
    const tags = ['Vue', 'Nuxt', '.NET', 'C#', 'Docker'].map((title) => tag(title))
    expect(getTagSummary(project({ tags }))).toBe('Vue, Nuxt, .NET, C#')
  })
})

describe('padNumber', () => {
  it('pads single digits to two characters', () => {
    expect(padNumber(1)).toBe('01')
    expect(padNumber(12)).toBe('12')
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

describe('getHost', () => {
  it('strips the protocol and trailing slash', () => {
    expect(getHost('https://experience.josephpickering.co.uk/')).toBe('experience.josephpickering.co.uk')
    expect(getHost(undefined)).toBe('')
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

describe('getHeroImage', () => {
  it('prefers the first desktop screenshot by order', () => {
    const images = [
      { id: '2', image: 'second.png', type: ImageType.DESKTOP, order: 2 },
      { id: '1', image: 'first.png', type: ImageType.DESKTOP, order: 1 },
    ]
    expect(getHeroImage(project({ images, bannerUrl: 'banner.png', cardUrl: 'card.png' }))).toBe('first.png')
  })

  it('falls back to the banner, then the card', () => {
    expect(getHeroImage(project({ bannerUrl: 'banner.png', cardUrl: 'card.png' }))).toBe('banner.png')
    expect(getHeroImage(project({ cardUrl: 'card.png' }))).toBe('card.png')
    expect(getHeroImage(project())).toBe('')
  })
})
