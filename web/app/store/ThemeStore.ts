import { defineStore } from 'pinia'

export const useThemeStore = defineStore('themeStore', {
  state: () => ({
    selectedTheme: 'dark' as string,
  }),
  actions: {
    setSelectedTheme(theme: string) {
      this.selectedTheme = theme
    },
  },
  persist: true,
})
