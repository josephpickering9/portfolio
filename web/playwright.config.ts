import { defineConfig, devices } from '@playwright/test'

const PORT = process.env['E2E_PORT'] ?? '3100'
const BASE_URL = process.env['E2E_BASE_URL'] ?? `http://localhost:${PORT}`

export default defineConfig({
  testDir: './e2e',
  fullyParallel: true,
  retries: process.env['CI'] ? 2 : 0,
  reporter: process.env['CI'] ? [['list'], ['html', { open: 'never' }]] : 'list',
  use: {
    baseURL: BASE_URL,
    trace: 'on-first-retry',
  },
  projects: [{ name: 'chromium', use: { ...devices['Desktop Chrome'] } }],
  webServer: process.env['E2E_BASE_URL']
    ? undefined
    : {
        command: `yarn build && PORT=${PORT} node .output/server/index.mjs`,
        url: BASE_URL,
        reuseExistingServer: !process.env['CI'],
        timeout: 300_000,
      },
})
