import { ERROR_MESSAGES, GENERIC_ERROR_MESSAGE, isErrorCode, NETWORK_ERROR_MESSAGE, NOT_FOUND_ERROR_CODES } from '~/types/errors'

const CANCELLED_ERROR_NAME = 'CanceledError'

function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === 'object' && value !== null
}

function codeFromBody(body: unknown): string | undefined {
  return isRecord(body) && typeof body['error'] === 'string' ? body['error'] : undefined
}

function responseOf(error: Record<string, unknown>): Record<string, unknown> | undefined {
  const response = error['response']
  return isRecord(response) ? response : undefined
}

export function extractErrorCode(error: unknown): string | undefined {
  if (!isRecord(error)) return undefined
  return codeFromBody(error['body']) ?? codeFromBody(responseOf(error)?.['data']) ?? codeFromBody(error)
}

export function isNetworkError(error: unknown): boolean {
  if (!isRecord(error) || extractErrorCode(error)) return false
  const hasStatus = typeof error['status'] === 'number' || responseOf(error) !== undefined
  return (error['isAxiosError'] === true || error['name'] === 'ApiRequestError') && !hasStatus
}

export function isCancelledError(error: unknown): boolean {
  return isRecord(error) && error['name'] === CANCELLED_ERROR_NAME
}

export function messageForCode(code: string): string {
  return isErrorCode(code) ? ERROR_MESSAGES[code] : GENERIC_ERROR_MESSAGE
}

export function isNotFoundCode(code?: string): boolean {
  return isErrorCode(code) && NOT_FOUND_ERROR_CODES.includes(code)
}

export function extractError(error: unknown): string {
  const code = extractErrorCode(error)
  if (code) return messageForCode(code)
  if (typeof error === 'string' && error.trim()) return error
  if (isNetworkError(error)) return NETWORK_ERROR_MESSAGE
  return GENERIC_ERROR_MESSAGE
}
