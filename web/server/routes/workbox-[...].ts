export default defineEventHandler((event) => {
    // Handle any workbox-related requests in development
    setResponseHeader(event, 'Content-Type', 'application/javascript')
    setResponseHeader(event, 'Cache-Control', 'no-cache, no-store, must-revalidate')

    return '// Workbox disabled in development mode'
})
