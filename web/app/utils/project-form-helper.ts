import { ImageType } from '@api'
import type { CreateProject, Project, ProjectImage } from '@api'
import { fromExistingImage } from '~/utils/upload-helper'
import type { ProjectImageSlots } from '~/utils/upload-helper'
import { getImagesByType } from '~/utils/project-helper'
import { toDateInputValue } from '~/utils/date-helper'

export function projectToForm(project: Project): CreateProject {
  return {
    title: project.title,
    shortDescription: project.shortDescription,
    description: project.description,
    problemStatement: project.problemStatement ?? undefined,
    keyDecision: project.keyDecision ?? undefined,
    outcome: project.outcome ?? undefined,
    companyId: project.companyId ?? undefined,
    startDate: toDateInputValue(project.startDate),
    endDate: toDateInputValue(project.endDate),
    website: project.website ?? '',
    showMockup: project.showMockup,
    tags: project.tags.map((tag) => tag.title),
    repositories: project.repositories.map(({ id, title, url, order }) => ({ id, title, url, order })),
    stats: project.stats.map(({ id, label, value, order }) => ({ id, label, value, order })),
  }
}

export function projectToImageSlots(project: Project, toUrl: (image: string) => string): ProjectImageSlots {
  const single = (image?: ProjectImage) => (image ? fromExistingImage(image, toUrl(image.image)) : null)
  const gallery = (type: ImageType) => getImagesByType(project, type).map((image) => fromExistingImage(image, toUrl(image.image)))

  return {
    banner: single(project.banner),
    card: single(project.card),
    logo: single(project.logo),
    desktop: gallery(ImageType.DESKTOP),
    mobile: gallery(ImageType.MOBILE),
  }
}
