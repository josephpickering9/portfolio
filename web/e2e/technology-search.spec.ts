import { expect, type Page, test } from '@playwright/test'

function tiles(page: Page) {
  return page.locator('main a[href^="/projects?q="]')
}

test.describe('Technology search', () => {
  test('filters technologies by name and keeps the term in the URL', async ({ page }) => {
    await page.goto('/technologies')
    await expect(tiles(page).first()).toBeVisible()
    const initialCount = await tiles(page).count()

    await page.getByPlaceholder('Search technologies').fill('script')

    await expect(page).toHaveURL(/[?&]q=script/)
    const filteredCount = await tiles(page).count()
    expect(filteredCount).toBeLessThan(initialCount)
    for (const name of await tiles(page).allTextContents()) expect(name.toLowerCase()).toContain('script')
  })

  test('shows an empty state and clears every filter, including the URL', async ({ page }) => {
    await page.goto('/technologies?q=this-will-not-match-anything-12345&type=Backend')

    await expect(page.getByText('No technologies match.')).toBeVisible()
    await page.getByRole('button', { name: 'Clear filters' }).click()

    await expect(page).toHaveURL(/\/technologies$/)
    await expect(page.getByPlaceholder('Search technologies')).toHaveValue('')
    await expect(tiles(page).first()).toBeVisible()
  })
})
