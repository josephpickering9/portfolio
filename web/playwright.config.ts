import { defineConfig, devices } from '@playwright/test'

export default defineConfig({
  testDir: './e2e',
  // `yarn dev` (Nuxt's dev-mode SSR) serializes and slows down under concurrent
  // requests, causing flaky timeouts with parallel workers. A production build
  // wouldn't have this problem, but running E2E against dev is simpler for now.
  fullyParallel: false,
  workers: 1,
  retries: process.env['CI'] ? 2 : 0,
  reporter: 'list',
  use: {
    baseURL: process.env['E2E_BASE_URL'] ?? 'http://localhost:3000',
    trace: 'on-first-retry',
  },
  projects: [{ name: 'chromium', use: { ...devices['Desktop Chrome'] } }],
  webServer: process.env['E2E_BASE_URL']
    ? undefined
    : {
        command: 'yarn dev',
        url: 'http://localhost:3000',
        reuseExistingServer: !process.env['CI'],
        timeout: 120_000,
      },
})
