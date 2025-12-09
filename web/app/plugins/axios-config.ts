import { client } from '@api/client.gen'

export default defineNuxtPlugin(() => {
  const config = useRuntimeConfig()
  client.setConfig({ baseURL: config.public.apiBase })
})
