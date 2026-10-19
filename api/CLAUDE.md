# Work Experience Search — Backend (.NET API)

## Code Style
Code should be intuitive enough to read without comments. Default to writing no comments at all. Only add one when there's vital context a reader can't get from the code itself — a non-obvious constraint, a workaround for a specific bug, a subtle invariant. Never write a comment that restates what the code already says. When editing existing code, remove comments that don't meet this bar rather than leaving them.
- No commented-out code — delete it. Git has history.
- No TODO comments in merged code.
- No XML doc comments on internal methods.

## Layering
`Controller → Service → Repository → EF Core (Database)`. This is enforced by `WorkExperienceSearchTests/Tests/Architecture/ArchitectureTests.cs` (NetArchTest rules) — Controllers can't reference Repositories directly, Services can't reference Controllers, Repositories can't reference Services/Controllers. Keep new code inside these boundaries; if a test starts failing after adding a dependency, that's the architecture test catching a layering violation, not a bug in the test.

**Primary constructors for DI everywhere** — no `readonly` field boilerplate. Every concrete class is `sealed` (enforced by `ArchitectureTests.ConcreteClasses_ShouldBeSealed`):
```csharp
public sealed class ProjectController(IProjectService projectService) : ControllerBase
public sealed class ProjectService(IProjectRepository projectRepository, ITagService tagService) : IProjectService
```

**`Result` / `Result<T>` pattern, not exceptions, for expected failures.** `Types/Result.cs` defines `readonly record struct`s `Result` (no value) and `Result<T>` (with `Value`), whose `Error` is an error code from `Types/ErrorCodes.cs`. Write `value.ToOk()` / `Result.Ok()` for success, `ErrorCodes.X.ToFail()` for failure (converts implicitly to `Result` and any `Result<T>`), and `value.ToResultOrFail(ErrorCodes.X)` for a nullable lookup. Use `Result<IReadOnlyList<X>>.Ok(list)` when the value's static type isn't exactly `T` (e.g. a `List<X>`). An operation with nothing to return (delete, optimise) returns `Result`, never `Result<bool>` or the deleted entity.

Controllers are one expression: call the service and map the un-awaited task with an extension from `Extensions/ResultExtensions.cs` — `.ToResponse()` (200), `.ToCreatedResponse(nameof(GetXById), x => new { id = x.Id })` (201 + `Location`, with `[ProducesResponseType(StatusCodes.Status201Created)]`), `.ToNoContentOrError()` (204, with `[ProducesResponseType(StatusCodes.Status204NoContent)]`) or `.ToFileResponse()`. Keep `ActionResult<T>` return types so Swagger (and so codegen) infers the response type. Controllers never branch on errors themselves:
```csharp
// Service — a public read: not found is the caller's problem, not worth a log line
public async Task<Result<Project>> GetProjectAsync(ProjectId id, CancellationToken cancellationToken = default) =>
    (await projectRepository.GetAsync(id, cancellationToken)).ToResultOrFail(ErrorCodes.ProjectNotFound);

// Controller
[HttpGet("{id:guid}")]
public Task<ActionResult<Project>> GetProjectById(ProjectId id, CancellationToken cancellationToken) =>
    projectService.GetProjectAsync(id, cancellationToken).ToResponse();

[HttpDelete("{id:guid}")]
[Authorize]
[ProducesResponseType(StatusCodes.Status204NoContent)]
public Task<IActionResult> DeleteProject(ProjectId id, CancellationToken cancellationToken) =>
    projectService.DeleteProjectAsync(id, cancellationToken).ToNoContentOrError();
```

**Query strings bind to a `[FromQuery]` record** (`Requests/SearchQuery.cs`) with a FluentValidation validator whose messages are `ErrorCodes` values — never a bare `string?` action parameter. Use `[FromQuery(Name = "...")]` on the property so the Swagger parameter name (and the generated client) stays camelCase.

**Slugs** come from `title.ToUniqueSlugAsync(fallback, repository.SlugExistsAsync…)` (`Extensions/SlugExtensions.cs`): two titles that normalise to the same slug ("C#" and "C") get `c` and `c-2` rather than a unique-index violation, and a title with no slug characters falls back to the entity name. Only re-resolve the slug when the title actually changes, so an edit never moves an existing URL.

**Text matching in repositories.** Case-insensitive equality is `x.Title.ToLower() == title.ToLower()`; substring search is `x.Title.ToLower().Contains(search.ToLower())` (Npgsql translates it to `strpos`, so `%` and `_` in user input match literally). Don't pass user input to `EF.Functions.ILike` as a pattern.

