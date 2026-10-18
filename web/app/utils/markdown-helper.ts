const BULLET = /^\s*[-*] /
const HEADING = /^#{1,6}\s+(.+)$/gm
const HEADING_MARKER = '\u0000'

function escapeHtml(text: string): string {
  return text.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;')
}

function renderInline(text: string): string {
  return text.replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>').replace(/\*(.+?)\*/g, '<em>$1</em>')
}

export function renderBasicMarkdown(markdown: string): string {
  return escapeHtml(markdown)
    .replace(HEADING, `\n\n${HEADING_MARKER}$1\n\n`)
    .split(/\n{2,}/)
    .filter((block) => block.trim())
    .map((block) => {
      if (block.startsWith(HEADING_MARKER)) return `<p><strong>${renderInline(block.slice(1).trim())}</strong></p>`

      const lines = block.split('\n').filter((line) => line.trim())
      if (lines.every((line) => BULLET.test(line))) {
        return `<ul>${lines.map((line) => `<li>${renderInline(line.replace(BULLET, ''))}</li>`).join('')}</ul>`
      }
      return `<p>${lines.map(renderInline).join('<br>')}</p>`
    })
    .join('')
}
