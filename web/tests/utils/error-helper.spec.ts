import { describe, expect, it } from 'vitest'
import { ApiRequestError } from '~/utils/async-helper'
import {
  extractError,
  extractErrorCode,
  isCancelledError,
  isNetworkError,
  isNotFoundCode,
  messageForCode,
} from '~/utils/error-helper'
import { ERROR_MESSAGES, ErrorCodes, GENERIC_ERROR_MESSAGE, NETWORK_ERROR_MESSAGE } from '~/types/errors'

const axiosError = (status: number | undefined, data: unknown) => ({
  isAxiosError: true,
  name: 'AxiosError',
  response: status === undefined ? undefined : { status, data },
})

describe('extractErrorCode', () => {
  it('reads the code from an unwrapped API failure', () => {
    expect(extractErrorCode(new ApiRequestError({ error: ErrorCodes.ProjectNotFound }, 404))).toBe(ErrorCodes.ProjectNotFound)
  })

  it('reads the code from an axios response body', () => {
    expect(extractErrorCode(axiosError(409, { error: ErrorCodes.Conflict }))).toBe(ErrorCodes.Conflict)
  })

  it('reads the code from a raw error body', () => {
    expect(extractErrorCode({ error: ErrorCodes.Forbidden })).toBe(ErrorCodes.Forbidden)
  })

  it('returns undefined when there is no code', () => {
    expect(extractErrorCode(axiosError(500, 'Server exploded'))).toBeUndefined()
    expect(extractErrorCode('plain')).toBeUndefined()
    expect(extractErrorCode(null)).toBeUndefined()
  })
})

describe('extractError', () => {
  it('maps a known code to its user-facing message', () => {
    expect(extractError(new ApiRequestError({ error: ErrorCodes.UpstreamUnavailable }, 503))).toBe(
      ERROR_MESSAGES[ErrorCodes.UpstreamUnavailable],
    )
  })

  it('maps a validation 400 with a code through the same map', () => {
    expect(extractError(axiosError(400, { error: ErrorCodes.BadRequest }))).toBe(ERROR_MESSAGES[ErrorCodes.BadRequest])
  })

  it('falls back to the generic message for an unknown code', () => {
    expect(extractError(new ApiRequestError({ error: 'some_new_code' }, 400))).toBe(GENERIC_ERROR_MESSAGE)
  })

  it('does not show raw server text', () => {
    expect(extractError(axiosError(500, 'NullReferenceException at line 12'))).toBe(GENERIC_ERROR_MESSAGE)
  })

  it('reports a network failure when no response came back', () => {
    expect(extractError(axiosError(undefined, undefined))).toBe(NETWORK_ERROR_MESSAGE)
    expect(extractError(new ApiRequestError({}))).toBe(NETWORK_ERROR_MESSAGE)
  })

  it('returns a thrown string as-is', () => {
    expect(extractError('Pick a file first')).toBe('Pick a file first')
  })

  it('falls back to the generic message for anything else', () => {
    expect(extractError(new Error('boom'))).toBe(GENERIC_ERROR_MESSAGE)
    expect(extractError(undefined)).toBe(GENERIC_ERROR_MESSAGE)
    expect(extractError('   ')).toBe(GENERIC_ERROR_MESSAGE)
  })
})

describe('isNetworkError', () => {
  it('is false when the server responded', () => {
    expect(isNetworkError(axiosError(500, {}))).toBe(false)
    expect(isNetworkError(new ApiRequestError({}, 500))).toBe(false)
  })

  it('is false for non-request errors', () => {
    expect(isNetworkError(new Error('boom'))).toBe(false)
  })
})

describe('isCancelledError', () => {
  it('recognises a cancelled request', () => {
    expect(isCancelledError({ name: 'CanceledError' })).toBe(true)
    expect(isCancelledError(new Error('boom'))).toBe(false)
  })
})

describe('messageForCode', () => {
  it('returns the mapped message or the generic fallback', () => {
    expect(messageForCode(ErrorCodes.Unauthorized)).toBe(ERROR_MESSAGES[ErrorCodes.Unauthorized])
    expect(messageForCode('unmapped')).toBe(GENERIC_ERROR_MESSAGE)
  })
})

describe('isNotFoundCode', () => {
  it('recognises every not-found code', () => {
    expect(isNotFoundCode(ErrorCodes.NotFound)).toBe(true)
    expect(isNotFoundCode(ErrorCodes.ProjectNotFound)).toBe(true)
    expect(isNotFoundCode(ErrorCodes.CompanyNotFound)).toBe(true)
    expect(isNotFoundCode(ErrorCodes.TagNotFound)).toBe(true)
  })

  it('rejects other codes and missing codes', () => {
    expect(isNotFoundCode(ErrorCodes.Conflict)).toBe(false)
    expect(isNotFoundCode(undefined)).toBe(false)
  })
})
