import type { APIRequestContext } from '@playwright/test'

const PROJECT_CARD_SLUG = /<a[^>]*href="\/projects\/([^"/?]+)"[^>]*data-testid="project-card"/

export async function firstProjectSlug(request: APIRequestContext): Promise<string | undefined> {
  const html = await (await request.get('/projects')).text()
  return html.match(PROJECT_CARD_SLUG)?.[1]
}
