# Portfolio API

A .NET 10 REST API serving the portfolio website at [josephpickering.co.uk](https://josephpickering.co.uk). Built with clean architecture, a typed Result pattern, AI-powered search via Google Vertex AI, and automated CI/CD to Digital Ocean.

**Live API:** [api.experience.josephpickering.co.uk](https://api.experience.josephpickering.co.uk/health)
**Swagger UI:** [api.experience.josephpickering.co.uk/swagger](https://api.experience.josephpickering.co.uk/swagger)

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10 / C# 14 |
| Database | PostgreSQL (EF Core + Npgsql) |
| Authentication | Auth0 (JWT Bearer) |
| AI Search | Google Vertex AI Discovery Engine |
| Validation | FluentValidation |
| Logging | Serilog (structured, console + file) |
| Image optimisation | Tinify |
| API Docs | Swagger / Swashbuckle |
| Testing | xUnit, Moq, NetArchTest, Coverlet |
| CI/CD | GitHub Actions |
| Hosting | Digital Ocean droplet |

---

## Architecture

```
Controllers  →  Services  →  Repositories  →  Database (EF Core / PostgreSQL)
                    ↓
              External APIs (Vertex AI, Tinify, Auth0)
```

### Key Patterns

**Result pattern** — every fallible service method returns `Result<T>` carrying an error code from `Types/ErrorCodes.cs`. Controllers never throw or branch; a single `result.ToResponse()` call maps success to 200/204 and failure to `{ "error": "<code>" }` with the status from `Extensions/ErrorStatusCodes.cs`. Validation, auth, rate-limit and unhandled-exception failures use the same body.

```csharp
// Service — primary-constructor DI, sealed, CancellationToken plumbed through.
// A not-found on a public read is the caller's problem, so it isn't logged.
public sealed class ProjectService(IProjectRepository projectRepository, ILogger<ProjectService> logger) : IProjectService
{
    public async Task<Result<Project>> GetProjectAsync(ProjectId id, CancellationToken cancellationToken = default) =>
        (await projectRepository.GetAsync(id, cancellationToken)).ToResultOrFail(ErrorCodes.ProjectNotFound);
}

// Controller — one expression, never branches on the result
public sealed class ProjectController(IProjectService projectService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public Task<ActionResult<Project>> GetProjectById(ProjectId id, CancellationToken cancellationToken) =>
        projectService.GetProjectAsync(id, cancellationToken).ToResponse();
}
```

**Lean `Program.cs`** — startup is a list of `AddXxx()` / `UseXxx()` calls; each integration (database, repositories, services, Vertex AI, Swagger, Auth0, CORS, rate limiting, exception handling) registers itself in `Extensions/*Extensions.cs`.

**Repository pattern with base class** — `BaseRepository` centralises the `IMemoryCache` get/set helpers so each concrete repository stays focused on building its own cache keys and queries.

**Token-based cache invalidation** — `CacheInvalidator` issues `IChangeToken` instances per entity type. Write operations (add / update / delete) cancel the relevant token, atomically evicting all related cache entries without explicit key tracking.

**List responses are summaries** — `GET /project` returns `ProjectSummary` records (`Responses/`) with only the fields list views render, plus a server-chosen `cardImageUrl`; the full `Project` comes from `GET /project/{slug}`. That cut the homepage's server-rendered payload from 227 KB to 76 KB.

**Hardened for running behind nginx** — forwarded headers are trusted from loopback, so the AI search rate limit is per visitor rather than per proxy and HSTS is sent; every response carries `nosniff`, `X-Frame-Options: DENY`, `Referrer-Policy` and a `default-src 'none'` CSP (`Extensions/SecurityExtensions.cs`); CORS only allows the configured frontend origin (`Cors:AllowedOrigins`).

**Typed IDs** — every entity uses a strongly-typed ID wrapper (e.g. `ProjectId`, `CompanyId`) backed by `Guid`, surfaced as plain UUIDs in the OpenAPI spec.

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/) (for the local PostgreSQL instance)
- An [Auth0](https://auth0.com/) application and API
- *(Optional)* Google Cloud project with Vertex AI Discovery Engine for AI search

### Installation

1. **Clone the repository**

   ```bash
   git clone https://github.com/josephpickering9/portfolio.git
   cd portfolio/api
   ```

2. **Start the database**

   ```bash
   docker-compose -f Portfolio.Api/docker-compose.yml up -d
   ```

3. **Configure environment variables**

   Create a `.env` file in `Portfolio.Api/` (use `.env.example` as a reference). Most keys use a colon, matching `IConfiguration`'s path syntax directly — `dotenv.net` loads them as literal environment variable names and ASP.NET's `AddEnvironmentVariables()` binds a colon in the name straight through. The connection string uses `ConnectionStrings__DefaultConnection` so `GetConnectionString("DefaultConnection")` picks it up (`__` also maps to `:`):

   ```env
   ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=work_experience;Username=postgres;Password=postgres
   Auth0:Domain=your-auth0-domain.auth0.com
   Auth0:ClientId=your-client-id
   Auth0:Audience=your-api-audience

   # Optional — required only for AI search
   VertexAi:ProjectId=your-gcp-project-id
   VertexAi:Location=global
   VertexAi:Collection=default_collection
   VertexAi:Branch=0
   VertexAi:Model=gemini-3.8-flash
   VertexAi:ModelLocation=us-central1
   VertexAi:QueryDataStoreSuffix=your-datastore-suffix
   VertexAi:CredentialsFile=/path/to/service-account.json
   ```

4. **Apply database migrations**

   ```bash
   cd Portfolio.Api
   dotnet ef database update
   ```

5. **Run the application**

   ```bash
   dotnet run --project Portfolio.Api
   ```

   The API starts at `http://localhost:5105`. Swagger is available at `http://localhost:5105/swagger` (and publicly at [api.experience.josephpickering.co.uk/swagger](https://api.experience.josephpickering.co.uk/swagger)).

6. **Check health**

   ```bash
   curl http://localhost:5105/health
   ```

---

## Testing

The test suite covers three levels:

| Type | Location | Tooling |
|---|---|---|
| Unit | `Portfolio.Api.Tests/Unit/` | xUnit, Moq, real PostgreSQL (Testcontainers) |
| Integration | `Portfolio.Api.Tests/Integration/` | `WebApplicationFactory` against real PostgreSQL, real HTTP client, locally-signed JWTs, stubbed Vertex AI upstream |
| Architecture | `Portfolio.Api.Tests/Architecture/` | NetArchTest |

Tests run against real PostgreSQL, so Docker must be running. `Fixtures/TestDatabase.cs` starts one `postgres:17-alpine` container per run, applies the migrations to a template database once, and clones a fresh database from it for every unit test and every integration test class. Set `TEST_POSTGRES_CONNECTION` to a server connection string to use an existing PostgreSQL instead of a container.

The suite needs no network access and no secrets: integration tests sign their own JWTs (`Fixtures/TestJwt.cs`) and the factory points JwtBearer at that test key, so the real authentication pipeline (including the `401 { "error": "unauthorized" }` body) is exercised without calling Auth0.

Run all tests:

```bash
dotnet test
```

Run with coverage:

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Architecture rules enforced

- Controllers must not reference Repository types directly
- Services must not reference Controller types
- Repositories must not reference Service or Controller types
- Every concrete class is `sealed`

---

## CI/CD

`.github/workflows/api.yml` (path-filtered to `api/**`) builds and tests the API with a coverage summary on every PR and push, runs the shared E2E suite (`e2e.yml`: the Playwright tests against a production frontend build and this API on a seeded PostgreSQL, plus a check that the generated frontend client still matches the Swagger spec), and only then, on `main`, calls `api-deploy.yml`.

The deploy publishes the app and a self-contained migrations bundle, uploads them to the droplet and runs [`deploy/activate.sh`](./deploy/activate.sh): write config, migrate, switch the `current` symlink, restart the `portfolio-api` systemd service and poll `/health`, rolling back to the previous release if it never passes. See [`docs/deploy`](../docs/deploy/README.md).

---

## Project Structure

```
Portfolio.Api/
├── Controllers/          # Thin HTTP layer — receive, delegate, respond
├── Services/<Domain>/    # Business logic, one folder per domain (Project, Company, Tag, File, Image, VertexAi, Database)
├── Repositories/<Domain>/ # EF Core data access, one folder per domain; BaseRepository.cs at root for shared caching
├── Requests/             # Immutable record DTOs for inbound requests
├── Responses/            # Response DTOs where the entity is too heavy (ProjectSummary)
├── Validators/           # FluentValidation validators (one per request type)
├── Models/               # EF Core entity models
├── Types/                # Result<T>, ErrorCodes, ApiErrorDto, typed IDs (Ids.cs)
├── Exceptions/           # Typed exception hierarchy (unexpected-failure paths only)
└── Extensions/           # Startup registration, security, error contract, extension methods (slug, enum, file)

Portfolio.Api.Tests/
├── Unit/                 # Service, model and contract tests against real PostgreSQL
├── Integration/          # Controller-level tests against a real HTTP server
├── Architecture/         # NetArchTest layer-boundary assertions
└── Fixtures/             # Testcontainers database, test JWTs, web application factories
```

---

## Conventions

Coding conventions (layering, comment policy, testing, style) live in [`CLAUDE.md`](./CLAUDE.md). The repo root's [`docs/CODE_QUALITY_BACKLOG.md`](../docs/CODE_QUALITY_BACKLOG.md) tracks the active code-quality backlog for both projects.
