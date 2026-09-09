import { ref } from 'vue'
import { describe, expect, it } from 'vitest'
import { asyncForm, tryCatchFinally } from '~/utils/async-helper'

describe('asyncForm', () => {
  it('returns an empty, non-loading form', () => {
    expect(asyncForm()).toEqual({ data: undefined, error: undefined, loading: false })
  })
})

describe('tryCatchFinally', () => {
  it('stores the resolved value and clears loading on success', async () => {
    const form = ref(asyncForm<string>())

    const result = await tryCatchFinally(form, async () => 'ok')

    expect(result).toBe('ok')
    expect(form.value.data).toBe('ok')
    expect(form.value.error).toBeUndefined()
    expect(form.value.loading).toBe(false)
  })

  it('sets loading while the function is in flight', async () => {
    const form = ref(asyncForm<string>())
    let loadingDuringCall = false

    await tryCatchFinally(form, async () => {
      loadingDuringCall = form.value.loading === true
      return 'ok'
    })

    expect(loadingDuringCall).toBe(true)
  })

  it('extracts and stores an error message on rejection', async () => {
    const form = ref(asyncForm<string>())

    const result = await tryCatchFinally(form, async () => {
      throw 'a plain string error'
    })

    expect(result).toBeUndefined()
    expect(form.value.error).toBe('a plain string error')
    expect(form.value.loading).toBe(false)
  })

  it('falls back to the generic message for an unrecognised error shape', async () => {
    const form = ref(asyncForm<string>())

    await tryCatchFinally(form, async () => {
      throw new Error('boom')
    })

    expect(form.value.error).toBe('There was an issue with your request')
  })

  it('silently ignores a cancelled request without setting an error', async () => {
    const form = ref(asyncForm<string>())

    const result = await tryCatchFinally(form, async () => {
      const cancelled = new Error('cancelled') as Error & { name: string }
      cancelled.name = 'CanceledError'
      throw cancelled
    })

    expect(result).toBeUndefined()
    expect(form.value.error).toBeUndefined()
    expect(form.value.loading).toBe(false)
  })

  it('clears a previous error before retrying', async () => {
    const form = ref(asyncForm<string>())
    form.value.error = 'stale error'

    await tryCatchFinally(form, async () => 'ok')

    expect(form.value.error).toBeUndefined()
  })

  it('always clears loading, even when the function throws', async () => {
    const form = ref(asyncForm<string>())

    await tryCatchFinally(form, async () => {
      throw new Error('boom')
    })

    expect(form.value.loading).toBe(false)
  })
})
