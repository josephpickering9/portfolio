import { OpenAPI } from '../api'
import { defineNuxtPlugin, useRuntimeConfig } from '#app'

export default defineNuxtPlugin(() => {
  const config = useRuntimeConfig()
  // eslint-disable-next-line no-console
  console.log('config.public.apiBase', config.public.apiBase)
  OpenAPI.BASE = config.public.apiBase
})
