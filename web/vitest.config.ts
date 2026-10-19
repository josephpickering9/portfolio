import { fileURLToPath } from 'node:url'
import { defineConfig } from 'vitest/config'

const nuxtImportsShim = fileURLToPath(new URL('./tests/shims/nuxt-imports.ts', import.meta.url))

export default defineConfig({
  resolve: {
    alias: {
      '~': fileURLToPath(new URL('./app', import.meta.url)),
      '@': fileURLToPath(new URL('./app', import.meta.url)),
      '@api': fileURLToPath(new URL('./api', import.meta.url)),
      '#imports': nuxtImportsShim,
      '#app': nuxtImportsShim,
    },
  },
  define: {
    'import.meta.client': 'true',
    'import.meta.server': 'false',
  },
  test: {
    environment: 'node',
    include: ['tests/**/*.spec.ts'],
    env: {
      TZ: 'UTC',
    },
    coverage: {
      provider: 'v8',
      include: ['app/utils/**', 'app/store/**', 'app/composables/**', 'app/data/**', 'app/types/**'],
      reporter: ['text-summary', 'json-summary'],
    },
  },
})
