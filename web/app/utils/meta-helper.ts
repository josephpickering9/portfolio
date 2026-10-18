export const SITE_NAME = 'Joseph Pickering'

export function formatPageTitle(title?: string): string {
  return title ? `${title} | ${SITE_NAME}` : SITE_NAME
}
