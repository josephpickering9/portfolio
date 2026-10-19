# 0006. A nonce-based Content Security Policy via nuxt-security

- **Status:** Accepted
- **Date:** 2026-10

## Context

The site had no security headers, while the CMS keeps Auth0 tokens in `localStorage`, so an XSS would have been enough to take over the CMS. Nuxt SSR emits inline scripts, so a static allow-list CSP would need `'unsafe-inline'`.

## Decision

`nuxt-security` generates a per-request nonce and adds SRI hashes to every script. The policy is `script-src 'self' 'strict-dynamic' 'nonce-…'`, `object-src 'none'`, `base-uri 'none'` and `frame-ancestors 'none'`, with `connect-src`/`img-src`/`frame-src` built at build time from the API and Auth0 origins. The one inline event handler `@nuxt/image` renders is allowed by its exact hash under `'unsafe-hashes'`, not by `'unsafe-inline'`. The API sends its own `default-src 'none'` policy, exempting Swagger UI.

## Consequences

- New third-party origins have to be added to the policy in `nuxt.config.ts`; E2E fails on any CSP violation because it fails on console errors.
- `style-src` still allows `'unsafe-inline'` for Vue's style bindings.
