import { describe, expect, it } from 'vitest'
import { renderBasicMarkdown } from '~/utils/markdown-helper'

describe('renderBasicMarkdown', () => {
  it('wraps blocks in paragraphs and keeps single line breaks', () => {
    expect(renderBasicMarkdown('First line\nsecond line\n\nNext block')).toBe('<p>First line<br>second line</p><p>Next block</p>')
  })

  it('renders bold and italic text', () => {
    expect(renderBasicMarkdown('**Pulse** is *fast*')).toBe('<p><strong>Pulse</strong> is <em>fast</em></p>')
  })

  it('renders bullet lists', () => {
    expect(renderBasicMarkdown('- Vue\n* Nuxt')).toBe('<ul><li>Vue</li><li>Nuxt</li></ul>')
  })

  it('renders headings as bold paragraphs, even without a blank line after them', () => {
    expect(renderBasicMarkdown('### Web apps\n- Vue')).toBe('<p><strong>Web apps</strong></p><ul><li>Vue</li></ul>')
  })

  it('escapes HTML', () => {
    expect(renderBasicMarkdown('<img src=x onerror=alert(1)>')).toBe('<p>&lt;img src=x onerror=alert(1)&gt;</p>')
  })
})
