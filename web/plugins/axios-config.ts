import { OpenAPI } from '../api'
import { defineNuxtPlugin, useRuntimeConfig } from '#app'

export default defineNuxtPlugin(() => {
  const config = useRuntimeConfig()
  // eslint-disable-next-line no-console
  console.log('config.public', config.public)
  OpenAPI.BASE = config.public.apiBase
})
