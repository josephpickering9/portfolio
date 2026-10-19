# Portfolio — Monorepo

Two projects in one git repo, not a package-manager-linked monorepo:

- **`api/`** — .NET 10 Web API + PostgreSQL backend. See `api/CLAUDE.md`.
- **`web/`** — Nuxt 4 / Vue 3 frontend. See `web/CLAUDE.md`.

Each has its own dependency graph, CI pipeline and CLAUDE.md with project-specific conventions. This file only covers things that span both.

## How they connect

The frontend's API client (`web/api/`) is **generated from the backend's Swagger spec**, not hand-written. Workflow when the API surface changes:

1. Run the backend locally (`dotnet run --project api/Portfolio.Api`, serves Swagger at `http://localhost:5105/swagger/v1/swagger.json`).
2. In `web/`, run `yarn codegen` to regenerate `api/types.gen.ts`, `api/sdk.gen.ts`, `api/client`, `api/core`.
3. Fix any resulting TypeScript errors in stores/components that consume the changed types.

Never hand-edit files under `web/api/` — they're overwritten on the next codegen run. `yarn codegen` downloads the spec to `web/.openapi.json` first (override the URL with `OPENAPI_URL`) so the output is the same however it's run; the E2E workflow regenerates the client against the API it starts and fails if the committed copy differs, so commit the regenerated client with the API change.

## Local dev order

Backend must be running before frontend codegen or before the frontend can make real API calls (`NUXT_PUBLIC_API_BASE` points at it). Bring up Postgres via the backend's `docker-compose.yml` first, then the API, then the frontend dev server.

## Error contract

Every API failure body is `{ "error": "<snake_case_code>" }`. The codes are defined twice, once per side, and must stay in sync: `api/Portfolio.Api/Types/ErrorCodes.cs` (with the HTTP status per code in `Extensions/ErrorStatusCodes.cs`) and `web/app/types/errors.ts` (with the user-facing message per code). Swagger doesn't describe error bodies, so codegen won't catch drift — `web/tests/types/errors-contract.spec.ts` parses `ErrorCodes.cs` and fails if the two lists differ (the Web workflow also triggers on changes to `ErrorCodes.cs`). Change both files in the same PR.

## CI/CD

- `.github/workflows/api.yml` and `web.yml` are each side's pipeline, path-filtered to its own folder: test (with a coverage summary), then the shared `e2e.yml`, then — only on push to `main` — the reusable `api-deploy.yml` / `web-deploy.yml`. A deploy can't run without the tests before it passing; don't add a trigger that bypasses that.
- `e2e.yml` builds the frontend for production and runs Playwright against the real API on a PostgreSQL service seeded from `web/e2e/fixtures/seed.sql`. A change on either side runs it.
- `codeql.yml` (C#, TypeScript, Actions), `lighthouse.yml` (live-site budgets in `web/lighthouserc.json`, after each web deploy and weekly) and `.github/dependabot.yml` (weekly, 14-day cooldown).
- Every third-party action is pinned to a full commit SHA with the version in a trailing comment. Keep it that way when adding or updating one; Dependabot updates both.
- Server-side release activation lives in `api/deploy/activate.sh` and `web/deploy/activate.sh` (health check + rollback). `docs/deploy/README.md` covers the server, including the nginx settings that aren't in the repo.

## Auth

Both sides use Auth0 — the backend validates JWT bearer tokens (`Auth0.AspNetCore.Authentication`), the frontend obtains them via `@auth0/auth0-vue`. Auth0 tenant/client config must stay in sync between `api/.env` and `web/.env` — see each project's `.env.example`.

## Docs

- `docs/adr/` — architecture decision records. Add one when a change settles a structural question (a new cross-cutting pattern, a reversed decision), not for routine features.
- `docs/deploy/README.md` — deployment, rollback and nginx.
- `LICENSE` (MIT, code only) and `NOTICE` (CV, photos, screenshots and site content are excluded).

## Code quality

`docs/CODE_QUALITY_BACKLOG.md` is a living backlog of code-quality/structural improvements, adapted from a similar initiative on another project. It records what was audited, what was already satisfied, what was fixed, and what was explicitly decided against — read it before assuming a code-quality gap here hasn't already been considered. Each project's own CLAUDE.md carries the resulting standing rules (file size caps, parameter limits, DRY, comment policy, mapping conventions, etc.) under a "Code quality standing rules" section — this is a portfolio repo on public GitHub, so these hold for new code, not just the pass that introduced them.

## Attribution

Git commits and PRs: end commit messages with `Co-Authored-By: Claude Sonnet 5 <noreply@anthropic.com>`; end PR descriptions with `🤖 Generated with [Claude Code](https://claude.com/claude-code)`.
