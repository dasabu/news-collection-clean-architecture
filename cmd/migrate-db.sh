#!/bin/bash

set -e

if [ -z "$1" ]; then
  echo "❌ Error: Missing name for migration"
  echo "👉 Example: bash cmd/migrate.sh <MigrationName>"
  exit 1
fi

MIGRATION_NAME=$1

dotnet ef migrations add "$MIGRATION_NAME" \
  --project NewsCollection.Infrastructure \
  --startup-project NewsCollection.Api \
  --output-dir Data/Migrations