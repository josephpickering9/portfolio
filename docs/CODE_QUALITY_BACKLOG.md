# Code Quality Backlog

Living backlog of code-quality/structural improvements, adapted from the equivalent
initiative on the Pulse project (PUL-126/PUL-101 follow-up). This is a portfolio repo
on public GitHub — code quality is part of what it's demonstrating, so this backlog
gets worked in parallel with feature work, not deferred to "closer to launch."

## Audit summary (2026-09-09)

Ran a full audit against the Pulse ticket's 14-item checklist before planning any
work. Result: ~10/14 items were already satisfied by existing conventions on both
projects. Real gaps below are the actual scope of this backlog — not a wholesale
re-application of the Pulse checklist.

### Already satisfied — no action needed
- Feature-organized folders on both sides (`Controllers/Services/Repositories` by
  domain on the backend; `components/` split by domain, no root-level `.vue` files
  on the frontend)
- No file over 400 lines anywhere in either project
- Controller → Service → Repository layering enforced by an automated NetArchTest
  suite (`Portfolio.Api.Tests/Architecture/ArchitectureTests.cs`) — zero
  violations found
- `Result<T>` pattern in use for fallible operations; strongly-typed IDs
  (`ProjectId`, `CompanyId`, `TagId`, etc.) used consistently, zero raw `Guid`
  PK/FK stragglers
- Zero `FromSqlRaw` in repositories — LINQ throughout
- No long Vue prop lists — components already use the documented typed-object
  prop pattern
- All frontend API calls already routed through Pinia stores only (verified: zero
  direct API-client imports in `components/` or `composables/`)
- Zero `console.log`; comment policy (no comments except vital ones) already
  documented in both CLAUDE.md files and followed in spot checks
- No `docs/engineering/`, `docs/design/`, `docs/product/` trees exist in this repo
  (only 3 READMEs + 3 CLAUDE.md total) — that part of the Pulse ticket doesn't map
  to this repo's actual doc surface

### Real gaps (this backlog's scope)
1. **Zero frontend test coverage** — no Vitest/Playwright at all. Backend already
   has unit + integration + architecture tests.
2. **`AsNoTracking()` missing from all 8 backend repository read methods.**
3. **12 non-null assertions (`!`)** in frontend components/stores, unaudited.
4. Minor housekeeping: 1 inline DTO construction (`VertexProjectDescriptionService.cs`)
   instead of a `ToDto()` method; 1 leftover `TODO` in `AiSearch.vue`; several
   build-tool packages (`rollup`, `ts-node`, `tar`, `js-yaml`, etc.) misplaced in
   `dependencies` instead of `devDependencies`.
5. **Structured logging is limited to 3 VertexAi service files** — the rest of
   the backend (Controllers, other Services, Repositories) has little to no
   logging. Decided to expand now rather than defer: add structured logging
   (named properties, entity IDs included, per the existing VertexAi convention)
   to the other Services and Controllers, prioritizing failure paths (Result<T>
   failures, caught exceptions) over happy-path noise.
6. Rules from the Pulse ticket that *do* apply here aren't yet codified as
   standing rules in CLAUDE.md — they're followed today by convention, which is
   exactly the drift risk the Pulse ticket calls out.

### Explicitly decided NOT to do (ticket items considered and rejected for this repo)
- ~~**Centralized `ErrorCodes.cs` constants file**~~ — *reversed in the 2026-10-19
  audit below.* Original reasoning: backend already models errors
  via a typed `ErrorType` enum + free-text messages. This satisfies the ticket's
  actual intent (avoid scattered magic error strings) without adding a redundant
  second layer of error identity. Revisit only if free-text messages start
  getting compared/matched on by callers (a real "magic string" smell) — that
  hasn't happened.
- **Full re-audit of all 14 items from scratch** — the two-agent audit already
  covers this with concrete evidence (file:line counts, grep results). Re-running
  it wholesale would mostly re-confirm the same findings for no new fixes.

