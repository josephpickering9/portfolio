import { describe, expect, it } from 'vitest'
import { TagType } from '@api'
import { tagTypes } from '~/data/TagTypes'

describe('tagTypes', () => {
  it('covers every generated TagType exactly once', () => {
    expect(tagTypes.map((type) => type.value).sort()).toEqual(Object.values(TagType).sort())
  })

  it('keeps Default first and Other last', () => {
    expect(tagTypes[0]?.value).toBe(TagType.DEFAULT)
    expect(tagTypes.at(-1)?.value).toBe(TagType.OTHER)
  })
})
