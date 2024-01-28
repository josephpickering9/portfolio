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

export default {
  enumToSelectListItem,
}
