import { expect, test, type Page } from '@playwright/test'
import { gotoHydrated } from './support/navigation'

const QUESTION = 'Which projects use Vue?'
const CITED_PROJECT = {
  id: 'cited-1',
  slug: 'cited-project',
  title: 'Cited Project',
  shortDescription: 'A project the answer cites',
  description: '',
  showMockup: false,
  images: [],
  repositories: [],
  stats: [],
  tags: [],
}
const ANSWER = { answer: 'Joe has built **several** Vue apps.', citations: [{ id: CITED_PROJECT.id, featureType: 'Project', project: CITED_PROJECT }] }

async function mockQuery(page: Page, responses: Array<{ status: number; json: unknown }>) {
  const queue = [...responses]
  await page.route('**/vertex/query', (route) => {
    const next = queue.shift() ?? responses[responses.length - 1]
    return route.fulfill({ status: next?.status ?? 200, json: next?.json ?? {} })
  })
}

test.describe('AI search', () => {
  test('answers a question with cited projects and resets for a new search', async ({ page }) => {
    await mockQuery(page, [{ status: 200, json: ANSWER }])
    await gotoHydrated(page, '/search')

    await page.getByTestId('ai-search-input').fill(QUESTION)
    await page.getByTestId('ai-search-submit').click()

    await expect(page).toHaveURL(/\/search\?q=/)
    await expect(page.getByTestId('ai-search-question')).toHaveText(QUESTION)
    await expect(page.getByTestId('ai-search-answer')).toContainText('Joe has built several Vue apps.')
    await expect(page.getByTestId('ai-search-citation')).toHaveCount(1)
    await expect(page.getByTestId('ai-search-citation')).toContainText(CITED_PROJECT.title)

    await page.getByTestId('ai-search-reset').click()
    await expect(page.getByTestId('ai-search-result')).toBeHidden()
    await expect(page.getByTestId('ai-search-input')).toHaveValue('')
    await expect(page).toHaveURL(/\/search$/)
  })

  test('runs the question from the URL when the page opens', async ({ page }) => {
    await mockQuery(page, [{ status: 200, json: ANSWER }])

    await gotoHydrated(page, `/search?q=${encodeURIComponent(QUESTION)}`)

    await expect(page.getByTestId('ai-search-question')).toHaveText(QUESTION)
    await expect(page.getByTestId('ai-search-answer')).toContainText('several')
  })

  test('shows the unavailable state and recovers on retry', async ({ page }) => {
    await mockQuery(page, [
      { status: 503, json: { error: 'upstream_unavailable' } },
      { status: 200, json: ANSWER },
    ])
    await gotoHydrated(page, '/search')

    await page.getByTestId('suggested-prompt').first().click()
    await expect(page.getByTestId('ai-search-error')).toContainText('AI search is unavailable right now.')

    await page.getByTestId('ai-search-retry').click()
    await expect(page.getByTestId('ai-search-error')).toBeHidden()
    await expect(page.getByTestId('ai-search-answer')).toContainText('several')
  })
})
