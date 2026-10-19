import { ref } from 'vue'
import { describe, expect, it } from 'vitest'
import { ApiRequestError, asyncForm, isAwaitingData, tryCatchFinally, tryCatchFinallyIfLatest, unwrap } from '~/utils/async-helper'
import { ERROR_MESSAGES, ErrorCodes, GENERIC_ERROR_MESSAGE } from '~/types/errors'

describe('asyncForm', () => {
  it('returns an empty, non-loading form', () => {
    expect(asyncForm()).toEqual({ data: undefined, error: undefined, errorCode: undefined, loading: false })
  })
})

describe('unwrap', () => {
  it('returns the data of a successful response', () => {
    expect(unwrap({ data: 'ok', error: undefined })).toBe('ok')
  })

  it('throws an ApiRequestError carrying the body and status of a failed response', () => {
    const failed = { data: undefined, error: { error: ErrorCodes.TagNotFound }, response: { status: 404 } }

    expect(() => unwrap(failed)).toThrow(ApiRequestError)
    try {
      unwrap(failed)
    } catch (error: unknown) {
      expect(error).toMatchObject({ body: { error: ErrorCodes.TagNotFound }, status: 404 })
    }
  })

  it('rethrows a cancelled request unchanged so it can be ignored', () => {
    const cancelled = { name: 'CanceledError', error: {} }

    expect(() => unwrap(cancelled)).toThrow(expect.objectContaining({ name: 'CanceledError' }))
  })
})

describe('tryCatchFinally', () => {
  it('stores and returns the resolved value and clears loading on success', async () => {
    const form = ref(asyncForm<string>())

    const result = await tryCatchFinally(form, async () => 'ok')

    expect(result).toBe('ok')
    expect(form.value).toEqual({ data: 'ok', error: undefined, errorCode: undefined, loading: false })
  })

  it('sets loading while the function is in flight', async () => {
    const form = ref(asyncForm<string>())
    let loadingDuringCall: boolean | undefined

    await tryCatchFinally(form, async () => {
      loadingDuringCall = form.value.loading
      return 'ok'
    })

    expect(loadingDuringCall).toBe(true)
  })

  it('stores the mapped message and code when an unwrapped request fails', async () => {
    const form = ref(asyncForm<string>())

    const result = await tryCatchFinally(form, async () =>
      unwrap<string>({ error: { error: ErrorCodes.Conflict }, response: { status: 409 } }),
    )

    expect(result).toBeUndefined()
    expect(form.value.error).toBe(ERROR_MESSAGES[ErrorCodes.Conflict])
    expect(form.value.errorCode).toBe(ErrorCodes.Conflict)
    expect(form.value.loading).toBe(false)
  })

  it('returns undefined rather than stale data when a retry fails', async () => {
    const form = ref(asyncForm<string>())
    form.value.data = 'previous'

    const result = await tryCatchFinally(form, async () => {
      throw new Error('boom')
    })

    expect(result).toBeUndefined()
    expect(form.value.error).toBe(GENERIC_ERROR_MESSAGE)
  })

  it('ignores a cancelled request without setting an error', async () => {
    const form = ref(asyncForm<string>())

    const result = await tryCatchFinally(form, async () => {
      throw Object.assign(new Error('cancelled'), { name: 'CanceledError' })
    })

    expect(result).toBeUndefined()
    expect(form.value.error).toBeUndefined()
    expect(form.value.loading).toBe(false)
  })

  it('clears a previous error and code before retrying', async () => {
    const form = ref(asyncForm<string>())
    form.value.error = 'stale error'
    form.value.errorCode = ErrorCodes.InternalError

    await tryCatchFinally(form, async () => 'ok')

    expect(form.value.error).toBeUndefined()
    expect(form.value.errorCode).toBeUndefined()
  })
})

describe('tryCatchFinallyIfLatest', () => {
  it('commits the result when the request is still the latest', async () => {
    const form = ref(asyncForm<string>())

    const result = await tryCatchFinallyIfLatest(form, () => true, async () => 'fresh')

    expect(result).toBe('fresh')
    expect(form.value).toMatchObject({ data: 'fresh', loading: false, error: undefined })
  })

  it('keeps the previous data visible while loading', async () => {
    const form = ref({ ...asyncForm<string>(), data: 'previous' })
    let resolve: (value: string) => void = () => {}

    const pending = tryCatchFinallyIfLatest(form, () => true, () => new Promise<string>((r) => (resolve = r)))

    expect(form.value).toMatchObject({ data: 'previous', loading: true })
    resolve('next')
    await pending
    expect(form.value.data).toBe('next')
  })

  it('discards the result and leaves the form untouched when a newer request has started', async () => {
    const form = ref({ ...asyncForm<string>(), data: 'previous' })

    const result = await tryCatchFinallyIfLatest(form, () => false, async () => 'stale')

    expect(result).toBeUndefined()
    expect(form.value).toMatchObject({ data: 'previous', loading: true })
  })

  it('records the error code of a failed latest request', async () => {
    const form = ref(asyncForm<string>())

    await tryCatchFinallyIfLatest(form, () => true, async () => {
      throw new ApiRequestError({ error: ErrorCodes.ProjectNotFound }, 404)
    })

    expect(form.value).toMatchObject({ errorCode: ErrorCodes.ProjectNotFound, loading: false })
  })
})

describe('isAwaitingData', () => {
  it('is true before the first load and while loading', () => {
    expect(isAwaitingData(asyncForm())).toBe(true)
    expect(isAwaitingData({ data: ['a'], loading: true })).toBe(true)
  })

  it('is false once data or an error has arrived', () => {
    expect(isAwaitingData({ data: [], loading: false })).toBe(false)
    expect(isAwaitingData({ error: 'failed', loading: false })).toBe(false)
  })
})
