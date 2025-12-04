export const toTitleCase = (str: string): string => {
  return str.replace(/\w\S*/g, (txt: string) => {
    return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase()
  })
}

export const textConverter = (str: string): string => {
  switch (str.toLowerCase()) {
    case 'cms':
      return 'CMS'
    case 'dev_ops':
      return 'DevOps'
  }

  return str
}

export default {
  toTitleCase,
  textConverter,
}
