import MarkdownIt from 'markdown-it'

const markdown = new MarkdownIt({ html: false, breaks: true, linkify: true })
// linkify-it 6 (markdown-it 15) stopped linking bare domains by default; AI answers cite them that way.
markdown.linkify.set({ fuzzyLink: true })

markdown.renderer.rules['link_open'] = (tokens, index, options, _env, self) => {
  const token = tokens[index]
  if (token) {
    token.attrSet('target', '_blank')
    token.attrSet('rel', 'noopener noreferrer')
  }
  return self.renderToken(tokens, index, options)
}

export function renderMarkdown(text: string): string {
  return markdown.render(text)
}
