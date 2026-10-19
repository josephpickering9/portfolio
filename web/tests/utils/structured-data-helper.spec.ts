import { describe, expect, it } from 'vitest'
import { TagType } from '@api'
import type { Project } from '@api'
import { buildPersonJsonLd, buildProjectJsonLd, serializeJsonLd } from '~/utils/structured-data-helper'
import type { PersonDetails } from '~/utils/structured-data-helper'

const person: PersonDetails = {
  name: 'Joseph Pickering',
  url: 'https://example.com',
  jobTitle: 'Full stack developer',
  image: 'https://example.com/joe.png',
  email: 'joe@example.com',
  sameAs: ['https://github.com/joe', ''],
}

function project(overrides: Partial<Project> = {}): Project {
  return {
    id: '1',
    title: 'Pulse',
    shortDescription: 'A dashboard',
    description: '',
    startDate: '2024-01-01',
    showMockup: false,
    slug: 'pulse',
    images: [],
    repositories: [],
    tags: [{ id: 't1', title: 'Vue', type: TagType.FRONTEND, slug: 'vue' }],
    stats: [],
    ...overrides,
  }
}

describe('serializeJsonLd', () => {
  it('escapes "<" so content cannot close the script tag', () => {
    expect(serializeJsonLd({ name: '</script><script>alert(1)</script>' })).not.toContain('<')
  })
})

describe('buildPersonJsonLd', () => {
  it('describes the person and drops empty profile links', () => {
    expect(buildPersonJsonLd(person)).toEqual({
      '@context': 'https://schema.org',
      '@type': 'Person',
      name: 'Joseph Pickering',
      url: 'https://example.com',
      jobTitle: 'Full stack developer',
      image: 'https://example.com/joe.png',
      email: 'mailto:joe@example.com',
      sameAs: ['https://github.com/joe'],
    })
  })
})

describe('buildProjectJsonLd', () => {
  it('describes the project as a creative work by the author', () => {
    const jsonLd = buildProjectJsonLd(project({ endDate: '2025-01-01', website: 'https://pulse.example' }), {
      url: 'https://example.com/projects/pulse',
      image: 'https://example.com/card.png',
      author: person,
    })

    expect(jsonLd).toMatchObject({
      '@type': 'CreativeWork',
      name: 'Pulse',
      description: 'A dashboard',
      url: 'https://example.com/projects/pulse',
      image: 'https://example.com/card.png',
      dateCreated: '2024-01-01',
      dateModified: '2025-01-01',
      keywords: 'Vue',
      sameAs: 'https://pulse.example',
      author: { '@type': 'Person', name: 'Joseph Pickering', url: 'https://example.com' },
    })
  })

  it('omits optional fields that are not set', () => {
    const jsonLd = buildProjectJsonLd(project(), { url: 'https://example.com/projects/pulse', author: person })

    expect(jsonLd).not.toHaveProperty('image')
    expect(jsonLd).not.toHaveProperty('dateModified')
    expect(jsonLd).not.toHaveProperty('sameAs')
  })
})
