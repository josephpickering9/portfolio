# 0002. Expected failures are `Result<T>` values with snake_case error codes

- **Status:** Accepted
- **Date:** 2026-10

## Context

Controllers used to branch on service outcomes and return `ProblemDetails` with free-text messages, and the frontend displayed those messages and branched on HTTP status. Neither side had a stable way to tell "not found" from "slug taken".

## Decision

Services return `Result` / `Result<T>` whose error is a code from `Types/ErrorCodes.cs`. Controllers map the task with `ToResponse()`, `ToCreatedResponse()` or `ToNoContentOrError()` and never branch. Every failure body, including validation, auth, rate limiting and unhandled exceptions, is `{ "error": "<code>" }`, with the status per code in one table (`ErrorStatusCodes.cs`). The frontend maps codes to copy in `app/types/errors.ts`, and `errors-contract.spec.ts` parses the C# file so the two lists can't drift. Exceptions remain for genuinely unexpected failures and startup misconfiguration.

## Consequences

- Adding a failure means adding a code on both sides in the same PR; CI enforces it.
- This reversed an earlier decision not to centralise error codes (see the code quality backlog's second audit).
