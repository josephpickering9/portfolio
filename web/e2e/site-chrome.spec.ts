import { expect, test } from '@playwright/test'
import { gotoHydrated } from './support/navigation'

const MOBILE_VIEWPORT = { width: 390, height: 844 }

test.describe('Site chrome', () => {
  test('switches theme and keeps it after a reload', async ({ page }) => {
    await gotoHydrated(page, '/')
    const html = page.locator('html')
    await expect(html).toHaveAttribute('data-theme', 'midnight')

    await page.getByTestId('theme-ember').first().click()
    await expect(html).toHaveAttribute('data-theme', 'ember')
    await expect(page.getByTestId('theme-ember').first()).toHaveAttribute('aria-pressed', 'true')

    await gotoHydrated(page, '/projects')
    await expect(html).toHaveAttribute('data-theme', 'ember')
  })

  test('opens the mobile menu, navigates and closes it', async ({ page }) => {
    await page.setViewportSize(MOBILE_VIEWPORT)
    await gotoHydrated(page, '/')

    await page.getByTestId('mobile-menu-toggle').click()
    const menu = page.getByTestId('mobile-menu')
    await expect(menu).toBeVisible()

    await menu.getByRole('link', { name: 'Companies' }).click()
    await expect(page).toHaveURL(/\/companies$/)
    await expect(menu).toBeHidden()
  })
})
