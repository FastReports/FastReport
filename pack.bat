@ECHO OFF
setlocal ENABLEDELAYEDEXPANSION
rem HELP for this file
rem --version 1.0.0 for use version
rem --with-out-debug for build without debug configuration

SET "WITH_OUT_DEBUG=false"
SET "FRVERSION=2021.4.15"


for %%x in (%*) do (
  IF "!FRVERSION!" == "0.0.0" ( SET "FRVERSION=%%x" )
  IF [%%x] == [--version] ( SET "FRVERSION=0.0.0" ) 
)

dotnet run --project ".\Pack\BuildScripts\buildScript.csproj" --target=PackOpenSource --config=Release --solution-filename=FastReport.OpenSource.sln --out-dir=fr_packages --vers=%FRVERSION%
