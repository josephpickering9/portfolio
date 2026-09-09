import { expect, test } from '@playwright/test'

test.describe('Public navigation', () => {
  const publicPages: Array<{ path: string; heading: string }> = [
    { path: '/', heading: 'Home' },
    { path: '/projects', heading: 'Projects' },
    { path: '/companies', heading: 'Companies' },
    { path: '/technologies', heading: 'Technologies' },
  ]

  for (const { path, heading } of publicPages) {
    test(`loads ${path} without a console error`, async ({ page }) => {
      const consoleErrors: string[] = []
      page.on('console', (message) => {
        if (message.type() === 'error') consoleErrors.push(message.text())
      })

      const response = await page.goto(path)

      expect(response?.ok()).toBe(true)
      await expect(page).toHaveTitle(new RegExp(heading, 'i'))
      expect(consoleErrors, `console errors on ${path}: ${consoleErrors.join('; ')}`).toEqual([])
    })
  }

  test('redirects an unauthenticated visitor away from a protected route', async ({ page }) => {
    await page.goto('/projects/new')

    await page.waitForURL((url) => !url.pathname.includes('/projects/new'), { timeout: 15_000 })
    expect(page.url()).not.toContain('/projects/new')
  })
})
