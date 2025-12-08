import type { CreateProject } from '~/api'

export const defaultProjectForm: CreateProject = {
  title: '',
  shortDescription: '',
  description: '',
  startDate: '',
  endDate: undefined,
  showMockup: false,
  tags: [],
}
