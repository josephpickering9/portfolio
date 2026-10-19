import { ref } from 'vue'
import type { Ref } from 'vue'
import type { AsyncForm } from '~/types/AsyncForm'
import { extractError, extractErrorCode, isCancelledError } from '~/utils/error-helper'

export class ApiRequestError extends Error {
  readonly body: unknown
  readonly status?: number

  constructor(body: unknown, status?: number) {
    super('API request failed')
    this.name = 'ApiRequestError'
    this.body = body
    this.status = status
  }
}

export interface ApiResult<T> {
  data?: T
  error?: unknown
  name?: string
  status?: number
  response?: { status?: number }
}

export function unwrap<T>(result: ApiResult<T>): T {
  if (result.error === undefined) return result.data as T
  if (isCancelledError(result)) throw result
  throw new ApiRequestError(result.error, result.response?.status ?? result.status)
}

export async function tryCatchFinally<T>(form: Ref<AsyncForm<T>>, fn: () => Promise<T>): Promise<T | undefined> {
  form.value.error = undefined
  form.value.errorCode = undefined
  form.value.loading = true

  try {
    const result = await fn()
    form.value.data = result
    return result
  } catch (error: unknown) {
    if (isCancelledError(error)) return undefined
    form.value.error = extractError(error)
    form.value.errorCode = extractErrorCode(error)
    return undefined
  } finally {
    form.value.loading = false
  }
}

export async function tryCatchFinallyIfLatest<T>(
  form: Ref<AsyncForm<T>>,
  isLatest: () => boolean,
  fn: () => Promise<T>,
): Promise<T | undefined> {
  form.value.loading = true
  const request = ref<AsyncForm<T>>({ ...asyncForm<T>(), data: form.value.data }) as Ref<AsyncForm<T>>
  const result = await tryCatchFinally(request, fn)
  if (!isLatest()) return undefined

  form.value = request.value
  return result
}

export function asyncForm<T>(): AsyncForm<T> {
  return {
    data: undefined,
    error: undefined,
    errorCode: undefined,
    loading: false,
  }
}

export function isAwaitingData<T>(form: AsyncForm<T>): boolean {
  return Boolean(form.loading) || (form.data === undefined && !form.error)
}
