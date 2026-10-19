# work_experience — Frontend (Nuxt 4 / Vue 3)

## Design System
Always read DESIGN.md before making any visual or UI decisions.
All font choices, colors, spacing, and aesthetic direction are defined there.
Do not deviate without explicit user approval.
In QA mode, flag any code that doesn't match DESIGN.md.

## Code Style
Code should be intuitive enough to read without comments. Default to writing no comments at all. Only add one when there's vital context a reader can't get from the code itself. Never write a comment that restates what the code already says. When editing existing code, remove comments that don't meet this bar rather than leaving them.
- No commented-out code — delete it. Git has history.
- No `console.log`/`alert` in committed code — ESLint errors on both (`no-console`, `no-alert`).

## Tech stack
Nuxt 4 (compatibility v4, `app/` source dir), Vue 3, TypeScript, Tailwind CSS v4 on the design tokens in `app/assets/css/tokens.css` (no component library — custom components live under `app/components/ui/`), Pinia for state, Auth0 (`@auth0/auth0-vue`) for auth, `@vuelidate` for form validation, TipTap for rich text, `@vite-pwa/nuxt` for PWA.

## API client — never hand-edit
`api/`, `api/client/`, `api/core/` are generated from the backend's Swagger spec via `@hey-api/openapi-ts` (`openapi-ts.config.ts`). Regenerate with `yarn codegen` (backend must be running locally at `http://localhost:5105`, see the repo root `CLAUDE.md`). These files are overwritten on every run — never edit them by hand, and don't add business logic there.

`app/plugins/axios-config.ts` sets the generated client's `baseURL` from `runtimeConfig.public.apiBase`; `app/plugins/auth0.client.ts` attaches the bearer token. The client returns `{ data, error }` and does not throw on non-2xx, so every call is wrapped in `unwrap()`.

## Async / store pattern
Stores use `asyncForm<T>()` + `tryCatchFinally` + `unwrap` (`app/utils/async-helper.ts`) to standardize `loading`/`error`/`errorCode`/`data` state per action: `tryCatchFinally(ref(this.form), async () => unwrap(await getProject(...)))`. Don't manage those as separate manual refs with try/catch/finally. When a newer call must win over an older one still in flight (route-driven loads, re-queries), keep a request counter in state and use `tryCatchFinallyIfLatest(toRef(this, 'form'), () => request === this.latestRequest, fn)` — it keeps the previous data visible while loading and drops a superseded response; don't hand-roll the copy-then-compare. Components and composables never call the generated client directly, only store actions.

API failures are `{ "error": "<snake_case_code>" }`. Codes and their user-facing copy live in one place, `app/types/errors.ts` (`ErrorCodes` + `ERROR_MESSAGES`, generic fallback for unknown codes); `extractError` in `app/utils/error-helper.ts` maps them. Branch on the code (e.g. `isNotFoundCode`), never on a string literal or the HTTP status.

Public pages load their data during SSR so crawlers and link unfurlers get real content and meta: the page (not its components) calls `useProjectsData()` / `useCompaniesData()` or its own `useAsyncData(key, fn, { lazy: true })` and then `if (import.meta.server) await …`. Don't load public-page data in `onMounted` — it only runs in the browser. Pass reactive meta to `updateMeta(() => ({ ... }))` rather than calling it after a fetch. Admin/auth-gated pages stay client-loaded.

Stores are **options-style Pinia stores** (`defineStore(name, { state, getters, actions })`), not setup stores — match this when adding a new store, don't introduce the other style.

## Directory layout
- `app/components/<domain>/` — one folder per domain (`company`, `project`, `tag`, `feedback`, `search`, `repository`, `mockup`, `layout`, `home`), often split further by responsibility (`filter/`, `form/`, `list/`, `timeline/`, `details/`). Project stat/repository inputs live in `project/forms/` and `repository/`. Generic primitives live in `app/components/ui/` (`button`, `filter`, `form`, `input`, `layout`) — put a new component there only if it's genuinely domain-agnostic.
- `app/composables/use*.ts` — flat, one file per concern.
- `app/store/*Store.ts` — flat, one Pinia store per domain.
- `app/pages/` — file-based routing; CMS-style resources follow a `new.vue` / `update.vue` convention alongside the resource's other pages.
- `app/types/` — flat, one file per concept.
- `app/data/` — static lookup data (e.g. `TagTypes.ts`).
- `app/utils/*-helper.ts` — flat, pure-function helper modules (array, async, company, date, default, entity-form, error, form-data, image, markdown, meta, project, string, tab, tag, upload, validation). No Vue reactivity, Nuxt runtime calls or API calls in utils. All date/string/number formatting goes through these.
- `app/middleware/auth.ts`, `app/plugins/`, `app/layouts/default.vue`.
- `server/api/`, `server/routes/` — Nitro server routes (sitemap generation, service worker) — for backend-proper logic use the .NET API, not Nitro.

