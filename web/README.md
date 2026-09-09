# Work Experience (Frontend)

A Nuxt 4 portfolio application showcasing my professional projects and the technologies used across them. Originally built as a personal tool to search projects by technology, it has evolved into a full portfolio with AI-powered search, company timelines, and a CMS for managing content.

Linked to the [Work Experience API](https://github.com/josephpickering9/work_experience_api).

**Live:** [experience.josephpickering.co.uk](https://experience.josephpickering.co.uk)

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Environment Variables](#environment-variables)
- [Authentication](#authentication)
- [OpenAPI Codegen](#openapi-codegen)
- [Testing](#testing)
- [Linting](#linting)
- [Deployment](#deployment)

## Features

- **Server-Side Rendering** via Nuxt 4 for fast load times and SEO
- **AI Search** powered by Vertex AI — query projects in natural language
- **Company Timelines** — visual history of roles and projects per company
- **CMS** — authenticated users can create, update, and delete projects, companies, and technologies
- **Filtering & Sorting** — filter projects by company, technology type, or tag with URL-persisted state
- **Progressive Web App** — installable with offline support via `@vite-pwa/nuxt`
- **Auth0 Authentication** — secure login with token-based API access
- **OpenAPI TypeScript Generation** — types and API client auto-generated from the backend Swagger spec
- **Theme Switching** — multiple daisyUI themes with persisted preference

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | Nuxt 4, Vue 3, TypeScript |
| Styling | Tailwind CSS v4, daisyUI |
| State | Pinia (with persisted state) |
| Auth | Auth0 (`@auth0/auth0-vue`) |
| API Client | `@hey-api/client-axios` (OpenAPI generated) |
| Forms | Vuelidate |
| Rich Text | Tiptap |
| Drag & Drop | vuedraggable |
| PWA | `@vite-pwa/nuxt` |
| Testing | Vitest (unit), Playwright (E2E) |
| Linting | ESLint, Prettier, Husky + lint-staged |

## Project Structure

```
app/
├── components/     # UI components organised by feature (project/, company/, tag/, ui/, search/)
├── composables/    # Shared logic (useAuth, useEntityForm, useProjectFilters, useValidation)
├── pages/          # File-based routing
├── store/          # Pinia stores — all API calls go through here
├── types/          # TypeScript type definitions
├── utils/          # Pure utility functions (async-helper, form-data, image-helper, etc.)
└── data/           # Static data (tag types)

api/                # Auto-generated OpenAPI client (do not edit manually)
e2e/                # Playwright E2E specs
```

## Getting Started

**Prerequisites:** Node.js (LTS), Yarn, a running instance of the [Work Experience API](https://github.com/josephpickering9/work_experience_api), and an Auth0 application.

```bash
# Clone and install
git clone https://github.com/josephpickering9/work_experience.git
cd work_experience
yarn install

# Configure environment
cp .env.example .env
# Fill in the values — see Environment Variables below

# With the backend running locally (see its README), generate the API client
yarn codegen

# Start dev server
yarn dev
```

## Environment Variables

| Variable | Description |
|---|---|
| `NUXT_PUBLIC_API_BASE` | Base URL of the Work Experience API (e.g. `http://localhost:5105` locally) |
| `NUXT_PUBLIC_BASE` | Base URL of this frontend (used for PWA and redirects, e.g. `http://localhost:3000` locally) |
| `NUXT_AUTH0_CLIENT_ID` | Auth0 application client ID |
| `NUXT_AUTH0_CLIENT_SECRET` | Auth0 application client secret |
| `NUXT_AUTH0_DOMAIN` | Auth0 domain (e.g. `your-tenant.auth0.com`) |
| `NUXT_PUBLIC_LINKED_IN_URL` | LinkedIn profile URL displayed in the header |
| `NUXT_PUBLIC_UMAMI_ID` | Umami analytics website ID (optional — leave blank to disable analytics) |

## Authentication

This project uses Auth0. To set up:

1. Create an application (Single Page Application) in the [Auth0 dashboard](https://manage.auth0.com).
2. Create an API in Auth0 and set its identifier as the `audience` — this should match `NUXT_PUBLIC_API_BASE`.
3. Add your local and production URLs to the **Allowed Callback URLs**, **Allowed Logout URLs**, and **Allowed Web Origins** in the Auth0 application settings.
4. Populate `NUXT_AUTH0_CLIENT_ID`, `NUXT_AUTH0_CLIENT_SECRET`, and `NUXT_AUTH0_DOMAIN` in `.env`.

Authentication is enforced per-page via `definePageMeta({ middleware: 'auth' })`. The plugin at `app/plugins/auth0.client.ts` attaches the bearer token to all API requests.

## OpenAPI Codegen

API types and the HTTP client are auto-generated from the backend's Swagger spec. The generated files live in `api/` and should not be edited manually — they're overwritten on the next run.

```bash
# With the backend running locally at http://localhost:5105, regenerate the client
yarn codegen
```

Fix any resulting TypeScript errors in stores/components that consume the changed types.

## Testing

Vitest for unit tests, Playwright for E2E:

```bash
yarn test          # Unit tests (app/**/*.test.ts)
yarn test:watch    # Unit tests in watch mode
yarn test:e2e      # E2E tests (e2e/*.spec.ts) — starts its own dev server automatically
```

E2E coverage is currently the public browsing flows (project search/filter, public page navigation) — the authenticated CMS flows (create/update/delete) sit behind a real Auth0 login with no bypass in this codebase, so they aren't covered yet. See `CLAUDE.md`'s Testing section for details.

## Linting

ESLint and Prettier are enforced via Husky pre-commit hooks.

```bash
yarn lint          # Lint staged files
yarn lint:full     # Lint all files
yarn lint --fix    # Lint and auto-fix
```

## Deployment

The project deploys automatically to a Digital Ocean droplet via GitHub Actions on push to `main`. The server runs the app with PM2 (`ecosystem.config.js`).

## Conventions

Coding conventions (directory layout, component patterns, testing, linting) live in [`CLAUDE.md`](./CLAUDE.md). The monorepo root's [`docs/CODE_QUALITY_BACKLOG.md`](../docs/CODE_QUALITY_BACKLOG.md) tracks the active code-quality backlog for both projects.
