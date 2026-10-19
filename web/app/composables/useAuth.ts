import { computed } from 'vue'
import type { ComputedRef } from 'vue'
import { useNuxtApp, useState } from '#imports'
import { AUTHENTICATED_STATE_KEY } from '~/utils/auth-helper'

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

  const isAuthenticated = useState(AUTHENTICATED_STATE_KEY, () => false)
  const { $auth } = useNuxtApp()

  return {
    isAuthenticated: computed(() => isAuthenticated.value),
    login: () => void $auth.login(),
    logout: () => void $auth.logout(),
  }
}
