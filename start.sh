#!/usr/bin/env bash
set -euo pipefail

PORT="${PORT:-5000}"
SERVICE="${SERVICE:-}"

# Railway may set RAILWAY_SERVICE_NAME or RAILWAY_SERVICE
if [[ -n "${RAILWAY_SERVICE_NAME:-}" ]]; then SERVICE="$RAILWAY_SERVICE_NAME"; fi
if [[ -n "${RAILWAY_SERVICE:-}" ]]; then SERVICE="$RAILWAY_SERVICE"; fi

# Fallback detection
if [[ -z "$SERVICE" ]]; then
  if [[ -f "Northwind.Api/src/Northwind.Api/Northwind.Api.csproj" ]]; then
    SERVICE="api"
  elif [[ -f "Northwind.Web/package.json" ]]; then
    SERVICE="web"
  fi
fi

echo "Selected service: $SERVICE"

if [[ "$SERVICE" == "api" ]]; then
  cd Northwind.Api
  echo "Restoring and publishing .NET API..."
  dotnet restore Northwind.slnx
  dotnet publish src/Northwind.Api/Northwind.Api.csproj -c Release -o /app/out
  echo "Starting API on port $PORT"
  ASPNETCORE_URLS="http://+:$PORT" dotnet /app/out/Northwind.Api.dll

elif [[ "$SERVICE" == "web" ]]; then
  cd Northwind.Web
  echo "Installing and building web app..."
  npm ci
  npm run build
  echo "Serving built site on port $PORT"
  npx serve -s dist -l "$PORT"

else
  echo "Could not determine service to start. Set SERVICE=api or SERVICE=web environment variable."
  exit 1
fi
