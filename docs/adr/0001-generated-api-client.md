# 0001. Generate the frontend API client from the Swagger spec

- **Status:** Accepted
- **Date:** 2025-12

## Context

The frontend and backend live in one repo but have separate toolchains. A hand-written client drifts silently: a renamed field on the API becomes `undefined` in a component, and nothing fails until a user notices.

## Decision

The backend's Swagger document is the contract. `@hey-api/openapi-ts` generates `web/api/` from it (`yarn codegen`), the generated files are never edited by hand, and components reach the API only through Pinia store actions. The E2E workflow regenerates the client against the API it starts and fails if the committed copy differs.

## Consequences

- A backend change surfaces as TypeScript errors in exactly the stores and components that consume it.
- Swagger has to describe responses precisely: `ActionResult<T>` return types, `[ProducesResponseType]` for 201/204, `[Required]` on DTO properties that are always present.
- Swagger doesn't describe error bodies, so the error contract needs its own drift check (ADR 0002).
