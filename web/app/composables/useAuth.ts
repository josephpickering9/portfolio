import { computed } from 'vue'
import { useAuth0 } from '@auth0/auth0-vue'

export default function useAuth() {
  if (process.server) return {}
  if (!useAuth0()) return {}

  const { isAuthenticated, loginWithRedirect, logout } = useAuth0()

  return {
    isAuthenticated: computed(() => isAuthenticated.value),
    login() {
      loginWithRedirect()
    },
    logout() {
      logout({ logoutParams: { returnTo: window.location.origin } })
    },
  }
}
