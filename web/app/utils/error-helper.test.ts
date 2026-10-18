import { describe, expect, it } from 'vitest'
import { extractError } from '~/utils/error-helper'

const genericMessage = 'There was an issue with your request'

describe('extractError', () => {
  it('returns the response body for an axios error with a string response', () => {
    const error = { isAxiosError: true, response: { data: 'Company not found' } }

    expect(extractError(error)).toBe('Company not found')
  })

  it('falls back to the generic message for an axios error with no response', () => {
    const error = { isAxiosError: true, response: undefined }

    expect(extractError(error)).toBe(genericMessage)
  })

  it('falls back to the generic message for an axios error with a non-string response body', () => {
    const error = { isAxiosError: true, response: { data: { message: 'nested' } } }

    expect(extractError(error)).toBe(genericMessage)
  })

  it('returns the first validation error for an ApiError with field errors', () => {
    const error = { name: 'ApiError', body: { errors: { name: ['Name is required'] } } }

    expect(extractError(error)).toBe('Name is required')
  })

  it('returns a friendly not-found message for a 404 ApiError', () => {
    const error = { name: 'ApiError', body: { status: 404 } }

    expect(extractError(error)).toBe('Record could not be found')
  })

  it('returns the body message for a non-404 ApiError with no field errors', () => {
    const error = { name: 'ApiError', body: { status: 500, message: 'Something broke' } }

    expect(extractError(error)).toBe('Something broke')
  })

  it('falls back to the generic message for an ApiError with no message', () => {
    const error = { name: 'ApiError', body: { status: 500 } }

    expect(extractError(error)).toBe(genericMessage)
  })

  it('falls back to the generic message for an ApiError with no body', () => {
    const error = { name: 'ApiError', body: undefined }

    expect(extractError(error)).toBe(genericMessage)
  })

  it('returns the error as-is when it is already a string', () => {
    expect(extractError('plain string error')).toBe('plain string error')
  })

  it('falls back to the generic message for an unrecognised error shape', () => {
    expect(extractError(new Error('boom'))).toBe(genericMessage)
  })
})
