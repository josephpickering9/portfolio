---
name: Portfolio Frontend
description: Dark, cinematic developer portfolio with a bespoke visual identity. daisyUI is removed; styling is Tailwind CSS v4 on a small set of CSS custom properties.
reference: Portfolio.dc.html (design source of truth)
---

# Portfolio Frontend

## Overview

**North star:** Dark and cinematic. A deep background with project mockups that glow, editorial serif headlines and monospace metadata. The work is shown on the home page itself, not behind navigation cards.

**Audience:** Recruiters scanning fast, plus freelance and contract clients. Engineers doing due diligence get depth from the case studies.

**Scope:** Every page, including the authenticated CMS (`new.vue`, `update.vue`, forms). daisyUI is removed completely, so the CMS is restyled with the same tokens and simple primitives (input, select, button, toggle, dialog, toast).

## Colour

Four themes, each defined as CSS custom properties on `[data-theme]` and exposed to Tailwind v4 through `@theme`. Never hardcode a colour in a component; always use a token.

| Token | Midnight (default) | Ember | Moss | Daylight |
|---|---|---|---|---|
| `--bg` | oklch(0.15 0.02 260) | oklch(0.15 0.012 50) | oklch(0.15 0.015 150) | oklch(0.97 0.006 85) |
| `--surface` | oklch(0.19 0.02 260) | oklch(0.19 0.014 50) | oklch(0.19 0.018 150) | oklch(0.995 0.003 85) |
| `--surface2` | oklch(0.23 0.02 260) | oklch(0.23 0.016 50) | oklch(0.23 0.02 150) | oklch(0.94 0.01 85) |
| `--line` | oklch(0.3 0.02 260) | oklch(0.3 0.016 50) | oklch(0.3 0.02 150) | oklch(0.87 0.01 85) |
| `--text` | oklch(0.96 0.01 260) | oklch(0.96 0.01 70) | oklch(0.96 0.01 140) | oklch(0.2 0.01 85) |
| `--muted` | oklch(0.74 0.02 260) | oklch(0.75 0.02 60) | oklch(0.75 0.02 145) | oklch(0.45 0.01 85) |
| `--accent` | oklch(0.68 0.21 25) | oklch(0.8 0.15 65) | oklch(0.87 0.17 128) | oklch(0.55 0.16 40) |
| `--accent-ink` | oklch(0.14 0.03 25) | oklch(0.2 0.04 50) | oklch(0.2 0.04 140) | oklch(0.99 0 0) |
| `--glow` | oklch(0.6 0.22 25 / 0.35) | oklch(0.7 0.16 50 / 0.35) | oklch(0.75 0.16 135 / 0.3) | oklch(0.6 0.12 40 / 0.22) |
| `--danger` | oklch(0.7 0.19 25) | oklch(0.7 0.19 25) | oklch(0.72 0.18 25) | oklch(0.52 0.2 25) |
| `--ok` | oklch(0.78 0.15 150) | oklch(0.78 0.15 150) | oklch(0.8 0.14 160) | oklch(0.5 0.13 150) |

- `--accent` is the only saturated colour. Use it for one highlight per view: the surname in the hero, section numbers, primary buttons, the active state.
- Text on `--accent` is always `--accent-ink`.
- `--danger` is for validation errors, destructive actions and error toasts; `--ok` only for success toasts. Never use either decoratively.
- `--glow` is only used in large, soft `box-shadow`s under mockups and in the hero's radial glow. It is never a fill.
- Midnight, Ember and Moss set `color-scheme: dark`; Daylight sets `light`.
- The theme switcher is four split swatches (background/accent). The choice persists via the existing `ThemeStore` (Pinia, persisted; also mirrored to a cookie so SSR can read it) and is applied as `data-theme` on `<html>` during SSR to avoid a flash. The CMS uses the same theme.

## Typography

Self-hosted in Nuxt (`@nuxt/fonts` with the local provider, or `.woff2` files in `public/fonts` with `@font-face` and `font-display: swap`). No Google Fonts requests at runtime.

| Role | Family | Weights | Use |
|---|---|---|---|
| Display | Instrument Serif | 400, 400 italic | H1, H2, company names, mobile menu items |
| Body / UI | Geist | 400, 500, 600 | Body copy, buttons, project titles (600) |
| Mono | Geist Mono | 400, 500 | Eyebrows, section numbers, dates, metadata |

