export default defineNuxtConfig({
  devtools: { enabled: true },

  ssr: true,

  app: {
    head: {
      charset: 'utf-8',
      viewport: 'width=device-width, initial-scale=1',
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
      apiBase: process.env.NUXT_PUBLIC_API_BASE ?? '',
      base: process.env.NUXT_PUBLIC_BASE ?? '',
      auth0Domain: process.env.NUXT_AUTH0_DOMAIN ?? '',
      auth0ClientId: process.env.NUXT_AUTH0_CLIENT_ID,
      linkedInUrl: process.env.NUXT_PUBLIC_LINKEDIN_URL ?? '',
    },
  },

  modules: [
    '@nuxtjs/tailwindcss',
    '@pinia/nuxt',
    'nuxt-icon',
    '@pinia-plugin-persistedstate/nuxt',
    'nuxt-simple-sitemap',
    '@vite-pwa/nuxt',
  ],

  alias: {
    cookie: 'cookie',
  },

  sitemap: {
    sources: ['/api/sitemap'],
    exclude: ['/auth/*', '*/new'],
  },

  pwa: {
    includeAssets: ['favicon.ico', 'apple-touch-icon.png', 'mask-icon.svg'],
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
