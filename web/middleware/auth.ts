import { useAuth0 } from '@auth0/auth0-vue'

export default defineNuxtRouteMiddleware((to, from) => {
  if (process.server) return

  const { isAuthenticated } = useAuth0()

  if (!isAuthenticated.value && to.path !== '/') return navigateTo('/')
})
