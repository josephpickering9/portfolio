import { OpenAPI } from '../api'
import { defineNuxtPlugin, useRuntimeConfig } from '#app'

export default defineNuxtPlugin(() => {
  const config = useRuntimeConfig()
  // eslint-disable-next-line no-console
  console.log('config.public', config.public, process.env)
  OpenAPI.BASE = process.env.NUXT_PUBLIC_API_BASE ?? ''
})
