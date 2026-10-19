#!/usr/bin/env bash
# Activates an uploaded frontend release on the server: writes its .env, switches the `current`
# symlink, (re)starts it under PM2 and health-checks it. If the new release never responds, the
# symlink is pointed back at the previous release and that is started instead.
#
# Called by .github/workflows/web-deploy.yml with the release paths and NUXT_ENV in the environment.
set -euo pipefail

: "${RELEASE_PATH:?}" "${ACTIVE_RELEASE_PATH:?}" "${NUXT_ENV:?}"

APP_NAME=PortfolioWeb
LEGACY_APP_NAME=WorkExperienceWeb
HEALTH_URL=http://localhost:3000/
HEALTH_ATTEMPTS=30

exec 9>/tmp/portfolio-deploy.lock
flock -x -w 1500 9 || { echo "Timed out waiting for the deploy lock." >&2; exit 1; }

export NVM_DIR="$HOME/.nvm"
# shellcheck source=/dev/null
[ -s "$NVM_DIR/nvm.sh" ] && . "$NVM_DIR/nvm.sh"
nvm use 22 > /dev/null

wait_until_healthy() {
  for _ in $(seq 1 "$HEALTH_ATTEMPTS"); do
    curl -sf -o /dev/null "$HEALTH_URL" && return 0
    sleep 2
  done
  return 1
}

start() {
  ln -sfn "$1" "$ACTIVE_RELEASE_PATH"
  pm2 delete "$APP_NAME" > /dev/null 2>&1 || true
  pm2 delete "$LEGACY_APP_NAME" > /dev/null 2>&1 || true
  (cd "$ACTIVE_RELEASE_PATH" && pm2 start ecosystem.config.js)
  pm2 save > /dev/null
}

previous_release=$(readlink -f "$ACTIVE_RELEASE_PATH" 2>/dev/null || true)

printf "%s" "$NUXT_ENV" > "$RELEASE_PATH/.env"
chmod 600 "$RELEASE_PATH/.env"
start "$RELEASE_PATH"

if wait_until_healthy; then
  echo "Release $(basename "$RELEASE_PATH") is healthy."
  exit 0
fi

echo "::error::The new release did not become healthy; rolling back." >&2
pm2 logs "$APP_NAME" --lines 50 --nostream >&2 || true

if [ -n "$previous_release" ] && [ -d "$previous_release" ] && [ "$previous_release" != "$(readlink -f "$RELEASE_PATH")" ]; then
  start "$previous_release"
  wait_until_healthy && echo "Rolled back to $previous_release." >&2 || echo "::error::The previous release is unhealthy too." >&2
fi
exit 1
