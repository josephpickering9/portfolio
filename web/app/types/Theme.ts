export enum Theme {
  MIDNIGHT = 'midnight',
  EMBER = 'ember',
  MOSS = 'moss',
  DAYLIGHT = 'daylight',
}

export const DEFAULT_THEME = Theme.MIDNIGHT

export function isTheme(value: unknown): value is Theme {
  return Object.values(Theme).includes(value as Theme)
}
