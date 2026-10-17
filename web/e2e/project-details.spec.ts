import { expect, test } from '@playwright/test'
import { gotoHydrated } from './support/navigation'

test.describe('Project details page', () => {
  test('loads a project case study without a console error', async ({ page }) => {
    const consoleErrors: string[] = []
    page.on('console', (message) => {
      if (message.type() === 'error') consoleErrors.push(message.text())
    })

    await gotoHydrated(page, '/projects')

    const firstProject = page.locator('[data-testid="project-grid"] a, [data-testid="project-list"] a').first()
    const hasProject = (await firstProject.count()) > 0
    test.skip(!hasProject, 'No projects seeded in this environment')

    await firstProject.click()
    await expect(page).toHaveURL(/\/projects\/[^/]+$/)
    await expect(page.getByRole('link', { name: '← All projects' })).toBeVisible()

    // The stats row is only rendered when it has real stats — never an empty shell.
    const statsRow = page.getByTestId('project-stats')
    if ((await statsRow.count()) > 0) {
      await expect(statsRow.getByTestId('project-stat').first()).toBeVisible()
    }

    expect(consoleErrors, `console errors on project details page: ${consoleErrors.join('; ')}`).toEqual([])
  })

  test('keeps each project\'s title when navigating to the next project and back', async ({ page }) => {
    await gotoHydrated(page, '/projects')

    const projectLinks = page.locator('[data-testid="project-grid"] a, [data-testid="project-list"] a')
    test.skip((await projectLinks.count()) < 2, 'Needs at least two projects seeded')
    await projectLinks.first().click()
    const nextProject = page.getByRole('link').filter({ hasText: 'Next project' })
    await expect(nextProject).toBeVisible()
    const firstTitle = await page.title()
    expect(firstTitle).toMatch(/^.+ \| Joseph Pickering$/)

    await nextProject.click()
    await expect(page).not.toHaveTitle(firstTitle)
    await expect(page).toHaveTitle(/^.+ \| Joseph Pickering$/)

    await page.goBack()
    await expect(page).toHaveTitle(firstTitle)
  })

  test('opens screenshots in a lightbox that can be stepped through and closed', async ({ page }) => {
    await gotoHydrated(page, '/projects')

    const firstProject = page.locator('[data-testid="project-grid"] a, [data-testid="project-list"] a').first()
    test.skip((await firstProject.count()) === 0, 'No projects seeded in this environment')
    await firstProject.click()
    await expect(page.getByRole('link', { name: '← All projects' })).toBeVisible()

    const galleryImages = page.getByTestId('gallery-image')
    const imageCount = await galleryImages.count()
    test.skip(imageCount === 0, 'First project has no screenshots')

    await galleryImages.first().click()
    const lightbox = page.getByTestId('project-lightbox')
    await expect(lightbox).toBeVisible()
    await expect(lightbox).toContainText(`1 / ${imageCount}`)

    if (imageCount > 1) {
      await page.keyboard.press('ArrowRight')
      await expect(lightbox).toContainText(`2 / ${imageCount}`)
      await lightbox.getByTestId('lightbox-prev').click()
      await expect(lightbox).toContainText(`1 / ${imageCount}`)
    }

    await page.keyboard.press('Escape')
    await expect(lightbox).toBeHidden()
  })

  test('shows the 404 page for an unknown project', async ({ page }) => {
    await page.goto('/projects/this-project-does-not-exist-12345')

    await expect(page.getByRole('heading', { name: 'Page not found' })).toBeVisible()
  })
})
