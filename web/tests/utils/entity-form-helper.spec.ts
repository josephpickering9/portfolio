import { describe, expect, it } from 'vitest'
import { getEntityFormCopy } from '~/utils/entity-form-helper'

describe('getEntityFormCopy', () => {
  it('describes a new entity', () => {
    expect(getEntityFormCopy('company', false, '')).toEqual({
      eyebrow: 'New company',
      title: 'New company',
      saveLabel: 'Create company',
      deleteTitle: 'Delete this company?',
    })
  })

  it('describes an existing entity, using its title when present', () => {
    expect(getEntityFormCopy('technology', true, 'Vue')).toEqual({
      eyebrow: 'Update technology',
      title: 'Vue',
      saveLabel: 'Save changes',
      deleteTitle: 'Delete this technology?',
    })
    expect(getEntityFormCopy('project', true, null).title).toBe('Untitled project')
  })
})
