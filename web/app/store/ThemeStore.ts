import { defineStore } from 'pinia'
import type { Theme } from '~/types/Theme'
import { DEFAULT_THEME, isTheme } from '~/types/Theme'

export const useThemeStore = defineStore('themeStore', {
  state: () => ({
    selectedTheme: DEFAULT_THEME as string,
  }),
  getters: {
    theme: (state): Theme => (isTheme(state.selectedTheme) ? state.selectedTheme : DEFAULT_THEME),
  },
  actions: {
    setSelectedTheme(theme: Theme) {
      this.selectedTheme = theme
    },
  },
  persist: true,
})
