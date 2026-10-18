export default defineEventHandler((event) => {
    // In development, return an empty service worker to prevent Vue Router warnings
    // The actual service worker is only used in production builds
    setResponseHeader(event, 'Content-Type', 'application/javascript')
    setResponseHeader(event, 'Cache-Control', 'no-cache, no-store, must-revalidate')

    return `
// Service worker is disabled in development mode
// This file prevents 404 errors from cached service worker requests
self.addEventListener('install', () => {
  self.skipWaiting();
});

self.addEventListener('activate', () => {
  self.registration.unregister();
});
`.trim()
})
