import { describe, expect, it } from 'vitest'
import { TagType } from '@api'
import { tagTypeDefinitions } from '~/data/TagTypes'
import { getTagIcon, getTagTypeLabel, parseTagType, tagToForm } from '~/utils/tag-helper'

describe('parseTagType', () => {
  it('accepts a generated enum value', () => {
    expect(parseTagType('DevOps')).toBe(TagType.DEV_OPS)
  })

  it('rejects anything else', () => {
    expect(parseTagType('devops')).toBeUndefined()
    expect(parseTagType(undefined)).toBeUndefined()
    expect(parseTagType('')).toBeUndefined()
  })
})

describe('getTagIcon', () => {
  it("prefers the tag's own icon", () => {
    expect(getTagIcon({ icon: 'simple-icons:vuedotjs', type: TagType.FRONTEND })).toBe('simple-icons:vuedotjs')
  })

  it('falls back to the type icon, then the default icon', () => {
    expect(getTagIcon({ icon: '', type: TagType.MOBILE })).toBe(tagTypeDefinitions[TagType.MOBILE].icon)
    expect(getTagIcon({ icon: null })).toBe(tagTypeDefinitions[TagType.DEFAULT].icon)
  })
})

describe('getTagTypeLabel', () => {
  it('returns the display label', () => {
    expect(getTagTypeLabel(TagType.DEV_OPS)).toBe('DevOps')
  })
})

describe('tagToForm', () => {
  it('copies the editable fields and blanks a missing icon', () => {
    const tag = { id: '1', slug: 'vue', title: 'Vue', type: TagType.FRONTEND, icon: null, customColour: '#fff' }
    expect(tagToForm(tag)).toEqual({ title: 'Vue', type: TagType.FRONTEND, icon: '', customColour: '#fff' })
  })
})
