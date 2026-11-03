#!/bin/sh
set -e

if [ -z "$ConnectionStrings__Default" ]; then
  echo "ConnectionStrings__Default is not set." >&2
  exit 1
fi

exec /app/EfBundle --connection "$ConnectionStrings__Default"