# 0003. Backend tests run against real PostgreSQL

- **Status:** Accepted
- **Date:** 2026-10

## Context

The tests used EF Core's in-memory provider. It doesn't enforce unique indexes or foreign-key cascades and translates `lower()`, `Contains` and `ILike` differently from Npgsql, which is exactly where this codebase's bugs were (duplicate slugs, user input used as a `LIKE` pattern). Production code also had provider-specific branches only to keep the in-memory tests passing.

## Decision

Tests use Testcontainers: one PostgreSQL container per run, migrations applied once to a template database, and a fresh clone per unit test and per integration class. JWTs are signed locally, so the suite needs no network access or Auth0 secrets. Production code never branches on the provider.

## Consequences

- Docker is required to run the tests (or `TEST_POSTGRES_CONNECTION`).
- On its first run it caught two tests seeding duplicate slugs that the in-memory provider had allowed.
- Cloning from a template keeps the full suite at a few seconds despite real databases.
