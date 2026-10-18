import { computed } from 'vue'
import type { ComputedRef } from 'vue'
import { useAuth0 } from '@auth0/auth0-vue'

interface Auth {
  isAuthenticated: ComputedRef<boolean>
  login: () => void
  logout: () => void
}

const signedOut: Auth = {
  isAuthenticated: computed(() => false),
  login: () => undefined,
  logout: () => undefined,
}

export default function useAuth(): Auth {
  if (import.meta.server) return signedOut

  const auth0 = useAuth0()
  if (!auth0) return signedOut

  return {
    isAuthenticated: computed(() => auth0.isAuthenticated.value),
    login: () => auth0.loginWithRedirect(),
    logout: () => auth0.logout({ logoutParams: { returnTo: window.location.origin } }),
  }
}
