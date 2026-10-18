import { defineEventHandler, setResponseHeader } from 'h3'

// Dev-only stand-in: unregisters any service worker cached from a production build so it can't intercept dev requests.
export default defineEventHandler((event) => {
  setResponseHeader(event, 'Content-Type', 'application/javascript')
  setResponseHeader(event, 'Cache-Control', 'no-cache, no-store, must-revalidate')

  return `
self.addEventListener('install', () => {
  self.skipWaiting();
});

self.addEventListener('activate', () => {
  self.registration.unregister();
});
`.trim()
})