## Component conventions
- `<script setup lang="ts">` always.
- Props via a named `interface Props` + `defineProps<Props>()` / `withDefaults`.
- Emits typed with `defineEmits<{...}>()`.
- Auto-imports are off (`imports: { autoImport: false }` in `nuxt.config.ts`): import every Vue symbol from `vue`, router composables from `vue-router`, and Nuxt composables/macros (`useAsyncData`, `navigateTo`, `definePageMeta`, `useHead`, `useRuntimeConfig`, …) from `#imports`. `vue-tsc` fails on a missing import. Components are still auto-registered.
- `<script setup>` order: imports, interfaces/types, props, model, emits, constants, stores/composables, refs, computed, functions, lifecycle hooks, watchers, `defineExpose`.
- No inline styles; the only `:style` allowed binds a CSS custom property consumed by a Tailwind class (`w-(--thumb-width)`). Raw CSS in `<style>` only for third-party DOM (TipTap in `TextEditor.vue`).
- `<NuxtImg>`, never `<img>`.
- ESLint enforces: no blank line between import statements, one blank line after the import block, PascalCase component tags in templates.

## Testing
Vitest for unit tests (`tests/**/*.spec.ts`, mirroring `app/`; `#imports` is aliased to `tests/shims/nuxt-imports.ts`, so stub what a test uses with `vi.stubGlobal`; config in `vitest.config.ts`, run with `yarn test` / `yarn test:watch`). Every util and store action has a spec; store specs mock `@api` and assert AsyncForm transitions including the `unwrap` error path.

Playwright for E2E (`e2e/*.spec.ts`, config in `playwright.config.ts`, run with `yarn test:e2e`). Select elements by `data-testid`, never CSS classes or DOM structure. AI search specs mock `**/vertex/query` with `page.route`; the rest need the backend running. Playwright's `webServer` starts `yarn dev` automatically — no need to have the dev server running first, though `E2E_BASE_URL` can point it at an already-running instance. E2E is capped to a single worker (`workers: 1`) because Nuxt's dev-mode SSR serializes under concurrent load and produces flaky timeouts otherwise; if this ever moves to testing a production build, revisit that cap.

Public pages are server-rendered, so markup is visible before Vue attaches event handlers — tests that click or type must navigate with `gotoHydrated` (`e2e/support/navigation.ts`, which waits for the `data-hydrated` mark set by `app/plugins/hydration-marker.client.ts`), not bare `page.goto`.

Auth-gated flows (anything behind `definePageMeta({ middleware: 'auth' })` — the CRUD `new`/`update` pages) redirect to a real Auth0 login with no bypass in this codebase, so they aren't covered by the current E2E suite — only that the redirect itself happens. Covering the authenticated flows would need a test Auth0 tenant/user; treat that as a follow-up, not a gap to silently ignore.

New code should come with tests, not just fixes for the code covered by this backlog pass.

## Code quality standing rules

Established by an audit-driven cleanup pass (see `docs/CODE_QUALITY_BACKLOG.md` at the repo root):

- **File size cap ~500-600 lines.** Split a large component into sub-components, or a large store/composable by responsibility, before it grows past this. No automated check yet — a code-review convention.
- **No non-null assertions (`!`).** Capture the value into a local `const` first so TypeScript can narrow it properly (a direct `if (x)` on a prop/computed doesn't reliably narrow across statements or closures, which is why the assertion tends to get reached for in the first place), or restructure so the assertion isn't needed — see the pattern used throughout `CompanyForm.vue`/`ProjectForm.vue`/`TagForm.vue`. Enforced by `@typescript-eslint/no-non-null-assertion` (and `no-explicit-any`) as ESLint errors.
- **Runtime-only packages go in `dependencies`; build/tooling-only packages go in `devDependencies`.** Before adding a package, check whether it's imported by anything under `app/` or `server/` at runtime — if not, it belongs in `devDependencies` even if it's also pinned in `resolutions` for a transitive-version fix.
- **No magic values.** A literal with business meaning (a status string, a numeric limit) should be a named constant or enum member, not a bare literal at the call site.

## Linting / formatting
ESLint (`eslint.config.mjs`): `@nuxt/eslint` flat config (which brings the Vue and TypeScript plugins) + `@stylistic` and `unused-imports`. Prettier (`.prettierrc.json`: printWidth 125, single quotes, no semicolons, `prettier-plugin-tailwindcss` for class sorting) is editor formatting only — ESLint doesn't run it, so it isn't checked in CI or pre-commit. Husky + lint-staged run `yarn lint:commit` on staged `*.{js,vue,ts}` files pre-commit — don't bypass with `--no-verify`.

Scripts: `yarn lint`, `yarn lint:full`, `yarn typecheck` (`vue-tsc --noEmit`), `yarn codegen`.

## Env
`.env.example` lists `NUXT_PUBLIC_API_BASE`, `NUXT_PUBLIC_BASE`, `NUXT_AUTH0_CLIENT_ID`/`SECRET`/`DOMAIN`, `NUXT_PUBLIC_LINKED_IN_URL`, `NUXT_PUBLIC_UMAMI_ID`. Never commit `.env`.

## Docs
`README.md` has the fuller getting-started guide, tech stack table, and deployment notes (PM2 / DigitalOcean on push to `main`) — read it alongside this file.
