# Work Experience API

A .NET 8 REST API serving the [Work Experience](https://experience.josephpickering.co.uk) portfolio website. Built with clean architecture, a typed Result pattern, AI-powered search via Google Vertex AI, and automated CI/CD to Digital Ocean.

**Live API:** [api.experience.josephpickering.co.uk](https://api.experience.josephpickering.co.uk)
**Swagger UI:** [api.experience.josephpickering.co.uk/swagger](https://api.experience.josephpickering.co.uk/swagger)

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 8 / C# 12 |
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
// Service — primary-constructor DI, sealed, CancellationToken plumbed through
public sealed class ProjectService(IProjectRepository projectRepository, ILogger<ProjectService> logger) : IProjectService
{
    public async Task<Result<Project>> GetProjectAsync(ProjectId id, CancellationToken cancellationToken = default)
    {
        var project = await projectRepository.GetAsync(id, cancellationToken);
        if (project is null)
        {
            logger.LogWarning("Project {ProjectId} not found.", id);
            return ErrorCodes.ProjectNotFound.ToFail();
        }

        return project.ToOk();
    }
}

// Controller
public sealed class ProjectController(IProjectService projectService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Project>> GetProject(ProjectId id, CancellationToken cancellationToken) =>
        (await projectService.GetProjectAsync(id, cancellationToken)).ToResponse();
}
```

**Lean `Program.cs`** — startup is a list of `AddXxx()` / `UseXxx()` calls; each integration (database, repositories, services, Vertex AI, Swagger, Auth0, CORS, rate limiting, exception handling) registers itself in `Extensions/*Extensions.cs`.

**Repository pattern with base class** — `BaseRepository` centralises the `IMemoryCache` get/set helpers so each concrete repository stays focused on building its own cache keys and queries.

**Token-based cache invalidation** — `CacheInvalidator` issues `IChangeToken` instances per entity type. Write operations (add / update / delete) cancel the relevant token, atomically evicting all related cache entries without explicit key tracking.

**Typed IDs** — every entity uses a strongly-typed ID wrapper (e.g. `ProjectId`, `CompanyId`) backed by `Guid`, surfaced as plain UUIDs in the OpenAPI spec.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
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
   docker-compose -f WorkExperienceSearch/docker-compose.yml up -d
   ```

3. **Configure environment variables**

   Create a `.env` file in `WorkExperienceSearch/` (use `.env.example` as a reference). Most keys use a colon, matching `IConfiguration`'s path syntax directly — `dotenv.net` loads them as literal environment variable names and ASP.NET's `AddEnvironmentVariables()` binds a colon in the name straight through. The connection string uses `ConnectionStrings__DefaultConnection` so `GetConnectionString("DefaultConnection")` picks it up (`__` also maps to `:`):

   ```env
   ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Database=work_experience;Username=postgres;Password=postgres
   Auth0:Domain=your-auth0-domain.auth0.com
   Auth0:ClientId=your-client-id
   Auth0:ClientSecret=your-client-secret
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
   cd WorkExperienceSearch
   dotnet ef database update
   ```

5. **Run the application**

   ```bash
   dotnet run --project WorkExperienceSearch
   ```

   The API starts at `http://localhost:5105`. Swagger is available at `http://localhost:5105/swagger`.

6. **Check health**

   ```bash
   curl http://localhost:5105/health
   ```

---

## Testing

The test suite covers three levels:

| Type | Location | Tooling |
|---|---|---|
| Unit | `WorkExperienceSearchTests/Tests/Unit/` | xUnit, Moq, EF Core `InMemoryDatabase` provider |
| Integration | `WorkExperienceSearchTests/Tests/Integration/` | `WebApplicationFactory` (EF Core `InMemoryDatabase`), real HTTP client, stubbed Vertex AI upstream |
| Architecture | `WorkExperienceSearchTests/Tests/Architecture/` | NetArchTest |

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

Two GitHub Actions workflows (path-filtered to `api/**`) run on push:

**`.github/workflows/api-tests.yml`** — spins up a PostgreSQL container via Docker Compose, restores, builds, and runs the full test suite. Pull requests cannot be merged if this workflow fails.

**`.github/workflows/api-deploy.yml`** — triggers on push to `main`. Publishes the app, uploads the artifact to the Digital Ocean droplet via SCP, runs `dotnet ef database update` on the server, then stops the currently running process, symlinks the new release into place, and starts it — a brief restart window, not a zero-downtime swap.

---

## Project Structure

```
WorkExperienceSearch/
├── Controllers/          # Thin HTTP layer — receive, delegate, respond
├── Services/<Domain>/    # Business logic, one folder per domain (Project, Company, Tag, File, Image, VertexAi, Database)
├── Repositories/<Domain>/ # EF Core data access, one folder per domain; BaseRepository.cs at root for shared caching
├── Requests/             # Immutable record DTOs for all endpoints
├── Validators/           # FluentValidation validators (one per request type)
├── Models/               # EF Core entity models
├── Types/                # Result<T>, ErrorCodes, ApiErrorDto, typed IDs (Ids.cs)
├── Exceptions/           # Typed exception hierarchy (unexpected-failure paths only)
└── Extensions/           # Startup registration, error contract, extension methods (string, enum, file)

WorkExperienceSearchTests/
├── Tests/Unit/           # Service-level tests with mocked dependencies
├── Tests/Integration/    # Controller-level tests against a real HTTP server
└── Tests/Architecture/   # NetArchTest layer-boundary assertions
```

---

## Conventions

Coding conventions (layering, comment policy, testing, style) live in [`CLAUDE.md`](./CLAUDE.md). The repo root's [`docs/CODE_QUALITY_BACKLOG.md`](../docs/CODE_QUALITY_BACKLOG.md) tracks the active code-quality backlog for both projects.
