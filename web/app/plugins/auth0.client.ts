import { createAuth0, useAuth0 } from '@auth0/auth0-vue'
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

  if (import.meta.client) nuxtApp.vueApp.use(auth0)

  addRouteMiddleware('auth', async () => {
    if (import.meta.client) {
      auth0.checkSession()
      if (!auth0.isAuthenticated.value) {
        auth0.loginWithRedirect({
          appState: {
            target: useRoute().path,
          },
        })
      } else {
        try {
          const { getAccessTokenSilently } = useAuth0()

          const token = await getAccessTokenSilently()
          if (token) {
            client.setConfig({
              headers: {
                Authorization: `Bearer ${token}`,
              },
            })
          }
        } catch (e) {
          // eslint-disable-next-line no-console
          console.log('token error', e)

          auth0.loginWithRedirect({
            appState: {
              target: useRoute().path,
            },
          })
        }
      }
    }
  })
})