Scale:
- Hero name: `clamp(56px, 10vw, 136px)`, line-height 0.9, letter-spacing −0.025em, one line, surname in `--accent`.
- Page H1: `clamp(48px, 7vw, 88px)`, line-height 0.95.
- Section H2: 44px serif. Project titles: 21–24px Geist 600.
- Lede: 19–20px, line-height 1.55, `--muted`, max-width ~640px.
- Eyebrow: Geist Mono 12–13px, uppercase, letter-spacing 0.06em.
- Body: 15–18px, line-height 1.55–1.65. Use `text-wrap: pretty` for paragraphs and `balance` for headings.

## Layout

- Content width: 1200px for every page (so content lines up with the header), except AI search at 820px. Gutters 28px.
- Long-form text (case-study overview, problem, decisions and outcome; company descriptions) is capped at 680px (~80 characters a line). Mockups, galleries, stack tables, diagrams and timelines use the full width.
- Fluid down to 360px. Grids use `repeat(auto-fit|auto-fill, minmax(min(100%, Npx), 1fr))`.
- Case-study rows: 180px mono label + content (flex-wrap, so they stack on mobile).
- Header: sticky, blurred `--bg` at 78% opacity, 1px `--line` bottom border. Below 860px the nav, swatches and CV collapse into a 44px menu button that opens a full-width panel.

## Shape and depth

- Radii: 8px (tags), 10–12px (inputs, small cards, diagram nodes), 14–16px (mockups, panels), 20px (feature card), 999px (pills, buttons, chips).
- Borders: 1px `--line`. Dashed `--line` for secondary or annotation boxes.
- Elevation is light, not shadow: mockups use `0 40px 90px -40px var(--glow)`; the case-study hero uses `0 60px 140px -50px var(--glow)`.

## Components

- **Browser mockup:** `--surface` frame, 3-dot bar, optional mono URL, 16:10 (cards) or 16:9 (case-study hero) screenshot, glow shadow.
- **Project card:** mockup, then mono number in `--accent`, title, one-line summary. Hover lifts by 6px (`cubic-bezier(.2,.8,.2,1)`, 350ms).
- **Feature card ("You're looking at it"):** `--surface` panel linking to this site's case study, with brand-icon tech tags.
- **AI prompt bar:** robot icon, input and accent "Ask" button in a `--surface` pill with glow. On the home page, submitting or tapping a suggested prompt **navigates to `/search?q=…`**, and the search page runs the query. Suggested prompts are 3 random items from the existing `allPrompts` list.
- **Chips:** pill, 1px `--line`, `--muted` text. Active: `--accent` fill, `--accent-ink` text.
- **Timeline (companies):** 1px `--line` rail, 13px accent-ringed dot with glow, circular logo, serif name, mono date range and duration, description, project pills plus "View all →". The logo fills a 48px `--surface2` circle (`object-fit: cover`) rather than sitting on a white disc; upload logos with transparent backgrounds so they take the theme colour.
- **Tech tile:** icon (simple-icons, falling back to heroicons/mdi) and name on `--surface`; hover adds an accent border and glow. Tiles are at least 240px wide so names and counts stay on one line; if they don't fit, the count wraps below the name rather than overlapping it.
- **Search field:** shared by Projects and Technologies (`ui/input/SearchField.vue`): `--surface`, radius 12px, magnifying-glass icon, sits above the type chips and combines with them. The term persists in the URL as `?q=`; a search with no results shows the serif "No … match." empty state with Clear filters.
- **Case study:** numbered sections `01 Problem`, `02 How it's built`, `03 Decisions`, `04 Stack`. One `--surface2` callout per case study, for the most important decision. Show stats only when real, interesting numbers exist.
- **Architecture diagram:** a row of nodes joined by mono `→` arrows (the primary node has an `--accent` border), plus dashed annotation boxes. Scrolls horizontally on narrow screens rather than wrapping.

## States

