import { describe, expect, it } from 'vitest'
import { hasCachedSession, isAuthRedirect } from '~/utils/auth-helper'

describe('hasCachedSession', () => {
  it('finds a cached Auth0 session for this client', () => {
    expect(hasCachedSession(['theme', '@@auth0spajs@@::client-1::https://api::openid'], 'client-1')).toBe(true)
  })

  it('ignores sessions cached for another client', () => {
    expect(hasCachedSession(['@@auth0spajs@@::client-2::https://api::openid'], 'client-1')).toBe(false)
  })

  it('is false when nothing is cached', () => {
    expect(hasCachedSession([], 'client-1')).toBe(false)
  })
})

describe('isAuthRedirect', () => {
  it('recognises the Auth0 callback', () => {
    expect(isAuthRedirect('?code=abc&state=xyz')).toBe(true)
  })

  it('recognises a failed callback', () => {
    expect(isAuthRedirect('?error=access_denied&state=xyz')).toBe(true)
  })

  it('ignores ordinary query strings', () => {
    expect(isAuthRedirect('?q=vue&type=Frontend')).toBe(false)
    expect(isAuthRedirect('?code=only')).toBe(false)
  })
})
