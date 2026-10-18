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
  suite (`WorkExperienceSearchTests/Tests/Architecture/ArchitectureTests.cs`) — zero
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
- **Centralized `ErrorCodes.cs` constants file** — backend already models errors
  via a typed `ErrorType` enum + free-text messages. This satisfies the ticket's
  actual intent (avoid scattered magic error strings) without adding a redundant
  second layer of error identity. Revisit only if free-text messages start
  getting compared/matched on by callers (a real "magic string" smell) — that
  hasn't happened.
- **Full re-audit of all 14 items from scratch** — the two-agent audit already
  covers this with concrete evidence (file:line counts, grep results). Re-running
  it wholesale would mostly re-confirm the same findings for no new fixes.

## What already exists (reused, not rebuilt)
- Backend test harness (`WorkExperienceSearchTests`) — the frontend test setup in
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
- Centralized `ErrorCodes.cs` — explicitly rejected above, revisit only if a real
  magic-string problem emerges.
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
