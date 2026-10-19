# 0005. Public pages load their data during SSR; the CMS stays client-side

- **Status:** Accepted
- **Date:** 2026-10

## Context

The site's audience is recruiters, clients and link unfurlers (LinkedIn, Slack). Some public pages loaded data in `onMounted`, so crawlers and unfurlers saw skeletons and generic meta.

## Decision

Public pages load through `useAsyncData` with `{ lazy: true }`, awaited only on the server, and set reactive meta, canonical links and JSON-LD from the loaded data. Stores are options-style Pinia stores whose state is SSR-serialised. Auth-gated CMS pages stay client-loaded, and the Auth0 SDK is downloaded only on those pages, for a returning signed-in visitor, or on the login callback.

## Consequences

- E2E asserts that the HTML of each public page contains its content, not skeletons.
- Anything in store state ends up in the page payload, which is why list payload size mattered (ADR 0004).
