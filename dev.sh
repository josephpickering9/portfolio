#!/usr/bin/env bash
# Runs the backend API and the frontend dev server together. Ctrl+C stops both.
#
# Usage: ./dev.sh [--docker] [--migrate] [--codegen]
#   --docker   start the Postgres container from the backend's docker-compose.yml first
#   --migrate  run `dotnet ef database update` before starting the API
#   --codegen  regenerate the frontend API client once the API is up
set -euo pipefail

ROOT="$(cd "$(dirname "$0")" && pwd)"
BE_DIR="$ROOT/api/WorkExperienceSearch"
FE_DIR="$ROOT/web"
API_URL="http://localhost:5105"

DOCKER=false MIGRATE=false CODEGEN=false
for arg in "$@"; do
  case "$arg" in
    --docker) DOCKER=true ;;
    --migrate) MIGRATE=true ;;
    --codegen) CODEGEN=true ;;
    -h|--help) sed -n '2,7p' "$0" | sed 's/^# \{0,1\}//'; exit 0 ;;
    *) echo "Unknown option: $arg" >&2; exit 1 ;;
  esac
done

# Prefix each line of a process's output so the two logs stay readable when interleaved.
prefix() { sed -u "s/^/$1 /"; }

cleanup() {
  trap - INT TERM EXIT
  echo -e "\nStopping..."
  kill 0 2>/dev/null || true
}

for port in 5105 3000; do
  pids=$(lsof -tiTCP:"$port" -sTCP:LISTEN 2>/dev/null || true)
  if [ -n "$pids" ]; then
    echo "Port $port in use — killing PID(s) $(echo $pids)"
    kill $pids 2>/dev/null || true
    for _ in 1 2 3 4 5; do
      lsof -tiTCP:"$port" -sTCP:LISTEN >/dev/null 2>&1 || break
      sleep 1
    done
    lsof -tiTCP:"$port" -sTCP:LISTEN >/dev/null 2>&1 && kill -9 $pids 2>/dev/null || true
  fi
done

if $DOCKER; then
  docker compose -f "$BE_DIR/docker-compose.yml" up -d
fi

if $MIGRATE; then
  dotnet ef database update --project "$BE_DIR"
fi

if [ ! -d "$FE_DIR/node_modules" ]; then
  (cd "$FE_DIR" && yarn install)
fi

trap cleanup INT TERM EXIT

(dotnet run --project "$BE_DIR" --launch-profile http 2>&1 | prefix $'\e[34m[api]\e[0m') &
API_PID=$!

echo "Waiting for API at $API_URL..."
until curl -sf -o /dev/null "$API_URL/swagger/v1/swagger.json"; do
  if ! kill -0 "$API_PID" 2>/dev/null; then
    echo "API exited before it came up — see [api] output above." >&2
    exit 1
  fi
  sleep 1
done
echo "API is up — Swagger at $API_URL/swagger"

if $CODEGEN; then
  (cd "$FE_DIR" && yarn codegen)
fi

(cd "$FE_DIR" && yarn dev 2>&1 | prefix $'\e[32m[web]\e[0m') &

wait