## What already exists (reused, not rebuilt)
- Backend test harness (`Portfolio.Api.Tests`) — the frontend test setup in
  Phase 1 should mirror its structure (unit/integration split) where it makes
  sense for Vitest/Playwright, not invent a new convention.
- `app/utils/async-helper.ts` (`asyncForm<T>` + `tryCatchFinally`) — already the
  DRY'd fetch/loading/error pattern; Phase 1 tests should target this directly
  since it's shared across every store.
- `Types/Result.cs` / `Utils/ResultExtensions.cs` — the existing mapping/response
  convention; the one inline-DTO fix in Phase 2 should follow this same
  `.ToResponse()`-adjacent style, not introduce a new mapper convention.

## Sequencing

Reordered from the initial draft after outside-voice review: test coverage lands
**before** the behavior-changing fixes it would otherwise have no safety net for.

```
Phase 1: Frontend test scaffolding          [independent, backend untouched]
  ├── Vitest config + unit tests for app/utils/, app/composables/,
  │   store logic (asyncForm, error-helper) — highest-value, most reused code first
  └── Playwright config + E2E for critical flows (project search/filter,
      company/project CRUD forms)
        │
        ▼  (safety net now exists for frontend behavior changes)
Phase 2: Verified mechanical fixes          [backend + frontend, can run in parallel]
  ├── Backend: verify no AsNoTracking-bound repo method is reused on a
  │   write path (grep call sites for SaveChanges/Update reuse) → apply
  │   AsNoTracking() to all 8 repositories
  ├── Backend: ToDto() method on VertexProjectDescriptionService.cs's entity
  ├── Frontend: audit + fix all 12 non-null assertions individually,
  │   run against Phase 1's new test suite to catch regressions
  ├── Frontend: resolve TODO in AiSearch.vue
  ├── Frontend: verify deploy pipeline installs with/without --production
  │   flag → move build-only packages to devDependencies accordingly
  └── Backend: expand structured logging into other Services/Controllers,
      focused on failure paths (Result<T> failures, caught exceptions),
      following the existing VertexAi named-property convention
        │
        ▼
Phase 3: Codify rules in CLAUDE.md          [docs only, do last — codify what's true]
  ├── Root CLAUDE.md: note this backlog's existence and audit methodology
  ├── Backend CLAUDE.md: AsNoTracking rule, ToDto() convention,
  │   layering (already enforced — document the NetArchTest as the mechanism)
  ├── Frontend CLAUDE.md: no stray non-null assertions rule, test coverage
  │   expectations (link to Phase 1 setup), dependency/devDependency convention
  └── Verify each new rule actually has an enforcement mechanism (lint rule,
      CI check, or architecture test) — a documented rule with nothing checking
      it is exactly the drift the Pulse ticket exists to prevent
```

**Why this order:** Phase 1 is the largest single item and the most
externally-visible (a portfolio reviewer sees test coverage, not CLAUDE.md).
Phase 2's non-null-assertion removals are behavior-changing edits on the
null path — they get Phase 1's safety net before landing. Phase 3 documents
conventions that are now actually true, including the ones Phase 2 just
established, rather than aspirationally documenting rules with no enforcement.

## Parallelization

Phase 1's two tracks (Vitest unit tests, Playwright E2E) are independent of each
other and of the backend — safe to run in parallel worktrees.

