// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  devtools: { enabled: true },

  app: {
    head: {
      charset: 'utf-8',
      viewport: 'width=device-width, initial-scale=1',
    },
    pageTransition: { name: 'page', mode: 'out-in' },
  },

  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_API_BASE ?? '',
      base: process.env.NUXT_PUBLIC_BASE ?? '',
    },
  },

  modules: ['@nuxtjs/tailwindcss', '@pinia/nuxt', 'nuxt-icon'],
})
