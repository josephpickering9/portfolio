import { describe, expect, it } from 'vitest'
import { serializeToFormData } from '~/utils/form-data'

describe('serializeToFormData', () => {
  it('flattens nested objects and arrays into dotted and indexed keys', () => {
    const formData = serializeToFormData({
      createProject: { title: 'Pulse', tags: ['Vue', 'Nuxt'], images: [{ type: 'Card', order: 1 }] },
    })

    expect([...formData.entries()]).toEqual([
      ['createProject.title', 'Pulse'],
      ['createProject.tags[0]', 'Vue'],
      ['createProject.tags[1]', 'Nuxt'],
      ['createProject.images[0].type', 'Card'],
      ['createProject.images[0].order', '1'],
    ])
  })

  it('skips null and undefined values and stringifies primitives', () => {
    const formData = serializeToFormData({ a: null, b: undefined, c: false, d: 0 })

    expect([...formData.entries()]).toEqual([
      ['c', 'false'],
      ['d', '0'],
    ])
  })

  it('appends files and dates as-is and as ISO strings', () => {
    const file = new File(['x'], 'logo.png')
    const date = new Date('2024-03-01T00:00:00Z')
    const formData = serializeToFormData({ logo: file, startedAt: date })

    expect(formData.get('logo')).toBeInstanceOf(File)
    expect(formData.get('startedAt')).toBe('2024-03-01T00:00:00.000Z')
  })

  it('returns an empty form for a non-object body', () => {
    expect([...serializeToFormData('nope').entries()]).toEqual([])
    expect([...serializeToFormData(['a']).entries()]).toEqual([])
  })
})
