import { describe, expect, it } from 'vitest'
import { notEmpty, removeAt, replaceAt, typedEntries } from '~/utils/array-helper'

describe('notEmpty', () => {
  it('filters out null and undefined but keeps falsy values', () => {
    expect([0, null, '', undefined, false].filter(notEmpty)).toEqual([0, '', false])
  })
})

describe('replaceAt', () => {
  it('replaces the item at the index without mutating the input', () => {
    const items = ['a', 'b', 'c']
    expect(replaceAt(items, 1, 'B')).toEqual(['a', 'B', 'c'])
    expect(items).toEqual(['a', 'b', 'c'])
  })
})

describe('removeAt', () => {
  it('removes the item at the index without mutating the input', () => {
    const items = ['a', 'b', 'c']
    expect(removeAt(items, 0)).toEqual(['b', 'c'])
    expect(items).toEqual(['a', 'b', 'c'])
  })
})

describe('typedEntries', () => {
  it('returns entries in insertion order', () => {
    expect(typedEntries({ b: 2, a: 1 })).toEqual([
      ['b', 2],
      ['a', 1],
    ])
  })
})
