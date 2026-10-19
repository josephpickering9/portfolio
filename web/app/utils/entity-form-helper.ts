export interface EntityFormCopy {
  eyebrow: string
  title: string
  saveLabel: string
  deleteTitle: string
}

export function getEntityFormCopy(entityName: string, isUpdate: boolean, title?: string | null): EntityFormCopy {
  const fallbackTitle = isUpdate ? `Untitled ${entityName}` : `New ${entityName}`
  return {
    eyebrow: isUpdate ? `Update ${entityName}` : `New ${entityName}`,
    title: title || fallbackTitle,
    saveLabel: isUpdate ? 'Save changes' : `Create ${entityName}`,
    deleteTitle: `Delete this ${entityName}?`,
  }
}
