import { describe, expect, it } from 'vitest'
import { tabId, tabPanelId } from '~/utils/tab-helper'

describe('tab ids', () => {
  it('pairs each tab with its panel', () => {
    expect(tabId(2)).toBe('tab-2')
    expect(tabPanelId(2)).toBe('tab-panel-2')
  })
})
