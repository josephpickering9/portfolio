# 0007. One droplet with scripted releases, not containers

- **Status:** Accepted
- **Date:** 2026-10

## Context

The site is small, has one environment and runs on a single DigitalOcean droplet next to its PostgreSQL. The original deploy killed whatever held the API port and restarted it with `nohup`: there was no restart on crash or reboot, no health check and no way back from a bad release.

## Decision

Keep the droplet and the symlinked `releases/<sha>` layout, but deploy only after tests pass and activate each release with a committed script (`api/deploy/activate.sh`, `web/deploy/activate.sh`). The API runs as a systemd service the script installs. Both scripts poll a health endpoint and, if the new release never comes up, switch the symlink back and restart the previous release. An unused, broken Dockerfile was deleted rather than kept around.

## Consequences

- Deploys still have a few seconds of downtime while the process restarts; zero-downtime would need two instances behind nginx.
- Migrations aren't rolled back, so they have to stay compatible with the previous release.
- nginx config isn't in the repo; the required settings are documented in `docs/deploy/README.md`.
