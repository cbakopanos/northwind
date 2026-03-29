#!/usr/bin/env bash
set -euo pipefail

CONTAINER_NAME="northwind-postgres"
IMAGE="postgres:17"
POSTGRES_USER="postgres"
POSTGRES_PASSWORD="postgres"
POSTGRES_DB="northwind"
HOST_PORT="5440"

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
INIT_SQL="$SCRIPT_DIR/init.sql"
SEED_SQL="$SCRIPT_DIR/seed.sql"

if docker ps -a --format '{{.Names}}' | grep -Fxq "$CONTAINER_NAME"; then
  echo "Error: container '$CONTAINER_NAME' already exists."
  echo "Please delete it manually and run this script again."
  exit 1
fi

docker run -d \
  --name "$CONTAINER_NAME" \
  -e POSTGRES_USER="$POSTGRES_USER" \
  -e POSTGRES_PASSWORD="$POSTGRES_PASSWORD" \
  -e POSTGRES_DB="$POSTGRES_DB" \
  -p "$HOST_PORT:5432" \
  "$IMAGE" >/dev/null

echo "Waiting for PostgreSQL to be ready..."
until docker exec "$CONTAINER_NAME" pg_isready -U "$POSTGRES_USER" -d "$POSTGRES_DB" >/dev/null 2>&1; do
  sleep 1
done

echo "Running init.sql..."
docker exec -i "$CONTAINER_NAME" psql -v ON_ERROR_STOP=1 -U "$POSTGRES_USER" -d "$POSTGRES_DB" < "$INIT_SQL"

echo "Running seed.sql..."
docker exec -i "$CONTAINER_NAME" psql -v ON_ERROR_STOP=1 -U "$POSTGRES_USER" -d "$POSTGRES_DB" < "$SEED_SQL"

echo "Done. Database '$POSTGRES_DB' is ready in container '$CONTAINER_NAME'."
