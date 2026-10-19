const AUTH0_CACHE_PREFIX = '@@auth0spajs@@'

export function hasCachedSession(storageKeys: string[], clientId: string): boolean {
  const prefix = `${AUTH0_CACHE_PREFIX}::${clientId}::`
  return storageKeys.some((key) => key.startsWith(prefix))
}

export function isAuthRedirect(search: string): boolean {
  const params = new URLSearchParams(search)
  return params.has('state') && (params.has('code') || params.has('error'))
}

export const AUTHENTICATED_STATE_KEY = 'auth:authenticated'
