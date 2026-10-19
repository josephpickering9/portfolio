import type { Page } from '@playwright/test'

export function projectItems(page: Page) {
  return page.getByTestId('project-card').or(page.getByTestId('project-row'))
}
