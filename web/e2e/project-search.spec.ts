import { expect, type Page, test } from '@playwright/test'
import { gotoHydrated } from './support/navigation'

function projectLinks(page: Page) {
  return page.locator('[data-testid="project-grid"] a, [data-testid="project-list"] a')
}

test.describe('Project search & filter', () => {
  test('lists projects and lets a visitor open one', async ({ page }) => {
    await gotoHydrated(page, '/projects')

    await expect(page.getByRole('heading', { name: 'All projects' })).toBeVisible()

    const firstProject = projectLinks(page).first()
    await expect(firstProject).toBeVisible()

    await firstProject.click()
    await expect(page).toHaveURL(/\/projects\/[^/]+$/)
  })

  test('filters the project list by search term and clears back to the full list', async ({ page }) => {
    await gotoHydrated(page, '/projects')

    await expect(projectLinks(page).first()).toBeVisible()
    const initialCount = await projectLinks(page).count()

    const search = page.getByPlaceholder('Search projects')
    await search.fill('zzz-no-project-should-match-zzz')

    await expect(page.getByText('No projects match.')).toBeVisible()

    await page.getByRole('button', { name: 'Clear filters' }).click()

    await expect(projectLinks(page)).toHaveCount(initialCount)
  })

  test('reflects the search term in the URL query string', async ({ page }) => {
    await gotoHydrated(page, '/projects')
    await expect(projectLinks(page).first()).toBeVisible()

    await page.getByPlaceholder('Search projects').fill('portfolio')

    await expect(page).toHaveURL(/[?&]q=portfolio/)
  })

  test('applies a search term from the URL', async ({ page }) => {
    await gotoHydrated(page, '/projects?q=this-will-not-match-anything-12345')

    await expect(page.getByPlaceholder('Search projects')).toHaveValue('this-will-not-match-anything-12345')
    await expect(page.getByText('No projects match.')).toBeVisible()
  })

  test('clears the search and type filters from the URL together', async ({ page }) => {
    await gotoHydrated(page, '/projects?q=this-will-not-match-anything-12345&type=Mobile')

    await expect(page.getByText('No projects match.')).toBeVisible()
    await page.getByRole('button', { name: 'Clear filters' }).click()

    await expect(page).toHaveURL(/\/projects$/)
    await expect(projectLinks(page).first()).toBeVisible()
  })
})
