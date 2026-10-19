# Portfolio

This repo holds the backend and frontend of [josephpickering.co.uk](https://josephpickering.co.uk/) (not a package-manager-linked monorepo — each has its own dependencies):

| Project | Path | Stack | Docs |
|---|---|---|---|
| Backend API | [`api/`](./api) | .NET 10, PostgreSQL, EF Core | [README](./api/README.md) · [CLAUDE.md](./api/CLAUDE.md) |
| Frontend | [`web/`](./web) | Nuxt 4, Vue 3, TypeScript | [README](./web/README.md) · [CLAUDE.md](./web/CLAUDE.md) |

See each project's own README for full setup instructions. This file only covers how the two fit together.

## How they connect

The frontend's API client (`web/api/`) is **generated from the backend's Swagger spec**, not hand-written. Whenever the backend's API surface changes:

1. Run the backend locally so it's serving Swagger at `http://localhost:5105/swagger/v1/swagger.json`.
2. In `web/`, run `yarn codegen` to regenerate `api/types.gen.ts`, `api/sdk.gen.ts`, `api/client`, `api/core`.
3. Fix any resulting TypeScript errors in stores/components that consume the changed types.

Never hand-edit files under `web/api/` — they're overwritten on the next codegen run.

Both sides use Auth0 for auth — the backend validates JWT bearer tokens, the frontend obtains them via `@auth0/auth0-vue`. Auth0 tenant/client config must stay in sync between `api/.env` and `web/.env`.

## Quick start

```bash
./dev.sh                       # API + frontend together, Ctrl+C stops both
./dev.sh --docker --migrate    # also start Postgres and apply migrations first
./dev.sh --codegen             # regenerate the API client once the API is up
```

Or run each side manually:

```bash
# 1. Backend — Postgres, migrations, API
docker-compose -f "api/WorkExperienceSearch/docker-compose.yml" up -d
dotnet ef database update --project api/WorkExperienceSearch
dotnet run --project api/WorkExperienceSearch   # http://localhost:5105, Swagger at /swagger

# 2. Frontend — in a second terminal
cd web
yarn install
yarn codegen   # regenerate the API client against the running backend
yarn dev
```

Each project needs its own `.env` — see `api/.env.example` and `web/.env.example`.

## Conventions

Project-specific coding conventions (layering, patterns, testing, linting) live in each project's own `CLAUDE.md`. Cross-project conventions live in the root [`CLAUDE.md`](./CLAUDE.md).

## Code quality

[`docs/CODE_QUALITY_BACKLOG.md`](./docs/CODE_QUALITY_BACKLOG.md) tracks the active code-quality backlog for both projects — what's been audited, fixed, and deliberately left alone.
