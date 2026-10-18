import type { TagType } from '@api'

export interface TagTypeDefinition {
  label: string
  icon: string
}

export interface TagTypeValue extends TagTypeDefinition {
  value: TagType
}