| Step | Modules touched | Depends on |
|------|-----------------|------------|
| Vitest setup + unit tests | `web/app/{utils,composables,store}` | — |
| Playwright setup + E2E | `web/{playwright config, e2e/}` | — |
| AsNoTracking + verification | `api/*/Repositories` | — |
| Non-null-assertion audit | `web/app/components`, `app/store` | Vitest setup (needs the safety net) |
| Housekeeping (ToDto, TODO, deps) | 1 backend service, 1 component, `package.json` | — |
| Structured logging expansion | `api/**/Services`, `Controllers` | — |
| CLAUDE.md codification | 3× `CLAUDE.md` | All above (documents what's now true) |

```
Lane A: Vitest unit tests → non-null-assertion audit (sequential, shared frontend scope)
Lane B: Playwright E2E (independent)
Lane C: AsNoTracking verification + apply (independent, backend-only)
Lane D: Housekeeping items (independent, touches 1 backend file + 1 frontend file + package.json)
Lane F: Structured logging expansion (independent, backend Services/Controllers —
        flag if it touches the same files as Lane C's AsNoTracking changes)
Lane E: CLAUDE.md codification (waits on A, B, C, D, F)
```

Launch A, B, C, D, F in parallel. Note: Lane C and Lane F both touch backend
Services/Repositories — check for file-level overlap before running fully
parallel; if a file is touched by both, sequence those two edits or coordinate
via a shared branch. Merge all. Then E.

## Test coverage plan (Phase 1 detail)

```
FRONTEND CODE PATHS                                    USER FLOWS
[+] app/utils/async-helper.ts                          [+] Project search & filter
  └── asyncForm<T>() / tryCatchFinally()                 ├── [GAP][→E2E] Apply filter, see results update
      ├── [GAP] success path                             ├── [GAP][→E2E] Empty results state
      ├── [GAP] error path (rejected promise)             └── [GAP][→E2E] Clear filters
      └── [GAP] finally always runs
[+] app/utils/error-helper.ts                          [+] Company/Project CRUD forms
  └── extractError(error: any)                            ├── [GAP][→E2E] Create project, validation errors shown
      ├── [GAP] structured API error shape                ├── [GAP][→E2E] Edit existing project, save succeeds
      ├── [GAP] plain Error                                ├── [GAP][→E2E] Non-null-assertion sites: e.g. props.id
      └── [GAP] unknown/non-Error thrown                   │   undefined on direct-link navigation (edge case
[+] store/{Company,Project,Tag}Store.ts                    │   the assertion currently masks)
  └── uses asyncForm — [GAP] each store's success/error   └── [GAP][→E2E] Network failure mid-submit

COVERAGE: 0/13 paths tested (0%) — greenfield, this phase establishes the baseline
QUALITY: none yet
GAPS: 13 (5 E2E, 8 unit)
```

Each GAP becomes a concrete test in Phase 1 — see Implementation Tasks below.

## Failure modes

| Codepath | Realistic failure | Test? | Error handling? | User sees |
|---|---|---|---|---|
| AsNoTracking applied to a method secretly reused on a write path | Update silently no-ops (EF doesn't detect the change) | No (until Phase 2 verification step runs) | No | **Silent** — data doesn't save, no error shown. **Critical gap if verification step is skipped.** |
| Non-null assertion removed, guard added incorrectly | Guard rejects a legitimately-present value | Yes, after Phase 1 | Depends on new guard | Should surface a clear validation/empty state, not silent |
| `--production` install breaks on moved devDependency | Build fails in CI/deploy | No dedicated test | Deploy pipeline fails loudly | Deploy fails visibly — not silent, but blocks shipping until caught |
| Playwright E2E flakiness on CI | Intermittent false failures | N/A (test infra itself) | N/A | Developer sees a red CI run that isn't a real regression |

The AsNoTracking row is the one critical gap with no test and a silent failure
mode — this is why the verification step (grep for write-path reuse before
applying) is mandatory, not optional, in Phase 2.

## NOT in scope
- ~~Centralized `ErrorCodes.cs`~~ — done in the 2026-10-19 audit.
- Any `docs/engineering/`, `docs/design/`, `docs/product/` documentation work from
  the original Pulse ticket — those trees don't exist in this repo.
- Re-auditing the ~10 already-satisfied checklist items — covered with evidence
  in the audit summary above.

## Implementation Tasks

- [x] **T1 (P1, human: ~1 day / CC: ~2h)** — frontend — Set up Vitest, write unit
  tests for `asyncForm`/`tryCatchFinally` and `extractError` covering
  success/error/edge paths
  - Surfaced by: Test Review — 0% frontend coverage
  - Files: `web/vitest.config.ts`, `app/utils/async-helper.test.ts`,
    `app/utils/error-helper.test.ts`
  - Verified: `yarn test` — 18/18 passing
- [x] **T2 (P1, human: ~1-2 days / CC: ~2-3h)** — frontend — Set up Playwright,
  write E2E for the public browsing flows
  - Surfaced by: Test Review — critical user flows untested
  - Files: `web/playwright.config.ts`, `e2e/project-search.spec.ts`,
    `e2e/public-navigation.spec.ts`
  - Verified: `yarn test:e2e` — 9/9 passing
  - **Scope deviation:** company/project CRUD forms are behind
    `definePageMeta({ middleware: 'auth' })`, which redirects to a real Auth0
    login with no bypass in this codebase. Covering those flows would need a
    test Auth0 tenant/user, which wasn't available — out of scope for this
    pass rather than faked. E2E instead covers project search/filter (live
    filtering, empty state, URL query sync), public navigation across all
    public pages (console-error-free), and confirms the auth redirect itself
    fires. Noted in `web/CLAUDE.md`'s Testing section as a real,
    documented gap, not a silently dropped requirement.
  - **Config deviation:** capped to `workers: 1` — Nuxt's dev-mode SSR
    serializes under concurrent requests, causing flaky 30s timeouts with
    parallel workers. Documented in `playwright.config.ts` and this project's
    CLAUDE.md.
- [x] **T3 (P1, human: ~1h / CC: ~15min)** — backend — Verify no AsNoTracking
  candidate repo method is reused on a write path, then apply `AsNoTracking()`
  to all safe repository read methods
  - Surfaced by: Code Quality Review — 0/8 repos use AsNoTracking
  - Files: `ProjectRepository.cs`, `CompanyRepository.cs`, `TagRepository.cs`
  - Verified: `dotnet test` — 81 passed, 1 pre-existing skip (real Postgres)
  - **Scope deviation, exactly the risk flagged in review:** 4 methods were
    *not* given `AsNoTracking()` because they're reused on write paths —
    verified empirically, not just by reading call sites. `ProjectRepository
    .GetAsync(ProjectId)` and `ProjectImageRepository.GetUnoptimisedImagesAsync
    ()` were initially changed, which broke 4 tests with `InvalidOperationException:
    another instance with the same key is already being tracked` — reverted.
    `CompanyRepository.GetAsync(CompanyId)`, `TagRepository.GetAsync(TagId)`,
    `TagRepository.GetByTitleAsync`, and `ProjectRepository.GetForUpdateAsync`
    were left untouched for the same class of reason (mutated then saved
    without an explicit `Update()`/`Entry().State` call). This is the
    documented exception case in the backend CLAUDE.md's AsNoTracking rule.
- [x] **T4 (P2, human: ~2-3h / CC: ~30min)** — frontend — Audit and fix all 12
  non-null assertions individually
  - Surfaced by: Code Quality Review
  - Files: `AutoComplete.vue`, `FilterBarDesktopDropdown.vue`, `Carousel.vue`,
    `ProjectForm.vue`, `TagForm.vue`, `TagFilterList.vue`, `CompanyAutoComplete.vue`,
    `CompanyForm.vue` (×3), `CompanyFilterList.vue`, `AiStore.ts`
  - Verified: 0 non-null assertions remain (grep-confirmed); `yarn typecheck`
    introduces no new errors vs. baseline (confirmed via `git stash` diff);
    `yarn test` and lint both clean
- [x] **T5 (P3, human: ~30min / CC: ~10min)** — housekeeping — ToDto()-style
  mapping method on VertexProjectDescriptionService.cs, resolve AiSearch.vue
  TODO, verify deploy install flags then move build-only packages to
  devDependencies
  - Files: `Services/VertexAi/VertexProjectDescriptionService.cs`,
    `app/components/search/AiSearch.vue`, `web/package.json`
  - Verified: deploy workflow (`.github/workflows/web-deploy.yml`)
    uses `yarn install --frozen-lockfile` with no `--production` flag, so
    devDependencies are always installed — safe to move `rollup`, `tar`,
    `node-forge`, `serialize-javascript`, `svgo`, `minimatch`, `js-yaml`,
    `ts-node` (confirmed zero runtime imports of any of them); `lodash-es` and
    `markdown-it` stayed in `dependencies` since they're actually imported.
    `dotnet build`, `yarn install`, `yarn test` all green after the move.
- [x] **T6 (P2, human: ~3-4h / CC: ~45min)** — backend — Expand structured
  logging into other Services, prioritizing failure paths, using the existing
  VertexAi named-property convention
  - Files: `ProjectService.cs`, `CompanyService.cs`, `TagService.cs`,
    `FileService.cs`, `ImageService.cs`, `ProjectImageService.cs`,
    `ProjectRepositoryService.cs`
  - Verified: `dotnet test` — 81 passed (same run as T3)
  - **Scope deviation:** Controllers were deliberately left untouched — this
    repo's own CLAUDE.md documents Controllers as "thin... no business logic"
    that never branches on `Result`, so adding branching log calls there would
    contradict that existing convention rather than extend it.
- [x] **T7 (P2, human: ~2-3h / CC: ~30min)** — docs — Codify applicable Pulse
  rules into all 3 CLAUDE.md files
  - Files: `CLAUDE.md`, `api/CLAUDE.md`, `web/CLAUDE.md`
  - Each new rule states its actual enforcement mechanism honestly: NetArchTest
    for layering (already existed), ESLint for the frontend rules that have a
    configured lint rule, and "code-review convention, not automated" for the
    rules with no realistic automated check in this stack (file size cap,
    parameter limits, magic values) — no rule claims automated enforcement it
    doesn't have.

## TODOS considered
None deferred — the one item considered for deferral (structured logging
expansion) was decided to be built now instead (see T6). CRUD-form E2E
coverage (behind Auth0) is a real, documented gap rather than a deferred
TODO — it needs a test Auth0 tenant/user before it can be picked up, which is
an infrastructure decision for the user to make, not a backlog item to track
here.

---

## Audit 2 (2026-10-19) — extended Pulse checklist

Re-audited against the full Pulse refactor checklist (7 sections, ~45 items — a
superset of the 14 items above), using Pulse's `docs/engineering/CODING_STANDARDS.md`
as the definition of each rule. Branch `refactor/code-quality-audit-2`.

### Reversed decision: error codes
The extended checklist requires `ErrorCodes.cs` and a frontend `types/errors.ts`, and
the old contract had a real problem the first audit missed: the frontend displayed
raw server `ProblemDetails.Detail` text and branched on HTTP status. Now every
failure body is `{ "error": "<code>" }` (Result failures, validation, 401/403, 429,
framework client errors, unhandled exceptions), codes live in `Types/ErrorCodes.cs`
with status mapping in `Extensions/ErrorStatusCodes.cs`, and the frontend maps codes
to copy in `app/types/errors.ts`. `web/tests/types/errors-contract.spec.ts` fails if
the two lists drift.

### Backend — fixed
- `Program.cs` 170 → 39 lines; registrations in `Extensions/*Extensions.cs`; `Utils/` folded into `Extensions/`.
- `Result<T>` rewritten Pulse-style (`ToOk()` / `ToFail()`); upstream Vertex failures are Results (`upstream_unavailable`, 502), not exceptions; `VertexController` no longer returns `Ok(...)` directly.
- Every concrete class `sealed` — **enforced** by `ArchitectureTests.ConcreteClasses_ShouldBeSealed`.
- `is null`, records for DTOs, `IReadOnlyList<T>` returns, primary constructors throughout, `CancellationToken` plumbed controller → repository, no inline fully-qualified types, validator messages are codes.
- Tracked reads renamed `*TrackedAsync`; all others `AsNoTracking()`. Fixed a real bug: `GET /tag/{slug}` always 500'd after the first audit's AsNoTracking change (include cycle) — now `AsNoTrackingWithIdentityResolution()`.
- Single typed `HttpClient` (`VertexGenerateContentClient`) via `IHttpClientFactory`.
- Tests 100 → 205: empty/not-found/401/upstream-failure/rate-limit cases per endpoint group, stubbed Vertex upstream, `ErrorContractTests` pinning code → status.

### Frontend — fixed
- `unwrap()` on every store call (the generated client returns `{ data, error }`); `tryCatchFinally` catches `unknown` and records `errorCode`.
- Auto-imports off (`imports: { autoImport: false }`) — explicit imports **enforced** by `vue-tsc`.
- `no-explicit-any` / `no-non-null-assertion` are ESLint errors — **enforced**.
- All public pages SSR via `useAsyncData` + `{ lazy: true }` + server-only await (technologies was `onMounted`).
- Tests moved to `tests/**/*.spec.ts`; 79 → 192 unit tests; E2E selectors → `data-testid`, new specs for home, AI search (mocked upstream), companies, theme/mobile menu.
- One source for lookup data (`data/TagTypes.ts` keyed by the generated `TagType` enum), shared formatting helpers, `AiStore` split by concern, `<NuxtImg>` in the lightbox, inline styles reduced to CSS-custom-property bindings, Pulse `<script setup>` order.
- `yarn typecheck` used to always exit 0 (`… | grep -v node_modules || echo`); it is now plain `vue-tsc --noEmit`, and the Web Lint & Test workflow runs typecheck and unit tests (it previously ran only lint).

### Dead-code sweep
Frontend checked with knip (components are explicitly imported, so it traces them); backend with the IDE0051/IDE0052/IDE0060/IDE0005 analyzers plus a public-member reference scan. Deleted: `ToResultOrFail` (never called), `ProjectRepositoryService.GetProjectRepositoriesAsync` (no callers), five service getters only reached from tests (`GetProjectRepositoryAsync`, `GetProjectImage(s)Async`, `GetProjectStat(s)Async`) and the test-only `ProjectId` overloads of the three `Sync…Async` methods (tests now drive the `Project` overload production uses), the now-unreturned codes `project_image_not_found` / `project_repository_not_found` / `project_stat_not_found`, and the frontend's unused `notEmpty`. Unused npm packages are listed under the dependency audit below.

### Kept as-is, deliberately
- **Stores and composables stay flat** (`app/store/*Store.ts`) — the documented layout here is one store per domain; one-file subfolders would add nothing.
- **Formatting helpers stay in `app/utils/*-helper.ts`** rather than a `utils/format/` folder — the rule's intent (no ad-hoc formatting in components) is met.
- **`VertexCredentialFactory` still throws** on invalid credentials JSON — deploy misconfiguration should fail loudly.
- **No non-generic `Result`** — the only no-value operation (`OptimiseImagesAsync`) would change its success response from 200 to 204.
- **Not applicable here:** Hangfire jobs, admin app, `docs/engineering|design|product/*`, `docs/INDEX.md`.

### Open — needs a decision or a separate PR
- **DESIGN.md mismatches** (reported, not changed — design changes need approval): featured slug list differs from DESIGN.md (glasgow-retina vs Asurafin); confirm-dialog Delete text is a hardcoded oklch with no `--danger-ink` token; FileInput overlays use `text-white` / raw black oklch; hover lifts beyond the one authored moment (gallery tiles, citation cards, list rows); hero AI prompt and search field are rounded rectangles, not pills; company timeline lacks the "View all →" link; accent used more than once per view (TagList group icons, LoadError icon + Retry).
- ~~**Dependency audit (separate PR)**~~ — done on `chore/dependency-upgrades`:
  - .NET 8 → 10 (EF Core / ASP.NET 10.0.13, Npgsql 10, Swashbuckle 6 → 10 with the Microsoft.OpenApi v2 migration, Auth0/Google/test packages to latest). Swagger output and the generated TypeScript client are unchanged; no pending EF model changes. xunit stays on v2 (v3 is a separate migration); FluentValidation.AspNetCore 11.3.1 is deprecated upstream with no successor version. **The droplet needs the ASP.NET Core 10 runtime in `/usr/share/dotnet` before this deploys** (publish is framework-dependent).
  - npm: removed 21 unused packages (incl. the `@pinia-plugin-persistedstate/nuxt` wrapper — `pinia-plugin-persistedstate` v4 ships its own Nuxt module, same cookie storage); `typescript` / `@types/node` moved to devDependencies; `@stylistic/eslint-plugin` and `@vue/devtools-api` (pinia 4 peer) declared; everything upgraded within range; pinia 4, @pinia/nuxt 1, @nuxtjs/sitemap 9, lint-staged 17. Production audit 7 → 0 advisories; all-deps 62 → 3 (dev-server-only paths inside Nuxt, no upstream patch). TypeScript 7 skipped — `vue-tsc` doesn't support the native compiler yet. `puppeteer` (used only by `scripts/generate-meta-image.js`) left undeclared to avoid a Chromium download on every install.
  - Fixed a pre-existing test bug found on the way: `MediaControllerIntegrationTests` recursively deleted the real `wwwroot/uploads`, and upload tests wrote into it. Test factories now use a temp web root.
- **E2E against local data:** 3 console-error checks fail locally (35/38 pass on .NET 10 + the upgraded frontend) because `_ipx` 404s on upload images missing from the local `wwwroot/uploads`; they need the real uploads.
- **Auth-gated CRUD E2E** — still needs a test Auth0 tenant/user.
- `VertexAiOptions.QueryDataStoreSuffix` is unused but still set by workflows/Dockerfile/README; `Models/Project.cs` uses Newtonsoft `[JsonIgnore]`, so System.Text.Json still serialises `Logo`/`Card`/`Banner` (fixing changes the success response).

---

## Pulse patterns pass (2026-10-19)

Compared the code (not the checklist) against Pulse and ported what was missing. Branch `refactor/pulse-patterns`.

### Backend — fixed
- `Result` / `Result<T>` are `readonly record struct`s (`Value`, plus a non-generic `Result`); `ToResultOrFail` is back in use. `ResultExtensions` maps un-awaited tasks, so controllers are one expression: `ToResponse()`, `ToCreatedResponse()` (POST → **201** + `Location`), `ToNoContentOrError()` (DELETE and image optimise → **204**). Reverses audit 2's "no non-generic `Result`".
- **Bug:** titles that differ but share a slug ("C#"/"C", "Vue.js"/"Vuejs") hit the unique index and returned 500. Slugs now go through `ToUniqueSlugAsync` (`c`, `c-2`, then a random suffix), only change when the title does, and fall back to the entity name when a title has no slug characters.
- **Bug:** `TagRepository.GetByTitleTrackedAsync` passed its arguments to `ILike` reversed, and every "already exists" check used the user's title as a LIKE pattern, so `_`/`%` matched other rows (a tag titled `%` "already existed"). Exact matches are now `lower() = lower()`, search is `strpos`, and the `SupportsILike()` provider branches that existed only for the InMemory tests are gone.
- List endpoints bind `[FromQuery] SearchQuery` with a validator (`search_too_long`, 100 chars) — also bounds the cache keys built from the search string.
- Committed Auth0 `ClientSecret` / `ApiClientSecret` removed from `appsettings.json` (they were only there so the tests could fetch a real token).

### Tests — fixed
- Real PostgreSQL via Testcontainers instead of EF InMemory: template database migrated once, cloned per unit test and per integration class. It immediately caught two tests seeding duplicate slugs that InMemory let through.
- Locally-signed JWTs (`TestJwt`) instead of a live Auth0 client-credentials call: no network or secrets; added expired / wrong-key / wrong-audience cases. CI no longer runs docker-compose or passes Vertex secrets to restore/build.
- Integration classes run in parallel (own database each): 191 → 223 tests, ~6 s.

### Frontend — fixed
- `tryCatchFinallyIfLatest` replaces the two hand-rolled stale-request guards (`ProjectStore.getProjectBySlug`, `AiSearchStore`). Pulse's closure-based `createRequestGeneration` wasn't ported as-is: these are options stores whose state is SSR-serialised, so the counter stays in state.
- Client regenerated (201/204 responses, typed `VertexIngestSummary`). `yarn codegen` used to exit 1 after generating because its ESLint post-step targeted the ESLint-ignored `api/` folder; the step is removed.

### Kept as-is, deliberately
- **No pagination.** Lists are a few dozen rows and the frontend filters, derives tag-type filters and builds the timeline from the full list client-side. The real cost is payload size, which the DTO work below addresses.

### Open
- **Response DTOs** (Pulse's `ToDto()` convention): controllers still return EF entities, which is also why `Project`'s Newtonsoft `[JsonIgnore]`s don't apply. Separate PR — summary DTO for lists, detail DTO for `GET /project/{slug}`, codegen, frontend types.
- **Rotate the Auth0 secrets** that were committed: they remain in git history on a public repo.
- **Auth-gated CRUD E2E** still needs a frontend-side test login (the backend half — tests that don't need Auth0 — now exists).

---

## Showcase hardening pass (2026-10-19)

Reviewed the repo and the live site as an outside reviewer would. Branch `chore/portfolio-hardening`.

### Renamed
- "Work Experience" / "Work Experience Search" → **Portfolio**: `api/Portfolio.Api` + `api/Portfolio.Api.Tests` (namespaces `Portfolio.Api.*`; the redundant `Tests/` folder flattened), `Portfolio.sln`, Swagger title, PM2 app `PortfolioWeb`, PWA manifest, docs. The site's own case study moves from `/projects/work-experience` to `/projects/portfolio` (301 from the old URL). Server paths (`/var/www/experience/...`) and local database names are unchanged.

### Fixed
- **Bug:** the AI search rate limiter partitioned on `RemoteIpAddress`, which behind nginx was `127.0.0.1` for everyone, so all visitors shared 10 queries/minute. Forwarded headers are now trusted from loopback (test fails without the fix).
- **Bug:** the deploy clean-up step used `ARTIFACTS_PATH` without passing it to the SSH session, so it ran `ls -t | tail -n +3 | xargs rm -rf` in `/root` on every deploy.
- **Bug:** the auth middleware read `isAuthenticated` without awaiting `checkSession()`, and the login callback never returned the user to the page they were heading for.
- Security headers on both sides (nonce CSP + SRI via `nuxt-security`; `nosniff`/DENY/CSP/HSTS on the API), CORS restricted to the frontend origin, `x-powered-by` removed, Swagger public.
- `GET /project` returns `ProjectSummary` (the "Response DTOs" open item from the Pulse patterns pass, for lists); homepage HTML 227 KB → 76 KB.
- Auth0 SDK lazy-loaded; `/_nuxt` assets precompressed (br/gz) and `immutable`; canonical links, `og:url`, JSON-LD (`Person`, `CreativeWork`).
- CI: deploys gated on unit + integration + E2E; E2E in CI against a production build and a seeded API; codegen drift check; coverage summaries; CodeQL; Lighthouse CI; Dependabot; every action pinned to a SHA. Deploys health-check and roll back (API under systemd).
- Deleted the broken, unused Dockerfile and `.http` template file, and the unused `public/site.webmanifest`. `NUXT_PUBLIC_BASE` is no longer used (absolute URLs come from `SITE_URL`).
- Docs: showcase README, ADRs (`docs/adr/`), deployment guide (`docs/deploy/`), MIT `LICENSE` + `NOTICE`.

### Open — needs action outside the repo
- **nginx** (see `docs/deploy/README.md`): serve `/_nuxt/` with `gzip_static` + `immutable`, gzip JS/CSS, enable HTTP/2, `server_tokens off`, forward `X-Forwarded-For`/`-Proto` to the API. Until then the live site still serves JS uncompressed.
- After deploying, rename the CMS project "Work Experience" to "Portfolio" so its slug becomes `portfolio`.
- `NUXT_PUBLIC_BASE` can be removed from the `NUXT_ENV` secret.
- Auth-gated CRUD E2E still needs a test login.

