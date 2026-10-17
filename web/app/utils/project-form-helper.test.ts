import { describe, expect, it } from 'vitest'
import { ImageType, TagType } from '@api'
import type { Project } from '@api'
import { projectToForm, projectToImageSlots } from '~/utils/project-form-helper'

const project: Project = {
  id: 'p1',
  title: 'Pulse',
  shortDescription: 'Gigs',
  description: '<p>Hi</p>',
  startDate: '2026-03-01T00:00:00Z',
  endDate: null,
  website: null,
  showMockup: true,
  slug: 'pulse',
  images: [
    { id: 'd2', image: 'd2.png', type: ImageType.DESKTOP, order: 2 },
    { id: 'd1', image: 'd1.png', type: ImageType.DESKTOP, order: 1 },
  ],
  banner: { id: 'b', image: 'banner.png', type: ImageType.BANNER },
  repositories: [{ id: 'r', title: 'API', url: 'https://github.com/a/b', projectId: 'p1' }],
  tags: [{ id: 't', title: 'Nuxt', type: TagType.FRONTEND, slug: 'nuxt' }],
  stats: [{ id: 's', label: 'gigs', value: '240+', projectId: 'p1' }],
}

describe('projectToForm', () => {
  it('maps a project to the editable form shape', () => {
    expect(projectToForm(project)).toMatchObject({
      title: 'Pulse',
      startDate: '2026-03-01',
      endDate: undefined,
      website: '',
      showMockup: true,
      tags: ['Nuxt'],
      repositories: [{ id: 'r', title: 'API', url: 'https://github.com/a/b' }],
      stats: [{ id: 's', label: 'gigs', value: '240+' }],
    })
  })
})

describe('projectToImageSlots', () => {
  it('builds ordered image slots with preview URLs', () => {
    const slots = projectToImageSlots(project, (image) => `/media/${image}`)

    expect(slots.banner).toEqual({ key: 'b', id: 'b', previewUrl: '/media/banner.png' })
    expect(slots.card).toBeNull()
    expect(slots.desktop.map((item) => item.id)).toEqual(['d1', 'd2'])
  })
})
