# Architecture decision records

Short records of the decisions that shape this codebase: what the context was, what was decided and what it costs. Newer records can supersede older ones; the code quality backlog (`docs/CODE_QUALITY_BACKLOG.md`) has the audit trail behind most of them.

| # | Decision |
|---|---|
| [0001](0001-generated-api-client.md) | Generate the frontend API client from the Swagger spec |
| [0002](0002-result-type-and-error-codes.md) | Expected failures are `Result<T>` values with snake_case error codes |
| [0003](0003-tests-against-real-postgresql.md) | Backend tests run against real PostgreSQL |
| [0004](0004-no-pagination-summary-dtos.md) | No pagination; list endpoints return summary DTOs |
| [0005](0005-ssr-public-pages.md) | Public pages load their data during SSR; the CMS stays client-side |
| [0006](0006-nonce-based-csp.md) | A nonce-based Content Security Policy via nuxt-security |
| [0007](0007-single-droplet-scripted-releases.md) | One droplet with scripted releases, not containers |
