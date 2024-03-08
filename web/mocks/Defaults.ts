import type { CreateProject } from '../api'

export const defaultProjectForm: CreateProject = {
  title: '',
  shortDescription: '',
  description: '',
  year: new Date().getFullYear(),
  showMockup: false,
  tags: [],
}
