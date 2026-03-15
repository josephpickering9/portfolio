import { defineStore } from 'pinia'
import { ViewMode } from '~/types/ViewMode'

export const usePreferencesStore = defineStore('preferences', {
    state: () => ({
        projectsViewMode: ViewMode.CARD as ViewMode,
    }),
    actions: {
        setProjectsViewMode(mode: ViewMode) {
            this.projectsViewMode = mode
        },
    },
    persist: true,
})
