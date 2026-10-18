export interface NavItem {
  label: string
  to: string
}

export const navItems: NavItem[] = [
  { label: 'Projects', to: '/projects' },
  { label: 'Companies', to: '/companies' },
  { label: 'Technologies', to: '/technologies' },
  { label: 'AI Search', to: '/search' },
]

export const CV_PATH = '/Joseph Pickering CV.pdf'
export const EMAIL = 'josephpickering9@gmail.com'
export const GITHUB_URL = 'https://github.com/josephpickering9'
export const DEFAULT_LINKED_IN_URL = 'https://www.linkedin.com/in/josephpickering'
export const CAREER_START = new Date(2016, 6)
