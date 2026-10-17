import { fileURLToPath } from 'node:url'
import { defineConfig } from 'vitest/config'

export default defineConfig({
  resolve: {
    alias: {
      '~': fileURLToPath(new URL('./app', import.meta.url)),
      '@': fileURLToPath(new URL('./app', import.meta.url)),
      '@api': fileURLToPath(new URL('./api', import.meta.url)),
    },
  },
  test: {
    environment: 'node',
    include: ['app/**/*.test.ts'],
  },
})
