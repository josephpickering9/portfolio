import { expect, test } from '@playwright/test'

test.describe('Public navigation', () => {
  const publicPages: Array<{ path: string; heading: string }> = [
    { path: '/', heading: 'Home' },
    { path: '/projects', heading: 'Projects' },
    { path: '/companies', heading: 'Companies' },
    { path: '/technologies', heading: 'Technologies' },
    { path: '/search', heading: 'AI Search' },
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

  test('shows the sign-in page', async ({ page }) => {
    await page.goto('/auth/login')

    await expect(page.getByTestId('login-button')).toBeVisible()
    await expect(page).toHaveTitle(/Sign in/)
  })

  test('sends an unauthenticated visitor to the Auth0 login from a protected route', async ({ page, baseURL }) => {
    await page.route('**/authorize?**', (route) => route.fulfill({ contentType: 'text/html', body: '<title>Auth0 login</title>' }))

    await page.goto('/projects/new')

    await page.waitForURL((url) => url.pathname.endsWith('/authorize'), { timeout: 15_000 })
    const authorizeUrl = new URL(page.url())
    expect(authorizeUrl.searchParams.get('redirect_uri')).toBe(new URL(baseURL ?? '').origin)
    expect(authorizeUrl.searchParams.get('response_type')).toBe('code')
  })
})
