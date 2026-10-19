#!/usr/bin/env bash
# Activates an uploaded API release on the server: writes its configuration, applies migrations,
# switches the `current` symlink, restarts the systemd service and health-checks it. If the new
# release never reports healthy, the symlink is pointed back at the previous release and that is
# restarted instead. Migrations are not rolled back, so they must stay backwards compatible.
#
# Called by .github/workflows/api-deploy.yml with the release paths and secrets in the environment.
set -euo pipefail

: "${RELEASE_PATH:?}" "${ACTIVE_RELEASE_PATH:?}" "${BASE_PATH:?}" "${ROOT_PATH:?}"
: "${DATABASE_CONNECTION_STRING:?}" "${VERTEX_AI_CREDENTIALS_JSON:?Vertex AI credential secret is missing or empty}"

SERVICE=portfolio-api
UNIT_PATH="/etc/systemd/system/$SERVICE.service"
LEGACY_PORT=5000
API_URL="http://localhost:$LEGACY_PORT"
HEALTH_URL="$API_URL/health"
HEALTH_ATTEMPTS=30
FRONTEND_ORIGIN=https://josephpickering.co.uk
DOTNET_ROOT=/usr/share/dotnet
APP_PATH="$RELEASE_PATH/Portfolio.Api"
BIN_PATH="$APP_PATH/bin/Release/net10.0/publish"
LOG_PATH="$BASE_PATH/logs"

export DOTNET_ROOT PATH="$PATH:$DOTNET_ROOT"

exec 9>/tmp/portfolio-deploy.lock
flock -x -w 1500 9 || { echo "Timed out waiting for the deploy lock." >&2; exit 1; }

wait_until_healthy() {
  for _ in $(seq 1 "$HEALTH_ATTEMPTS"); do
    curl -sf -o /dev/null "$HEALTH_URL" && return 0
    sleep 2
  done
  return 1
}

write_config() {
  local service_account="$APP_PATH/service-account.json"
  echo "$VERTEX_AI_CREDENTIALS_JSON" | base64 -d > "$service_account"
  chmod 600 "$service_account"

  cat > "$BIN_PATH/appsettings.json" << EOF
{
  "ConnectionStrings": {
    "DefaultConnection": "$DATABASE_CONNECTION_STRING"
  },
  "Auth0": {
    "Domain": "$AUTH0_DOMAIN",
    "ClientId": "$AUTH0_CLIENT_ID",
    "Audience": "$AUTH0_AUDIENCE"
  },
  "Cors": {
    "AllowedOrigins": ["$FRONTEND_ORIGIN"]
  },
  "VertexAi": {
    "ProjectId": "$VERTEX_AI_PROJECT_ID",
    "Location": "$VERTEX_AI_LOCATION",
    "Collection": "$VERTEX_AI_COLLECTION",
    "Branch": "$VERTEX_AI_BRANCH",
    "Environment": "$VERTEX_AI_ENVIRONMENT",
    "Model": "$VERTEX_AI_MODEL",
    "ModelLocation": "$VERTEX_AI_MODEL_LOCATION",
    "QueryDataStoreSuffix": "$VERTEX_AI_QUERY_DATASTORE_SUFFIX",
    "CredentialsFile": "$service_account"
  }
}
EOF
  chmod 600 "$BIN_PATH/appsettings.json"
}

install_service() {
  mkdir -p "$LOG_PATH"
  cat > "$UNIT_PATH.new" << EOF
[Unit]
Description=Portfolio API
After=network.target

[Service]
WorkingDirectory=$ACTIVE_RELEASE_PATH
ExecStart=$DOTNET_ROOT/dotnet $ACTIVE_RELEASE_PATH/Portfolio.Api.dll
Environment=ASPNETCORE_URLS=$API_URL
Environment=DOTNET_ROOT=$DOTNET_ROOT
Restart=always
RestartSec=5
KillSignal=SIGINT
StandardOutput=append:$LOG_PATH/api.log
StandardError=append:$LOG_PATH/api.log

[Install]
WantedBy=multi-user.target
EOF

  if ! cmp -s "$UNIT_PATH.new" "$UNIT_PATH"; then
    mv "$UNIT_PATH.new" "$UNIT_PATH"
    systemctl daemon-reload
  else
    rm "$UNIT_PATH.new"
  fi
  systemctl enable --quiet "$SERVICE"
}

# Before the systemd unit existed the API ran under nohup; stop that instance so the port is free.
stop_legacy_instance() {
  systemctl is-active --quiet "$SERVICE" && return 0
  local pid
  pid=$(lsof -t -i:"$LEGACY_PORT" -sTCP:LISTEN 2>/dev/null | head -n1 || true)
  if [ -n "$pid" ]; then
    kill "$pid" || true
    sleep 2
  fi
}

activate() {
  ln -sfn "$1" "$ACTIVE_RELEASE_PATH"
  ln -sfn "$ROOT_PATH" "$1/wwwroot"
  systemctl restart "$SERVICE"
}

previous_release=$(readlink -f "$ACTIVE_RELEASE_PATH" 2>/dev/null || true)

write_config
chmod +x "$RELEASE_PATH/efbundle"
"$RELEASE_PATH/efbundle" --connection "$DATABASE_CONNECTION_STRING"

install_service
stop_legacy_instance
activate "$BIN_PATH"

if wait_until_healthy; then
  echo "Release $(basename "$RELEASE_PATH") is healthy."
  exit 0
fi

echo "::error::The new release did not become healthy; rolling back." >&2
tail -n 50 "$LOG_PATH/api.log" >&2 || true

if [ -n "$previous_release" ] && [ -d "$previous_release" ] && [ "$previous_release" != "$BIN_PATH" ]; then
  activate "$previous_release"
  wait_until_healthy && echo "Rolled back to $previous_release." >&2 || echo "::error::The previous release is unhealthy too." >&2
fi
exit 1
