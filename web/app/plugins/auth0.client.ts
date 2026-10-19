import type { Auth0Client } from '@auth0/auth0-spa-js'
import { addRouteMiddleware, defineNuxtPlugin, useRouter, useRuntimeConfig, useState } from '#imports'
import { client } from '@api/client.gen'
import { AUTHENTICATED_STATE_KEY, hasCachedSession, isAuthRedirect } from '~/utils/auth-helper'

interface AppState {
  target?: string
}

export default defineNuxtPlugin((nuxtApp) => {
  const config = useRuntimeConfig()
  const router = useRouter()
  const isAuthenticated = useState(AUTHENTICATED_STATE_KEY, () => false)
  let auth0: Promise<Auth0Client> | undefined

  // The SDK is only downloaded for the CMS, a returning signed-in visitor, or the login callback.
  function getClient(): Promise<Auth0Client> {
    auth0 ??= import('@auth0/auth0-spa-js').then(({ createAuth0Client }) =>
      createAuth0Client({
        domain: config.public.auth0Domain,
        clientId: config.public.auth0ClientId,
        authorizationParams: {
          redirect_uri: window.location.origin,
          audience: config.public.apiBase,
        },
        useRefreshTokens: true,
        cacheLocation: 'localstorage',
      }),
    )
    return auth0
  }

  async function syncSession(auth: Auth0Client): Promise<boolean> {
    try {
      isAuthenticated.value = await auth.isAuthenticated()
      if (isAuthenticated.value) client.setConfig({ headers: { Authorization: `Bearer ${await auth.getTokenSilently()}` } })
    } catch {
      isAuthenticated.value = false
    }
    return isAuthenticated.value
  }

  async function completeLogin(): Promise<void> {
    const auth = await getClient()
    try {
      const { appState } = await auth.handleRedirectCallback<AppState>()
      await syncSession(auth)
      await router.replace(appState?.target ?? '/')
    } catch {
      await router.replace(window.location.pathname)
    }
  }

  function storageKeys(): string[] {
    try {
      return Object.keys(window.localStorage)
    } catch {
      return []
    }
  }

  nuxtApp.hook('app:mounted', () => {
    if (isAuthRedirect(window.location.search)) void completeLogin()
    else if (hasCachedSession(storageKeys(), config.public.auth0ClientId)) void getClient().then(syncSession)
  })

  addRouteMiddleware('auth', async (to) => {
    const auth = await getClient()
    if (await syncSession(auth)) return
    await auth.loginWithRedirect<AppState>({ appState: { target: to.fullPath } })
  })

  return {
    provide: {
      auth: {
        login: async () => (await getClient()).loginWithRedirect(),
        logout: async () => (await getClient()).logout({ logoutParams: { returnTo: window.location.origin } }),
      },
    },
  }
})
