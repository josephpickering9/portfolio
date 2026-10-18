import { TagType } from '@api'
import type { CreateCompany, CreateProject, CreateTag } from '@api'
import type { ProjectImageSlots } from '~/utils/upload-helper'

export const defaultProjectForm: CreateProject = {
  title: '',
  shortDescription: '',
  description: '',
  startDate: '',
  endDate: undefined,
  showMockup: false,
  tags: [],
}

export const defaultCompanyForm: CreateCompany = {
  name: '',
  description: '',
  website: '',
  startDate: undefined,
  endDate: undefined,
}

export const defaultTagForm: CreateTag = {
  title: '',
  type: TagType.FRONTEND,
  icon: '',
  customColour: undefined,
}

export const emptyImageSlots: ProjectImageSlots = { banner: null, card: null, logo: null, desktop: [], mobile: [] }
