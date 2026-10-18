import { TagType } from '@api'
import type { CreateTag, Tag } from '@api'
import { tagTypeDefinitions } from '~/data/TagTypes'

const TAG_TYPES = new Set<string>(Object.values(TagType))

function isTagType(value: string): value is TagType {
  return TAG_TYPES.has(value)
}

export function parseTagType(value?: string): TagType | undefined {
  return value && isTagType(value) ? value : undefined
}

export function getTagIcon(tag: { icon?: Tag['icon']; type?: Tag['type'] }): string {
  return tag.icon || tagTypeDefinitions[tag.type ?? TagType.DEFAULT].icon
}

export function getTagTypeLabel(type: TagType): string {
  return tagTypeDefinitions[type].label
}

export function tagToForm(tag: Tag): CreateTag {
  return { title: tag.title, type: tag.type, icon: tag.icon ?? '', customColour: tag.customColour }
}
