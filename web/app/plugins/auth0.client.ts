import { createAuth0 } from '@auth0/auth0-vue'
import { addRouteMiddleware, defineNuxtPlugin, useRuntimeConfig } from '#imports'
import { client } from '@api/client.gen'

export default defineNuxtPlugin((nuxtApp) => {
  const config = useRuntimeConfig()
  const auth0 = createAuth0({
    domain: config.public.auth0Domain,
    clientId: config.public.auth0ClientId,
    authorizationParams: {
      redirect_uri: window.location.origin,
      audience: config.public.apiBase,
    },
    useRefreshTokens: true,
    cacheLocation: 'localstorage',
  })

  nuxtApp.vueApp.use(auth0)

  addRouteMiddleware('auth', async (to) => {
    const redirectToLogin = () => auth0.loginWithRedirect({ appState: { target: to.path } })

    auth0.checkSession()
    if (!auth0.isAuthenticated.value) {
      redirectToLogin()
      return
    }

    try {
      const token = await auth0.getAccessTokenSilently()
      if (token) client.setConfig({ headers: { Authorization: `Bearer ${token}` } })
    } catch {
      redirectToLogin()
    }
  })
})
