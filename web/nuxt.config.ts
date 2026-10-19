import { fileURLToPath, URL } from 'node:url'
import { defineNuxtConfig } from 'nuxt/config'
import { LEGACY_THIS_SITE_SLUG, SITE_URL, THIS_SITE_SLUG } from './app/data/Site'

const UMAMI_HOST = 'https://cloud.umami.is'
const ONE_YEAR_SECONDS = 31_536_000
// The onerror marker @nuxt/image renders on SSR images, allowed by exact hash rather than 'unsafe-inline'
const NUXT_IMAGE_ONERROR_HASH = "'sha256-bwK6T5wZVTANitXbrTsel7kl/PyCjCd/Dq5Qoz3imjM='"

function toOrigin(value: string | undefined): URL | undefined {
  if (!value) return undefined
  try {
    return new URL(/^https?:\/\//.test(value) ? value : `https://${value}`)
  } catch {
    return undefined
  }
}

const apiUrl = toOrigin(process.env['NUXT_PUBLIC_API_BASE'])
const apiOrigins = apiUrl ? [apiUrl.origin] : []
const auth0Origins = [toOrigin(process.env['NUXT_AUTH0_DOMAIN'])?.origin].filter((origin) => origin !== undefined)

export default defineNuxtConfig({
  compatibilityDate: '2025-12-03',

  devtools: { enabled: true },

  future: {
    compatibilityVersion: 4,
  },

  ssr: true,

  imports: {
    autoImport: false,
  },

  typescript: {
    tsConfig: {
      include: ['../tests/**/*'],
    },
  },

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
        { rel: 'preload', href: '/fonts/InstrumentSerif-Regular.woff2', as: 'font', type: 'font/woff2', crossorigin: '' },
        { rel: 'preload', href: '/fonts/Geist-Variable.woff2', as: 'font', type: 'font/woff2', crossorigin: '' },
        { rel: 'preload', href: '/fonts/GeistMono-Variable.woff2', as: 'font', type: 'font/woff2', crossorigin: '' },
      ],
      meta: [{ name: 'theme-color', content: '#1D232A' }],
    },
    pageTransition: { name: 'page', mode: 'out-in' },
  },

  css: ['~/assets/css/main.css'],

  routeRules: {
    [`/projects/${LEGACY_THIS_SITE_SLUG}`]: { redirect: { to: `/projects/${THIS_SITE_SLUG}`, statusCode: 301 } },
    '/_nuxt/**': { headers: { 'cache-control': `public, max-age=${ONE_YEAR_SECONDS}, immutable` } },
  },

  nitro: {
    compressPublicAssets: { gzip: true, brotli: true },
  },

  security: {
    nonce: true,
    sri: true,
    rateLimiter: false,
    removeLoggers: false,
    corsHandler: false,
    xssValidator: false,
    headers: {
      crossOriginEmbedderPolicy: false,
      contentSecurityPolicy: {
        'default-src': ["'self'"],
        'base-uri': ["'none'"],
        'object-src': ["'none'"],
        'frame-ancestors': ["'none'"],
        'form-action': ["'self'"],
        'script-src': ["'self'", "'strict-dynamic'", "'nonce-{{nonce}}'"],
        'script-src-attr': ["'unsafe-hashes'", NUXT_IMAGE_ONERROR_HASH],
        'style-src': ["'self'", "'unsafe-inline'"],
        'img-src': ["'self'", 'data:', 'blob:', ...apiOrigins],
        'font-src': ["'self'"],
        'connect-src': ["'self'", ...apiOrigins, ...auth0Origins, UMAMI_HOST],
        'frame-src': auth0Origins,
        'worker-src': ["'self'"],
        'manifest-src': ["'self'"],
        'upgrade-insecure-requests': apiUrl?.protocol === 'https:',
      },
      strictTransportSecurity: { maxAge: ONE_YEAR_SECONDS, includeSubdomains: false },
      referrerPolicy: 'strict-origin-when-cross-origin',
      xFrameOptions: 'DENY',
      permissionsPolicy: {
        camera: [],
        microphone: [],
        geolocation: [],
        payment: [],
        usb: [],
      },
    },
  },

  runtimeConfig: {
    public: {
      apiBase: process.env['NUXT_PUBLIC_API_BASE'] ?? '',
      auth0Domain: process.env['NUXT_AUTH0_DOMAIN'] ?? '',
      auth0ClientId: process.env['NUXT_AUTH0_CLIENT_ID'],
      linkedInUrl: process.env['NUXT_PUBLIC_LINKED_IN_URL'] ?? '',
      commitSha: process.env['NUXT_PUBLIC_COMMIT_SHA'] ?? process.env['GITHUB_SHA'] ?? '',
    },
  },

  modules: [
    '@pinia/nuxt',
    '@nuxt/icon',
    'pinia-plugin-persistedstate/nuxt',
    '@nuxtjs/sitemap',
    '@vite-pwa/nuxt',
    '@nuxt/eslint',
    '@nuxt/image',
    'nuxt-security',
    ['nuxt-umami', {
      id: process.env['NUXT_PUBLIC_UMAMI_ID'] ?? '',
      host: UMAMI_HOST,
      autoTrack: true,
      ignoreLocalhost: true,
    }],
  ],

  vite: {
    plugins: [
      (await import('@tailwindcss/vite')).default(),
    ],
    optimizeDeps: {
      include: [
        '@vue/devtools-core',
        '@vue/devtools-kit',
        '@auth0/auth0-spa-js',
        '@tiptap/extensions',
        '@tiptap/starter-kit',
        '@tiptap/vue-3',
        '@vuelidate/core',
        '@vuelidate/validators',
        'date-fns',
        'axios',
        'lodash-es',
        'vuedraggable',
      ],
    },
  },

  alias: {
    cookie: 'cookie',
    '@api': fileURLToPath(new URL('./api', import.meta.url)),
  },

  site: {
    url: SITE_URL,
  },

  sitemap: {
    sources: ['/api/sitemap'],
    exclude: ['/auth/*', '*/new'],
  },

  image: {
    dir: 'assets/img',
    quality: 90,
    format: ['webp'],
    domains: apiUrl ? [apiUrl.host] : [],
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
      name: 'Joseph Pickering — Portfolio',
      short_name: 'Portfolio',
      description: 'Joseph Pickering, full stack developer. Selected work, case studies and an AI search over my projects.',
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
