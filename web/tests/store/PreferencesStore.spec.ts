import { beforeEach, describe, expect, it } from 'vitest'
import { createPinia, setActivePinia } from 'pinia'
import { usePreferencesStore } from '~/store/PreferencesStore'
import { ViewMode } from '~/types/ViewMode'

describe('PreferencesStore', () => {
  beforeEach(() => setActivePinia(createPinia()))

  it('defaults to the card view and can switch to the table view', () => {
    const store = usePreferencesStore()
    expect(store.projectsViewMode).toBe(ViewMode.CARD)

    store.setProjectsViewMode(ViewMode.TABLE)

    expect(store.projectsViewMode).toBe(ViewMode.TABLE)
  })
})
