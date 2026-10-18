import { defineEventHandler, setResponseHeader } from 'h3'

export default defineEventHandler((event) => {
  setResponseHeader(event, 'Content-Type', 'application/javascript')
  setResponseHeader(event, 'Cache-Control', 'no-cache, no-store, must-revalidate')

  return '// Workbox disabled in development mode'
})
