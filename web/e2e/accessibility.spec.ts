import AxeBuilder from '@axe-core/playwright'
import { expect, test } from '@playwright/test'
import { gotoHydrated } from './support/navigation'
import { firstProjectSlug } from './support/projects'

const WCAG_TAGS = ['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa', 'wcag22aa']
const BLOCKING_IMPACTS = new Set(['serious', 'critical'])
const PUBLIC_PAGES = ['/', '/projects', '/companies', '/technologies', '/search', '/auth/login']

async function expectNoBlockingViolations(page: Parameters<typeof AxeBuilder>[0]['page'], path: string) {
  const results = await new AxeBuilder({ page }).withTags(WCAG_TAGS).analyze()
  const blocking = results.violations.filter((violation) => BLOCKING_IMPACTS.has(violation.impact ?? ''))
  const summary = blocking.map((violation) => `${violation.id} (${violation.nodes.length}): ${violation.help}`)

  expect(summary, `serious accessibility violations on ${path}`).toEqual([])
}

test.describe('Accessibility', () => {
  for (const path of PUBLIC_PAGES) {
    test(`${path} has no serious WCAG 2.2 AA violations`, async ({ page }) => {
      await gotoHydrated(page, path)
      await expectNoBlockingViolations(page, path)
    })
  }

  test('a project page has no serious WCAG 2.2 AA violations', async ({ page, request }) => {
    const slug = await firstProjectSlug(request)
    test.skip(!slug, 'No projects seeded in this environment')

    await gotoHydrated(page, `/projects/${slug}`)
    await expectNoBlockingViolations(page, `/projects/${slug}`)
  })
})
