import { describe, expect, it } from 'vitest'
import { ERROR_MESSAGES, ErrorCodes, isErrorCode, NOT_FOUND_ERROR_CODES } from '~/types/errors'

describe('ErrorCodes', () => {
  it('uses snake_case values', () => {
    Object.values(ErrorCodes).forEach((code) => expect(code).toMatch(/^[a-z]+(_[a-z]+)*$/))
  })

  it('has a non-empty message for every code', () => {
    Object.values(ErrorCodes).forEach((code) => expect(ERROR_MESSAGES[code].trim()).not.toBe(''))
  })

  it('has no message for a code that is not defined', () => {
    expect(Object.keys(ERROR_MESSAGES).sort()).toEqual(Object.values(ErrorCodes).sort())
  })

  it('lists only defined codes as not-found codes', () => {
    NOT_FOUND_ERROR_CODES.forEach((code) => expect(isErrorCode(code)).toBe(true))
  })
})

describe('isErrorCode', () => {
  it('accepts known codes and rejects anything else', () => {
    expect(isErrorCode(ErrorCodes.InternalError)).toBe(true)
    expect(isErrorCode('made_up')).toBe(false)
    expect(isErrorCode(42)).toBe(false)
  })
})