- **Loading:** skeleton blocks in the final layout (cards: 16:10 image block + 45% title bar + 85% line; timeline rows: 48px circle + title bar + two lines). `--surface`/`--surface2` fills, `pf-pulse` opacity 0.45 → 1 → 0.45 over 1.4s ease-in-out.
- **Load error:** `--surface` panel, warning icon in `--accent`, "Couldn't load projects. Check your connection and try again." and an accent Retry pill.
- **Empty results:** "No projects match." in serif 32px + outline "Clear filters" pill.
- **AI search thinking:** three 8px accent dots (`pf-dot` scale 0.6 → 1, 1.2s, 0.15s stagger) + "Searching my projects…" + three shimmer lines (92%, 78%, 64%). The API takes `25s, so this state must feel alive.
- **AI answer:** the answer is revealed progressively (`7 characters per 16ms) with an accent ▍ caret; markdown bold and bullet lists are rendered. Cited projects appear as image cards once the reveal finishes; deduplicate citations by project slug.
- **AI error:** "AI search is unavailable right now." + Try again (accent) + Browse projects (outline).
- **Form validation:** errors appear on save, then clear per field as it's edited. Field border becomes `--danger`, 13px `--danger` message below, and a 7px `--danger` dot on the tab that contains the error. Saving with errors jumps to that tab.
- **Toasts:** bottom-centre pill, `--surface2`, 1px `--ok` or `--danger` border, icon + message, auto-dismiss after 3.2s.
- **Confirm dialog:** 60% black scrim with 4px blur, 440px `--surface` card, serif 30px title ("Delete this project?"), "This can't be undone.", Cancel (outline) + Delete (`--danger` fill).
- **404:** eyebrow "Error 404", hero-size serif "Page not found" with "found" in `--accent`, then Back to home / All projects / Ask AI Search.
- **Login:** centred 420px card: avatar, "Sign in", "Manage projects, companies and technologies.", full-width "Continue with Auth0" accent button, "← Back to site".
- **Focus:** `:focus-visible` → 2px `--accent` outline, 3px offset.

## Content rules

- Projects are listed newest first (API order, by start date). The home page pins four featured projects: Pulse, AI Chatbot, Data Extractor, Asurafin Website.
- Tag `customColour` stays in the API but is not shown in the UI; icons render in `currentColor`.
- CMS rich text renders with paragraph margin 0, 14px block gap, lists with 20px indent and 10px item gap, `<strong>` in `--text`/600 on a `--muted` body.

## Icons

`@nuxt/icon` with the installed `simple-icons`, `mdi` and `heroicons` collections. Real brand logos only; never hand-drawn approximations.

## Motion

- One authored moment: project cards lift on hover.
- Theme change: 400ms transition on background and text colour.
- AI search compacting: when a question is asked on `/search` (or New search restores the hero), the title, search field, prompts and answer card glide to their new positions with a same-document View Transition, 250ms `--ease-lift`; the robot icon and lede fade. Transform and opacity only. Browsers without View Transitions change instantly.
- Keep `pageTransition` (`page`, `out-in`).
- Respect `prefers-reduced-motion`: no lift, no transitions.

## Do / Don't

- Do use tokens for every colour; no Tailwind palette colours (`text-blue-500`), no daisyUI classes.
- Do keep `--accent` scarce.
- Do show the work on the home page.
- Don't use gradient washes or purple/indigo; the hero glow is a single soft radial in `--glow`.
- Don't fabricate stats or tags.
- Don't style the case-study narrative as three identical cards.

## Decisions log

| Date | Decision | Rationale |
|---|---|---|
| 2026-09-15 | Kept daisyUI, differentiated through content structure | Earlier bespoke directions read as generic or try-hard |
| 2026-10-16 | Removed daisyUI completely; bespoke dark, cinematic system with 4 themes | The site needs to impress recruiters and clients; content structure alone wasn't enough |
| 2026-10-16 | Instrument Serif / Geist / Geist Mono, self-hosted | Distinct editorial voice; no third-party font requests |
| 2026-10-16 | Default accent red; tag-type colours dropped in favour of a single accent | Cohesive look across themes |
| 2026-10-16 | Home hero AI search navigates to `/search` | Keeps Vertex AI calls on the search page only |
| 2026-10-16 | "How this site is built" lives in the portfolio's own case study | The site is itself a portfolio project |
| 2026-10-16 | Company logos fill a `--surface2` circle instead of sitting on near-white | The white discs were the brightest thing on the dark themes; filling the circle also stops square logo artwork showing as a box inside it |
| 2026-10-16 | Case study and Companies widened from 1040px to 1200px; prose capped at 680px | Left edges now line up with the header and other pages, screenshots get more room, and lines are shorter than before (~80 vs ~95 characters) |
| 2026-10-16 | Technologies page gets a text search (not in the original handoff); 4-column tile grid | 49+ technologies across 8 types are slow to scan; matches the Projects controls. Wider tiles stop names wrapping |
| 2026-10-17 | AI search header compacts once a question is asked, animated with a View Transition | The full hero pushed the answer below the fold; the 200px jump of the field you just typed in needed spatial continuity. Not a slide: it's the same page tightening, not a new screen |
