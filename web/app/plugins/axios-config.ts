import { OpenAPI } from '../api'
import { defineNuxtPlugin, useRuntimeConfig } from '#app'

export default defineNuxtPlugin(() => {
  OpenAPI.BASE = useRuntimeConfig().public.apiBase
})
