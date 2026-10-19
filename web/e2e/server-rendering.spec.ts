import { expect, test } from '@playwright/test'
import { gotoHydrated } from './support/navigation'
import { firstProjectSlug } from './support/projects'

const SKELETON = 'data-testid="skeleton"'
const HYDRATION_WARNING = /hydration/i

const SSR_PAGES: Array<{ path: string; content: string }> = [
  { path: '/', content: 'data-testid="project-card"' },
  { path: '/projects', content: 'data-testid="project-card"' },
  { path: '/companies', content: 'data-testid="company-timeline-item"' },
  { path: '/technologies', content: 'data-testid="tag-tile"' },
]

test.describe('Server rendering', () => {
  for (const { path, content } of SSR_PAGES) {
    test(`renders ${path} with its data instead of skeletons`, async ({ request }) => {
      const html = await (await request.get(path)).text()

      expect(html).not.toContain(SKELETON)
      expect(html).toContain(content)
    })
  }

  test('renders a project page with its own title and social meta', async ({ request }) => {
    const slug = await firstProjectSlug(request)
    test.skip(!slug, 'No projects seeded in this environment')

    const response = await request.get(`/projects/${slug}`)
    const html = await response.text()

    expect(response.status()).toBe(200)
    expect(html).not.toContain(SKELETON)
    const title = html.match(/<title>([^<]+)<\/title>/)?.[1]
    expect(title).toMatch(/^.+ \| Joseph Pickering$/)
    expect(html).toContain(`<meta property="og:title" content="${title}"`)
  })

  test('renders the suggested prompts on the server', async ({ browser }) => {
    const context = await browser.newContext({ javaScriptEnabled: false })
    const page = await context.newPage()

    await page.goto('/')
    expect(await page.getByTestId('hero-suggested-prompt').count()).toBeGreaterThan(1)

    await page.goto('/search')
    expect(await page.getByTestId('suggested-prompt').count()).toBeGreaterThan(1)
    await context.close()
  })

  test('responds 404 for an unknown project', async ({ request }) => {
    const response = await request.get('/projects/this-project-does-not-exist')

    expect(response.status()).toBe(404)
  })

  test('hydrates public pages without mismatches', async ({ page, request }) => {
    const slug = await firstProjectSlug(request)
    const paths = ['/', '/projects', '/companies', '/technologies', '/search', ...(slug ? [`/projects/${slug}`] : [])]
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
