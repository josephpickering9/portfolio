import type { SelectListItem } from '../types/SelectListItem'
import { toTitleCase, textConverter } from './string-helper'

export const enumToSelectListItem = (obj: any, titleCase: boolean = true): Array<SelectListItem> => {
  return Object.keys(obj).map((key: string) => {
    return {
      text: textConverter(titleCase ? toTitleCase(key) : key),
      value: obj[key],
    }
  })
}

export const getEnumValue = <E extends { [key: string]: string }>(enumObj: E, value: string): E[keyof E] | undefined => {
  const entries = Object.entries(enumObj)
  for (const [key, enumValue] of entries) {
    if (enumValue === value) return enumObj[key as keyof typeof enumObj]
  }
  return undefined
}

export default {
  enumToSelectListItem,
  getEnumValue,
}
