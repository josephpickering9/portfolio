# Portfolio — Monorepo

Two projects in one git repo, not a package-manager-linked monorepo:

- **`api/`** — .NET 8 Web API + PostgreSQL backend. See `api/CLAUDE.md`.
- **`web/`** — Nuxt 4 / Vue 3 frontend. See `web/CLAUDE.md`.

Each has its own dependency graph, CI workflows (`.github/workflows/api-*` / `web-*`, path-filtered so a change only builds and deploys its own side), and CLAUDE.md with project-specific conventions. This file only covers things that span both.

## How they connect

The frontend's API client (`web/api/`) is **generated from the backend's Swagger spec**, not hand-written. Workflow when the API surface changes:

1. Run the backend locally (`dotnet run --project api/WorkExperienceSearch`, serves Swagger at `http://localhost:5105/swagger/v1/swagger.json`).
2. In `web/`, run `yarn codegen` to regenerate `api/types.gen.ts`, `api/sdk.gen.ts`, `api/client`, `api/core`.
3. Fix any resulting TypeScript errors in stores/components that consume the changed types.

Never hand-edit files under `web/api/` — they're overwritten on the next codegen run.

## Local dev order

Backend must be running before frontend codegen or before the frontend can make real API calls (`NUXT_PUBLIC_API_BASE` points at it). Bring up Postgres via the backend's `docker-compose.yml` first, then the API, then the frontend dev server.

## Auth

Both sides use Auth0 — the backend validates JWT bearer tokens (`Auth0.AspNetCore.Authentication`), the frontend obtains them via `@auth0/auth0-vue`. Auth0 tenant/client config must stay in sync between `api/.env` and `web/.env` — see each project's `.env.example`.

## Code quality

`docs/CODE_QUALITY_BACKLOG.md` is a living backlog of code-quality/structural improvements, adapted from a similar initiative on another project. It records what was audited, what was already satisfied, what was fixed, and what was explicitly decided against — read it before assuming a code-quality gap here hasn't already been considered. Each project's own CLAUDE.md carries the resulting standing rules (file size caps, parameter limits, DRY, comment policy, mapping conventions, etc.) under a "Code quality standing rules" section — this is a portfolio repo on public GitHub, so these hold for new code, not just the pass that introduced them.

## Attribution

Git commits and PRs: end commit messages with `Co-Authored-By: Claude Sonnet 5 <noreply@anthropic.com>`; end PR descriptions with `🤖 Generated with [Claude Code](https://claude.com/claude-code)`.
