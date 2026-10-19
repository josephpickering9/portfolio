import { expect, test } from '@playwright/test'
import { gotoHydrated } from './support/navigation'

test.describe('Companies timeline', () => {
  test('lists companies with links to their projects', async ({ page }) => {
    await gotoHydrated(page, '/companies')

    const items = page.getByTestId('company-timeline-item')
    await expect(items.first()).toBeVisible()

    const projectLink = page.getByTestId('company-project-link').first()
    test.skip((await projectLink.count()) === 0, 'No company has projects in this environment')
    await projectLink.click()
    await expect(page).toHaveURL(/\/projects\/[^/]+$/)
  })
})
