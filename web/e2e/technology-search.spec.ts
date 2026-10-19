import { expect, test } from '@playwright/test'
import { gotoHydrated } from './support/navigation'

test.describe('Technology search', () => {
  test('filters technologies by name and keeps the term in the URL', async ({ page }) => {
    await gotoHydrated(page, '/technologies')
    const tiles = page.getByTestId('tag-tile')
    await expect(tiles.first()).toBeVisible()
    const initialCount = await tiles.count()

    await page.getByTestId('search-field').fill('script')

    await expect(page).toHaveURL(/[?&]q=script/)
    await expect.poll(() => tiles.count()).toBeLessThan(initialCount)
    for (const name of await tiles.allTextContents()) expect(name.toLowerCase()).toContain('script')
  })

  test('opens the projects using a technology from its tile', async ({ page }) => {
    await gotoHydrated(page, '/technologies')
    const firstTile = page.getByTestId('tag-tile').first()
    await expect(firstTile).toBeVisible()

    await firstTile.click()

    await expect(page).toHaveURL(/\/projects\?q=/)
  })

  test('shows an empty state and clears every filter, including the URL', async ({ page }) => {
    await gotoHydrated(page, '/technologies?q=this-will-not-match-anything-12345&type=Backend')

    await expect(page.getByTestId('empty-results')).toContainText('No technologies match.')
    await page.getByTestId('clear-filters').click()

    await expect(page).toHaveURL(/\/technologies$/)
    await expect(page.getByTestId('search-field')).toHaveValue('')
    await expect(page.getByTestId('tag-tile').first()).toBeVisible()
  })
})