**Error contract.** Every failure body is `{ "error": "<snake_case_code>" }` (`Types/ApiErrorDto.cs`) — Result failures, FluentValidation 400s (`InvalidModelStateResponseFactory`, first code wins), framework client errors (`Extensions/ApiClientErrorFactory.cs`), 401/403 (JwtBearer events), 429 (rate limiter) and unhandled exceptions (`Extensions/GlobalExceptionHandler.cs` → `internal_error`). Codes are `const string`s in `Types/ErrorCodes.cs` — never an inline string literal, including validator `.WithMessage(...)`. The HTTP status for each code lives in one place, `Extensions/ErrorStatusCodes.cs` (anything unlisted → 400); add a code there when it needs another status. Never build `Ok(...)`, `BadRequest(new { error })` or `StatusCode(...)` inline in a controller. When adding a code, mirror it in the frontend's `web/app/types/errors.ts`. `ErrorContractTests` pins the code → status table.

Real exceptions (`Exceptions/NotFoundException`, `ConflictException`) still exist for genuinely unexpected failures and are caught by `GlobalExceptionHandler` — don't use them to signal expected/handled failures. Upstream (Vertex AI) failures are expected failures: `VertexGenerateContentClient` and `VertexIngestOrchestrator` turn them into `upstream_unavailable` (502). Startup misconfiguration (invalid credentials JSON in `VertexCredentialFactory`) still throws, deliberately.

**Strongly-typed IDs.** Every domain entity ID (`ProjectId`, `CompanyId`, `TagId`, etc.) is a `readonly record struct` wrapping `Guid`, defined in `Types/Ids.cs`, with a custom `TypeConverter`, JSON converter, and EF `HasGuidIdConversion()` value conversion. Never use a raw `Guid` for a domain entity's primary or foreign key — add a new ID type in `Types/Ids.cs` when adding a new entity.

**Repository reads use `IMemoryCache`.** `BaseRepository` wraps caching; concrete repositories build cache keys manually and invalidate via `CacheInvalidator` (`IChangeToken` per entity type) on writes — see `Repositories/Project/ProjectRepository.cs`. When adding a new query method that should be cached, follow the existing key-building convention in the same repository rather than inventing a new scheme.

**Async conventions:** all repository/service methods are `async Task<...>` with an `Async` suffix and accept `CancellationToken cancellationToken = default`; controller actions take a `CancellationToken` and pass it all the way down. Never `.Result` or `.Wait()` on a `Task`.

**`Program.cs` stays lean** — a short list of `builder.Services.AddXxx()` / `app.UseXxx()` calls. Register new integrations in an `Extensions/<Concern>Extensions.cs` method, never inline. `HttpClient` only via `IHttpClientFactory` (typed clients registered with `AddHttpClient<TInterface, TImpl>()`).

## Code quality standing rules

Established by an audit-driven cleanup pass (see `docs/CODE_QUALITY_BACKLOG.md` at the repo root) — code review should hold new code to these, not just legacy code:

- **File size cap ~500-600 lines.** Split a service by responsibility or extract a sub-component before a file grows past this. No automated check for this yet (`.NET` has no built-in line-count analyzer) — a code-review convention, same as the comment-policy rules above.
- **Method parameters: max 5-6.** Beyond that, introduce a dedicated request/DTO class instead of growing the parameter list further.
- **Repository reads use `.AsNoTracking()`** unless the returned entity is mutated and saved by the caller — name those `*TrackedAsync` (`GetTrackedAsync`, `GetByTitleTrackedAsync`, `GetUnoptimisedImagesTrackedAsync`) so tracking is visible at the call site. Queries whose includes form a cycle (e.g. `Tag → Projects → Tags`) need `AsNoTrackingWithIdentityResolution()`.
- **LINQ over raw SQL in repositories.** `FromSqlRaw` is a last resort for queries EF Core genuinely can't express — not a convenience default. If you do need it, parameterize it (never string-interpolate values into the SQL).
- **Entity → DTO mapping via a method, not inline construction.** No dedicated mapper class exists in this codebase; the convention is a private (usually `static`) mapping method on the service that needs it (see `VertexIngestService.FlattenProject`/`FlattenCompany`/`FlattenTag`), and one `ApplyDetails(entity, request)` per service shared by create and update. Don't construct a DTO field-by-field at the call site, and use named records rather than anonymous objects for prompt/request payloads.
- **C# style:** `is null` / `is not null`; `record` for DTOs, requests and value objects (options classes bound from config may stay classes); `IReadOnlyList<T>` for collection return types; strongly-typed IDs on every entity/DTO `Id` — the one exception is `VertexCitation.Id`, a polymorphic `Guid?` paired with its `FeatureType` discriminator and converted to the matching typed ID at the service boundary.
- **Top-of-file `using` directives over inline fully-qualified type references.** Add the `using` rather than writing `System.Text.Json.JsonSerializer.Serialize(...)` inline.
- **Structured logging on failure paths.** Inject `ILogger<T>` via the primary constructor and log write-path `Result` failures (`LogWarning`, with the entity ID as a named property, e.g. `logger.LogWarning("Tag {TagId} not found for update.", id)`) and caught exceptions (`LogError`, with the exception). A not-found on a public read (`GET` by id or slug) is not logged — crawlers and stale links make it noise, and the 404 is already in the request log. Don't log the happy path — match the restraint already shown in `Services/VertexAi/`. Never string-interpolate log messages; always named properties, so they stay queryable as structured fields.
- **No magic values.** A literal with business meaning (a status string, a numeric limit) should be a named constant or enum member, not a bare literal at the call site.

