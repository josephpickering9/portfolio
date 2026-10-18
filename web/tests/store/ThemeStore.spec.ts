import { beforeEach, describe, expect, it } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { useThemeStore } from '~/store/ThemeStore'
import { DEFAULT_THEME, Theme } from '~/types/Theme'

describe('ThemeStore', () => {
  beforeEach(() => setActivePinia(createPinia()))

  it('starts on the default theme and switches theme', () => {
    const store = useThemeStore()
    expect(store.theme).toBe(DEFAULT_THEME)

    store.setSelectedTheme(Theme.EMBER)

    expect(store.theme).toBe(Theme.EMBER)
  })

  it('falls back to the default for an unknown persisted value', () => {
    const store = useThemeStore()
    store.selectedTheme = 'retired-theme'

    expect(store.theme).toBe(DEFAULT_THEME)
  })
})
