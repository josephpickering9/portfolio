import { expect, test } from '@playwright/test'
import { firstProjectSlug } from './support/projects'

const SITE_URL = 'https://josephpickering.co.uk'

function canonicalHref(html: string): string | undefined {
  return html.match(/<link[^>]*rel="canonical"[^>]*href="([^"]+)"/)?.[1]
}

function jsonLdBlocks(html: string): Array<Record<string, unknown>> {
  return [...html.matchAll(/<script[^>]*type="application\/ld\+json"[^>]*>(.*?)<\/script>/gs)].map((match) =>
    JSON.parse(match[1] ?? '{}'),
  )
}

test.describe('SEO', () => {
  test('the home page has a canonical URL and Person structured data', async ({ request }) => {
    const html = await (await request.get('/')).text()

    expect(canonicalHref(html)).toBe(`${SITE_URL}/`)
    expect(jsonLdBlocks(html)).toContainEqual(expect.objectContaining({ '@type': 'Person', url: SITE_URL }))
  })

  test('a project page has CreativeWork structured data', async ({ request }) => {
    const slug = await firstProjectSlug(request)
    test.skip(!slug, 'No projects seeded in this environment')

    const html = await (await request.get(`/projects/${slug}`)).text()

    expect(canonicalHref(html)).toBe(`${SITE_URL}/projects/${slug}`)
    expect(jsonLdBlocks(html)).toContainEqual(
      expect.objectContaining({ '@type': 'CreativeWork', url: `${SITE_URL}/projects/${slug}` }),
    )
  })

  test('the old case study URL redirects permanently to the new one', async ({ request }) => {
    const response = await request.get('/projects/work-experience', { maxRedirects: 0 })

    expect(response.status()).toBe(301)
    expect(response.headers()['location']).toBe('/projects/portfolio')
  })
})

test.describe('Security headers', () => {
  test('pages are served with a nonce-based CSP and without framework fingerprints', async ({ request }) => {
    const headers = (await request.get('/')).headers()

    expect(headers['content-security-policy']).toMatch(/script-src [^;]*'nonce-[^']+'/)
    expect(headers['content-security-policy']).toContain("frame-ancestors 'none'")
    expect(headers['x-content-type-options']).toBe('nosniff')
    expect(headers['referrer-policy']).toBe('strict-origin-when-cross-origin')
    expect(headers['x-powered-by']).toBeUndefined()
  })

  test('hashed build assets are cached immutably', async ({ request }) => {
    const html = await (await request.get('/')).text()
    const asset = html.match(/\/_nuxt\/[\w.-]+\.js/)?.[0]
    expect(asset).toBeDefined()

    const headers = (await request.get(asset ?? '')).headers()

    expect(headers['cache-control']).toBe('public, max-age=31536000, immutable')
  })
})
