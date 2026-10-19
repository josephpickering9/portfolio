import { describe, expect, it } from 'vitest'
import { renderMarkdown } from '~/utils/markdown-helper'

function render(text: string): string {
  return renderMarkdown(text).replace(/\n/g, '')
}

describe('renderMarkdown', () => {
  it('wraps blocks in paragraphs and keeps single line breaks', () => {
    expect(render('First line\nsecond line\n\nNext block')).toBe('<p>First line<br>second line</p><p>Next block</p>')
  })

  it('renders bold and italic text', () => {
    expect(render('**Pulse** is *fast*')).toBe('<p><strong>Pulse</strong> is <em>fast</em></p>')
  })

  it('renders bullet lists', () => {
    expect(render('- Vue\n- Nuxt')).toBe('<ul><li>Vue</li><li>Nuxt</li></ul>')
  })

  it('renders a list that directly follows a paragraph line', () => {
    expect(render('He also built:\n* **Centre for Life**')).toBe(
      '<p>He also built:</p><ul><li><strong>Centre for Life</strong></li></ul>',
    )
  })

  it('renders nested and numbered lists', () => {
    expect(render('1. Alnwick Castle\n   * C#\n   * SQL Server')).toBe(
      '<ol><li>Alnwick Castle<ul><li>C#</li><li>SQL Server</li></ul></li></ol>',
    )
  })

  it('renders links that open in a new tab', () => {
    expect(render('[alnwickcastle.com](https://www.alnwickcastle.com/)')).toBe(
      '<p><a href="https://www.alnwickcastle.com/" target="_blank" rel="noopener noreferrer">alnwickcastle.com</a></p>',
    )
  })

  it('drops unsafe link protocols', () => {
    expect(render('[click](javascript:alert(1))')).toBe('<p>[click](javascript:alert(1))</p>')
  })

  it('escapes HTML', () => {
    expect(render('<img src=x onerror=alert(1)>')).toBe('<p>&lt;img src=x onerror=alert(1)&gt;</p>')
  })
})
