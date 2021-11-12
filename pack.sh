#!/bin/bash
set +v
# HELP for this file
# --version 1.0.0 for use version

WITH_OUT_DEBUG=false
FRVERSION="2021.4.14"

dotnet run --project "./Pack/BuildScripts/buildScript.csproj" --target=PackOpenSource --config=Release --solution-filename=FastReport.OpenSource.sln --vers=$FRVERSION