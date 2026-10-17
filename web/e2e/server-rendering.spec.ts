import { expect, test, type APIRequestContext } from '@playwright/test'
import { gotoHydrated } from './support/navigation'

const SKELETON_CLASS = 'skeleton-pulse'
const HYDRATION_WARNING = /hydration/i
const PROJECT_LINK = /href="\/projects\/(?!new")([^"/]+)"/

async function firstProjectSlug(request: APIRequestContext): Promise<string | undefined> {
  const html = await (await request.get('/projects')).text()
  return html.match(PROJECT_LINK)?.[1]
}

test.describe('Server rendering', () => {
  for (const path of ['/', '/projects', '/companies']) {
    test(`renders ${path} with its data instead of skeletons`, async ({ request }) => {
      const html = await (await request.get(path)).text()

      expect(html).not.toContain(SKELETON_CLASS)
      expect(html).toMatch(PROJECT_LINK)
    })
  }

  test('renders a project page with its own title and social meta', async ({ request }) => {
    const slug = await firstProjectSlug(request)
    test.skip(!slug, 'No projects seeded in this environment')

    const response = await request.get(`/projects/${slug}`)
    const html = await response.text()

    expect(response.status()).toBe(200)
    expect(html).not.toContain(SKELETON_CLASS)
    const title = html.match(/<title>([^<]+)<\/title>/)?.[1]
    expect(title).toMatch(/^.+ \| Joseph Pickering$/)
    expect(html).toContain(`<meta property="og:title" content="${title}"`)
  })

  test('renders the hero prompt suggestions on the server', async ({ browser }) => {
    const context = await browser.newContext({ javaScriptEnabled: false })
    const page = await context.newPage()

    await page.goto('/')

    const suggestions = page.getByRole('group', { name: 'Suggested questions' }).getByRole('button')
    expect(await suggestions.count()).toBeGreaterThan(1)
    await context.close()
  })

  test('responds 404 for an unknown project', async ({ request }) => {
    const response = await request.get('/projects/this-project-does-not-exist')

    expect(response.status()).toBe(404)
  })

  test('hydrates public pages without mismatches', async ({ page, request }) => {
    const slug = await firstProjectSlug(request)
    const paths = ['/', '/projects', '/companies', ...(slug ? [`/projects/${slug}`] : [])]
    const warnings: string[] = []
    page.on('console', (message) => {
      if (HYDRATION_WARNING.test(message.text())) warnings.push(`${page.url()}: ${message.text()}`)
    })

    for (const path of paths) {
      await gotoHydrated(page, path)
      await page.waitForLoadState('networkidle')
    }

    expect(warnings).toEqual([])
  })
})
