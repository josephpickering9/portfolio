import { fileURLToPath, URL } from 'node:url'

export default defineNuxtConfig({
  compatibilityDate: '2025-12-03',

  devtools: { enabled: true },

  future: {
    compatibilityVersion: 4,
  },

  ssr: true,

  app: {
    head: {
      charset: 'utf-8',
      viewport: 'width=device-width, initial-scale=1',
      htmlAttrs: {
        lang: 'en',
      },
      link: [
        { rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' },
        { rel: 'apple-touch-icon', href: '/apple-touch-icon.png' },
        { rel: 'mask-icon', href: '/mask-icon.svg', color: '#1D232A' },
      ],
      meta: [{ name: 'theme-color', content: '#1D232A' }],
    },
    pageTransition: { name: 'page', mode: 'out-in' },
  },

  css: ['~/assets/css/main.css'],

  runtimeConfig: {
    public: {
      apiBase: process.env['NUXT_PUBLIC_API_BASE'] ?? '',
      base: process.env['NUXT_PUBLIC_BASE'] ?? '',
      auth0Domain: process.env['NUXT_AUTH0_DOMAIN'] ?? '',
      auth0ClientId: process.env['NUXT_AUTH0_CLIENT_ID'],
      linkedInUrl: process.env['NUXT_PUBLIC_LINKED_IN_URL'] ?? '',
    },
  },

  modules: [
    '@pinia/nuxt',
    '@nuxt/icon',
    '@pinia-plugin-persistedstate/nuxt',
    '@nuxtjs/sitemap',
    '@vite-pwa/nuxt',
    '@nuxt/eslint',
    '@nuxt/image',
  ],

  vite: {
    plugins: [
      (await import('@tailwindcss/vite')).default(),
    ],
  },

  alias: {
    cookie: 'cookie',
    '@api': fileURLToPath(new URL('./api', import.meta.url)),
    '@types': fileURLToPath(new URL('./types', import.meta.url)),
    '@mocks': fileURLToPath(new URL('./mocks', import.meta.url)),
  },

  sitemap: {
    sources: ['/api/sitemap'],
    exclude: ['/auth/*', '*/new'],
  },

  image: {
    dir: "assets/img",
  },

  pwa: {
    devOptions: {
      enabled: false,
    },
    strategies: 'generateSW',
    registerType: 'autoUpdate',
    includeAssets: ['favicon.ico', 'apple-touch-icon.png', 'mask-icon.svg'],
    workbox: {
      globPatterns: ['**/*.{js,css,html,ico,png,svg,json,vue}'],
      navigateFallback: null,
      runtimeCaching: [],
    },
    manifest: {
      name: 'Work Experience',
      short_name: 'Work Experience',
      description: "Joseph Pickering | All of the projects that I've been involved in",
      theme_color: '#1D232A',
      icons: [
        {
          src: 'android-chrome-192x192.png',
          sizes: '192x192',
          type: 'image/png',
        },
        {
          src: 'android-chrome-512x512.png',
          sizes: '512x512',
          type: 'image/png',
        },
        {
          src: 'android-chrome-512x512.png',
          sizes: '512x512',
          type: 'image/png',
          purpose: 'any maskable',
        },
      ],
    },
  },
})
