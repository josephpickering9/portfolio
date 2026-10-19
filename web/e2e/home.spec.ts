import { expect, test } from '@playwright/test'
import { gotoHydrated } from './support/navigation'

test.describe('Home page', () => {
  test('shows the selected work and opens a featured project', async ({ page }) => {
    await gotoHydrated(page, '/')

    const featured = page.getByTestId('featured-projects').getByTestId('project-card')
    await expect(featured.first()).toBeVisible()

    await featured.first().click()
    await expect(page).toHaveURL(/\/projects\/[^/]+$/)
  })

  test('sends a typed question to AI search', async ({ page }) => {
    await page.route('**/vertex/query', (route) => route.fulfill({ json: { answer: 'Mocked answer', citations: [] } }))
    await gotoHydrated(page, '/')

    await page.getByTestId('hero-ai-input').fill('What has Joe built with Vue?')
    await page.getByTestId('hero-ai-submit').click()

    await expect(page).toHaveURL(/\/search\?q=/)
    expect(new URL(page.url()).searchParams.get('q')).toBe('What has Joe built with Vue?')
    await expect(page.getByTestId('ai-search-question')).toHaveText('What has Joe built with Vue?')
  })

  test('sends a suggested prompt to AI search', async ({ page }) => {
    await page.route('**/vertex/query', (route) => route.fulfill({ json: { answer: 'Mocked answer', citations: [] } }))
    await gotoHydrated(page, '/')

    const prompt = page.getByTestId('hero-suggested-prompt').first()
    const text = (await prompt.textContent())?.trim() ?? ''
    await prompt.click()

    await expect(page).toHaveURL(/\/search\?q=/)
    await expect(page.getByTestId('ai-search-question')).toHaveText(text)
  })

  test('links the "You\'re looking at it" card to the case study and the source', async ({ page }) => {
    await gotoHydrated(page, '/')

    await expect(page.getByTestId('this-site-case-study-link')).toHaveAttribute('href', '/projects/portfolio')
    await expect(page.getByTestId('this-site-source-link')).toHaveAttribute('href', 'https://github.com/josephpickering9/portfolio')
    await expect(page.getByTestId('footer-source-link')).toHaveAttribute('href', 'https://github.com/josephpickering9/portfolio')
  })
})
