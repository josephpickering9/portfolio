import { expect, type Page, test } from '@playwright/test'

async function openSearchInput(page: Page) {
  await page.getByRole('button', { name: 'Filter' }).click()
  return page.getByPlaceholder('Search projects...')
}

test.describe('Project search & filter', () => {
  test('lists projects and lets a visitor open one', async ({ page }) => {
    await page.goto('/projects')

    await expect(page.getByRole('heading', { name: 'Projects' })).toBeVisible()

    const firstProject = page.locator('a[href^="/projects/"]').first()
    await expect(firstProject).toBeVisible()

    await firstProject.click()
    await expect(page).toHaveURL(/\/projects\/[^/]+$/)
  })

  test('filters the project list by search term and clears back to the full list', async ({ page }) => {
    await page.goto('/projects')

    const projectLinks = page.locator('a[href^="/projects/"]')
    await expect(projectLinks.first()).toBeVisible()
    const initialCount = await projectLinks.count()

    const search = await openSearchInput(page)
    await search.fill('zzz-no-project-should-match-zzz')

    await expect(page.getByText('No projects found')).toBeVisible()

    await search.fill('')

    await expect(projectLinks).toHaveCount(initialCount)
  })

  test('shows an empty state when no projects match the search term', async ({ page }) => {
    await page.goto('/projects')

    const search = await openSearchInput(page)
    await search.fill('this-will-not-match-anything-12345')

    await expect(page.getByText('No projects found')).toBeVisible()
  })

  test('reflects the search term in the URL query string', async ({ page }) => {
    await page.goto('/projects')

    const search = await openSearchInput(page)
    await search.fill('portfolio')

    await expect(page).toHaveURL(/[?&]search=portfolio/)
  })
})
