#!/bin/bash

set -e

dotnet clean && dotnet build && cd NewsCollection.Api && dotnet run