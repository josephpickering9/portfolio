import { defineNuxtRouteMiddleware } from '#imports'

// The real guard is registered in plugins/auth0.client.ts; this server-side no-op keeps the named middleware resolvable during SSR.
export default defineNuxtRouteMiddleware(() => {})
