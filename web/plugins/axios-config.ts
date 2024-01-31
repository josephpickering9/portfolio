import { OpenAPI } from '../api'
import { defineNuxtPlugin, useRuntimeConfig } from '#app'

export default defineNuxtPlugin((nuxtApp) => {
  const config = useRuntimeConfig()
  OpenAPI.BASE = config.public.apiBase
})
