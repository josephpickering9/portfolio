# 0004. No pagination; list endpoints return summary DTOs

- **Status:** Accepted
- **Date:** 2026-10

## Context

There are a few dozen projects. The frontend filters, derives the technology-type chips and builds the company timeline from the full list client-side. But `GET /project` returned full EF entities, including descriptions, every image and duplicated image objects, which made 190 KB of the homepage's 227 KB of HTML serialised data.

## Decision

Keep returning the whole list, but as `ProjectSummary` records carrying only what list views render plus a server-chosen `cardImageUrl`. The detail endpoint still returns the full `Project`. Image fallback order (card, then banner, then first desktop screenshot) lives on the entity as computed properties, so the frontend no longer repeats it.

## Consequences

- The homepage's server-rendered HTML went from 227 KB to 76 KB.
- Revisit pagination if the list grows by an order of magnitude; filtering would then need to move server-side.
- The Vertex AI ingest reads full projects from the repository rather than the list endpoint.
