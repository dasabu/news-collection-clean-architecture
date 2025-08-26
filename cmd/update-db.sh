#!/bin/bash

set -e

dotnet ef database update \
  --project NewsCollection.Infrastructure \
  --startup-project NewsCollection.Api