import { expect, test } from '@playwright/test'
import { gotoHydrated } from './support/navigation'
import { projectItems } from './support/selectors'

const NO_MATCH = 'this-will-not-match-anything-12345'

test.describe('Project search & filter', () => {
  test('lists projects and lets a visitor open one', async ({ page }) => {
    await gotoHydrated(page, '/projects')

    const firstProject = projectItems(page).first()
    await expect(firstProject).toBeVisible()

    await firstProject.click()
    await expect(page).toHaveURL(/\/projects\/[^/]+$/)
  })

  test('filters the project list by search term and clears back to the full list', async ({ page }) => {
    await gotoHydrated(page, '/projects')

    await expect(projectItems(page).first()).toBeVisible()
    const initialCount = await projectItems(page).count()

    await page.getByTestId('search-field').fill(NO_MATCH)
    await expect(page.getByTestId('empty-results')).toContainText('No projects match.')

    await page.getByTestId('clear-filters').click()
    await expect(projectItems(page)).toHaveCount(initialCount)
  })

  test('reflects the search term in the URL query string', async ({ page }) => {
    await gotoHydrated(page, '/projects')
    await expect(projectItems(page).first()).toBeVisible()

    await page.getByTestId('search-field').fill('portfolio')

    await expect(page).toHaveURL(/[?&]q=portfolio/)
  })

  test('applies a search term from the URL', async ({ page }) => {
    await gotoHydrated(page, `/projects?q=${NO_MATCH}`)

    await expect(page.getByTestId('search-field')).toHaveValue(NO_MATCH)
    await expect(page.getByTestId('empty-results')).toBeVisible()
  })

  test('filters by technology type and keeps the type in the URL', async ({ page }) => {
    await gotoHydrated(page, '/projects')
    await expect(projectItems(page).first()).toBeVisible()
    const initialCount = await projectItems(page).count()

    const typeChip = page.locator('[data-testid^="type-filter-"]:not([data-testid="type-filter-all"])').first()
    const testId = await typeChip.getAttribute('data-testid')
    const type = testId?.replace('type-filter-', '') ?? ''
    await typeChip.click()

    await expect(page).toHaveURL(new RegExp(`[?&]type=${type}`))
    await expect(typeChip).toHaveAttribute('aria-pressed', 'true')
    expect(await projectItems(page).count()).toBeLessThanOrEqual(initialCount)

    await page.getByTestId('type-filter-all').click()
    await expect(page).not.toHaveURL(/[?&]type=/)
  })

  test('clears the search and type filters from the URL together', async ({ page }) => {
    await gotoHydrated(page, `/projects?q=${NO_MATCH}&type=Mobile`)

    await expect(page.getByTestId('empty-results')).toBeVisible()
    await page.getByTestId('clear-filters').click()

    await expect(page).toHaveURL(/\/projects$/)
    await expect(projectItems(page).first()).toBeVisible()
  })

  test('switches to the list view and remembers it after a reload', async ({ page }) => {
    await gotoHydrated(page, '/projects')
    await expect(page.getByTestId('project-grid')).toBeVisible()

    await page.getByTestId('view-toggle-table').click()
    await expect(page.getByTestId('project-list')).toBeVisible()
    await expect(page.getByTestId('project-row').first()).toBeVisible()

    await gotoHydrated(page, '/projects')
    await expect(page.getByTestId('project-list')).toBeVisible()

    await page.getByTestId('view-toggle-card').click()
    await expect(page.getByTestId('project-grid')).toBeVisible()
  })
})
