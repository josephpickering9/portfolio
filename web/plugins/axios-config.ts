import { OpenAPI } from '../api'

export default defineNuxtPlugin((nuxtApp) => {
    const config = useRuntimeConfig()
    OpenAPI.BASE = config.public.apiBase
})
