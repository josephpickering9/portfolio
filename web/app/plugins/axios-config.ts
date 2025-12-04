import { OpenAPI } from '@api/index'

export default defineNuxtPlugin(() => {
  OpenAPI.BASE = useRuntimeConfig().public.apiBase
})