## Structure
- `Controllers/` — thin, one action per route, no business logic.
- `Services/<Domain>/` — business logic, one interface + implementation per domain (`Services/Project/`, `Services/Company/`, `Services/VertexAi/`).
- `Repositories/<Domain>/` — EF Core queries, one per domain; `BaseRepository.cs` at root for shared caching behaviour.
- `Models/` — EF entities.
- `Requests/` — record DTOs for inbound requests, one file per domain, plus `SearchQuery` for list query strings.
- `Validators/` — FluentValidation, one validator per request type.
- `Filters/` — Swagger/OpenAPI filters.
- `Exceptions/` — `NotFoundException`, `ConflictException` for unexpected-failure paths only.
- `Extensions/` — startup registration (`*Extensions.cs` called from `Program.cs`), the error contract (`ResultExtensions`, `ErrorStatusCodes`, `ApiClientErrorFactory`, `GlobalExceptionHandler`) and general extension methods (slug, enum, file).
- `Types/` — `Ids.cs` (strongly-typed IDs), `Result.cs`, `ErrorCodes.cs`, `ApiErrorDto.cs`, `RateLimitPolicies.cs`.
- `Migrations/` — EF Core migrations, never hand-edited (see below).

## Migrations
Never create migration files manually. Always run `dotnet ef migrations add <Name>` from `api/WorkExperienceSearch/`. Manual migrations skip the `.Designer.cs` snapshot and cause EF to block `database update` with a `PendingModelChangesWarning`. Apply with `dotnet ef database update`.

## Testing
xUnit + Moq + `NetArchTest.Rules` + Testcontainers, tests live in `WorkExperienceSearchTests/Tests/`. **Tests hit real PostgreSQL — never an in-memory provider**, because unique indexes, `lower()`/`strpos` translation and FK cascades are exactly what an in-memory provider gets wrong. Docker must be running (or set `TEST_POSTGRES_CONNECTION`). Production code never branches on the database provider.
- `Fixtures/` — `TestDatabase` (one container per run, migrations applied once to a template database, a fresh clone per test/class), `TestJwt` (locally-signed tokens), `ApiWebApplicationFactory` (abstract: per-class database, temp web root, JwtBearer pointed at the test key) with sealed `CustomWebApplicationFactory` / `VertexStubWebApplicationFactory` (stubs the Vertex token provider, HTTP handler and chatbot client).
- `Unit/Services/` — real repositories against a fresh database per test, mock only sibling services. Seeded via `BaseServiceTests : IAsyncLifetime`.
- `Integration/` — real `HttpClient` against the factory. `AuthenticatedClient` carries a valid test JWT; use `CreateClientWithToken(TestJwt.Create(...))` for expired/wrong-key/wrong-audience cases. No network or Auth0 secrets needed. Classes run in parallel (each has its own database). Assert failure bodies with `AssertErrorAsync` (exact error code) and success statuses exactly (`201` + `Location`, `204`), not `EnsureSuccessStatusCode`.
- `Architecture/` — layering rules described above.

Every new feature needs happy-path, not-found/empty, upstream-failure (where it calls Vertex or another external service) and 401/403 (for `[Authorize]` endpoints) tests.

## Style rules (`.editorconfig`)
4-space indent, UTF-8 BOM, trailing whitespace trimmed, `var` preferred everywhere, interfaces must be `I`-prefixed. `is null` / `is not null` — never `== null`.

## Running locally
```
docker-compose -f WorkExperienceSearch/docker-compose.yml up -d   # Postgres
dotnet ef database update --project WorkExperienceSearch
dotnet run --project WorkExperienceSearch                          # http://localhost:5105, Swagger at /swagger
```
`.env` (via `dotenv.net`) supplies `ConnectionStrings__DefaultConnection`, `Auth0__*`, `VertexAi__*` — see `README.md` for the full list. Never commit `.env`.

## Auth
Auth0 JWT Bearer scheme. `[Authorize]` on write endpoints.

## Docs
`README.md` at this project's root has the fuller getting-started guide and CI/CD notes — read it alongside this file.
