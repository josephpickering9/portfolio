# work_experience — Frontend (Nuxt 4 / Vue 3)

## Code Style
Code should be intuitive enough to read without comments. Default to writing no comments at all. Only add one when there's vital context a reader can't get from the code itself. Never write a comment that restates what the code already says. When editing existing code, remove comments that don't meet this bar rather than leaving them.
- No commented-out code — delete it. Git has history.
- No `console.log`/`alert` in committed code — ESLint errors on both (`no-console`, `no-alert`).

## Tech stack
Nuxt 4 (compatibility v4, `app/` source dir), Vue 3, TypeScript, Tailwind CSS v4 + daisyUI (no separate component library — custom components live under `app/components/ui/`), Pinia for state, Auth0 (`@auth0/auth0-vue`) for auth, `@vuelidate` for form validation, TipTap for rich text, `@vite-pwa/nuxt` for PWA.

## API client — never hand-edit
`api/`, `api/client/`, `api/core/` are generated from the backend's Swagger spec via `@hey-api/openapi-ts` (`openapi-ts.config.ts`). Regenerate with `yarn codegen` (backend must be running locally at `http://localhost:5105`, see the monorepo root `CLAUDE.md`). These files are overwritten on every run — never edit them by hand, and don't add business logic there.

`app/plugins/axios-config.ts` sets the generated client's `baseURL` from `runtimeConfig.public.apiBase`; `app/plugins/auth0.client.ts` attaches the bearer token.

## Async / store pattern
Stores use the `asyncForm<T>()` + `tryCatchFinally` helper pair (`app/utils/async-helper.ts`) to standardize `loading`/`error`/`data` state per action — don't manage those as separate manual refs with try/catch/finally. Stores call the generated API client functions directly (`getProject`, `postProject`, imported `from '@api'`); components and composables never call the generated client directly, only store actions.

Stores are **options-style Pinia stores** (`defineStore(name, { state, getters, actions })`), not setup stores — match this when adding a new store, don't introduce the other style.

## Directory layout
- `app/components/<domain>/` — one folder per domain (`company`, `project`, `tag`, `feedback`, `search`, `repository`, `mockup`, `layout`, `home`), often split further by responsibility (`filter/`, `form/`, `list/`, `timeline/`, `details/`). Generic primitives live in `app/components/ui/` (`button`, `filter`, `form`, `input`, `layout`, `table`, `tooltip`) — put a new component there only if it's genuinely domain-agnostic.
- `app/composables/use*.ts` — flat, one file per concern.
- `app/store/*Store.ts` — flat, one Pinia store per domain.
- `app/pages/` — file-based routing; CMS-style resources follow a `new.vue` / `update.vue` convention alongside the resource's other pages.
- `app/types/` — flat, one file per concept.
- `app/data/` — static lookup data (e.g. `TagTypes.ts`).
- `app/utils/*-helper.ts` — flat, pure-function helper modules (array, async, date, default, enum, error, form-data, image, string). No Vue reactivity or API calls in utils.
- `app/middleware/auth.ts`, `app/plugins/`, `app/layouts/default.vue`.
- `server/api/`, `server/routes/` — Nitro server routes (sitemap generation, service worker) — for backend-proper logic use the .NET API, not Nitro.

## Component conventions
- `<script setup lang="ts">` always.
- Props via a named `interface Props` + `defineProps<Props>()` / `withDefaults`.
- Emits typed with `defineEmits<{...}>()`.
- Prefer explicit imports for composables, stores, and types (`import type { Project } from '@api'`) even where Nuxt auto-import would work — this codebase leans explicit outside of components/pages.
- ESLint enforces: no blank line between import statements, one blank line after the import block, PascalCase component tags in templates.

## Testing
Vitest for unit tests (`app/**/*.test.ts`, config in `vitest.config.ts`, run with `yarn test` / `yarn test:watch`) and Playwright for E2E (`e2e/*.spec.ts`, config in `playwright.config.ts`, run with `yarn test:e2e`). Playwright's `webServer` starts `yarn dev` automatically — no need to have the dev server running first, though `E2E_BASE_URL` can point it at an already-running instance. E2E is capped to a single worker (`workers: 1`) because Nuxt's dev-mode SSR serializes under concurrent load and produces flaky timeouts otherwise; if this ever moves to testing a production build, revisit that cap.

Auth-gated flows (anything behind `definePageMeta({ middleware: 'auth' })` — the CRUD `new`/`update` pages) redirect to a real Auth0 login with no bypass in this codebase, so they aren't covered by the current E2E suite — only that the redirect itself happens. Covering the authenticated flows would need a test Auth0 tenant/user; treat that as a follow-up, not a gap to silently ignore.

New code should come with tests, not just fixes for the code covered by this backlog pass.

## Code quality standing rules

Established by an audit-driven cleanup pass (see `docs/CODE_QUALITY_BACKLOG.md` at the monorepo root):

- **File size cap ~500-600 lines.** Split a large component into sub-components, or a large store/composable by responsibility, before it grows past this. No automated check yet — a code-review convention.
- **No non-null assertions (`!`).** Capture the value into a local `const` first so TypeScript can narrow it properly (a direct `if (x)` on a prop/computed doesn't reliably narrow across statements or closures, which is why the assertion tends to get reached for in the first place), or restructure so the assertion isn't needed — see the pattern used throughout `CompanyForm.vue`/`ProjectForm.vue`/`TagForm.vue`. Not automated (no `no-non-null-assertion` ESLint rule configured); catch it in review.
- **Runtime-only packages go in `dependencies`; build/tooling-only packages go in `devDependencies`.** Before adding a package, check whether it's imported by anything under `app/` or `server/` at runtime — if not, it belongs in `devDependencies` even if it's also pinned in `resolutions` for a transitive-version fix.
- **No magic values.** A literal with business meaning (a status string, a numeric limit) should be a named constant or enum member, not a bare literal at the call site.

## Linting / formatting
ESLint (`eslint.config.mjs`): `@nuxt/eslint` flat config + `@stylistic`, `eslint-plugin-vue`, `unused-imports`, Prettier integration. Prettier (`.prettierrc.json`): printWidth 125, single quotes, no semicolons, `prettier-plugin-tailwindcss` for class sorting. Husky + lint-staged run `yarn lint:commit` on staged `*.{js,vue,ts}` files pre-commit — don't bypass with `--no-verify`.

Scripts: `yarn lint`, `yarn lint:full`, `yarn typecheck` (`vue-tsc --noEmit`), `yarn codegen`.

## Env
`.env.example` lists `NUXT_PUBLIC_API_BASE`, `NUXT_PUBLIC_BASE`, `NUXT_AUTH0_CLIENT_ID`/`SECRET`/`DOMAIN`, `NUXT_PUBLIC_LINKED_IN_URL`, `NUXT_PUBLIC_UMAMI_ID`. Never commit `.env`.

## Docs
`README.md` has the fuller getting-started guide, tech stack table, and deployment notes (PM2 / DigitalOcean on push to `main`) — read it alongside this file.
