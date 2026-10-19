import { TagType } from '@api'
import type { TagTypeDefinition, TagTypeValue } from '~/types/TagTypeValue'
import { typedEntries } from '~/utils/array-helper'

export const tagTypeDefinitions: Record<TagType, TagTypeDefinition> = {
  [TagType.DEFAULT]: { label: 'Default', icon: 'heroicons:tag' },
  [TagType.AI]: { label: 'AI', icon: 'heroicons:sparkles' },
  [TagType.BACKEND]: { label: 'Backend', icon: 'heroicons:server' },
  [TagType.FRONTEND]: { label: 'Frontend', icon: 'heroicons:computer-desktop' },
  [TagType.DEV_OPS]: { label: 'DevOps', icon: 'heroicons:circle-stack' },
  [TagType.DATA]: { label: 'Data', icon: 'heroicons:chart-bar' },
  [TagType.CMS]: { label: 'CMS', icon: 'heroicons:document-text' },
  [TagType.MOBILE]: { label: 'Mobile', icon: 'heroicons:device-phone-mobile' },
  [TagType.OTHER]: { label: 'Other', icon: 'heroicons:ellipsis-horizontal-circle' },
}

export const tagTypes: TagTypeValue[] = typedEntries(tagTypeDefinitions).map(([value, definition]) => ({ value, ...definition }))
