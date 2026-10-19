import { expect, test } from '@playwright/test'
import { gotoHydrated } from './support/navigation'
import { projectItems } from './support/selectors'

const TITLE_PATTERN = /^.+ \| Joseph Pickering$/

test.describe('Project details page', () => {
  test('loads a project case study without a console error', async ({ page }) => {
    const consoleErrors: string[] = []
    page.on('console', (message) => {
      if (message.type() === 'error') consoleErrors.push(message.text())
    })

    await gotoHydrated(page, '/projects')

    const firstProject = projectItems(page).first()
    test.skip((await firstProject.count()) === 0, 'No projects seeded in this environment')

    await firstProject.click()
    await expect(page).toHaveURL(/\/projects\/[^/]+$/)
    await expect(page.getByTestId('project-back-link')).toBeVisible()
    await expect(page.getByTestId('project-title')).not.toBeEmpty()

    const statsRow = page.getByTestId('project-stats')
    if ((await statsRow.count()) > 0) {
      await expect(statsRow.getByTestId('project-stat').first()).toBeVisible()
    }

    expect(consoleErrors, `console errors on project details page: ${consoleErrors.join('; ')}`).toEqual([])
  })

  test("keeps each project's title when navigating to the next project and back", async ({ page }) => {
    await gotoHydrated(page, '/projects')

    test.skip((await projectItems(page).count()) < 2, 'Needs at least two projects seeded')
    await projectItems(page).first().click()
    const nextProject = page.getByTestId('next-project')
    await expect(nextProject).toBeVisible()
    const firstTitle = await page.title()
    expect(firstTitle).toMatch(TITLE_PATTERN)

    await nextProject.click()
    await expect(page).not.toHaveTitle(firstTitle)
    await expect(page).toHaveTitle(TITLE_PATTERN)

    await page.goBack()
    await expect(page).toHaveTitle(firstTitle)
  })

  test('opens screenshots in a lightbox that can be stepped through and closed', async ({ page }) => {
    await gotoHydrated(page, '/projects')

    const firstProject = projectItems(page).first()
    test.skip((await firstProject.count()) === 0, 'No projects seeded in this environment')
    await firstProject.click()
    await expect(page.getByTestId('project-back-link')).toBeVisible()

    const galleryImages = page.getByTestId('gallery-image')
    const imageCount = await galleryImages.count()
    test.skip(imageCount === 0, 'First project has no screenshots')

    await galleryImages.first().click()
    const lightbox = page.getByTestId('project-lightbox')
    await expect(lightbox).toBeVisible()
    await expect(lightbox.getByTestId('lightbox-image')).toBeVisible()
    await expect(lightbox.getByTestId('lightbox-position')).toContainText(`1 / ${imageCount}`)

    if (imageCount > 1) {
      await page.keyboard.press('ArrowRight')
      await expect(lightbox.getByTestId('lightbox-position')).toContainText(`2 / ${imageCount}`)
      await lightbox.getByTestId('lightbox-prev').click()
      await expect(lightbox.getByTestId('lightbox-position')).toContainText(`1 / ${imageCount}`)
    }

    await page.keyboard.press('Escape')
    await expect(lightbox).toBeHidden()

    await galleryImages.first().click()
    await lightbox.getByTestId('lightbox-close').click()
    await expect(lightbox).toBeHidden()
  })

  test('shows the 404 page for an unknown project', async ({ page }) => {
    await page.goto('/projects/this-project-does-not-exist-12345')

    await expect(page.getByTestId('error-heading')).toHaveText('Page not found')
  })
})
