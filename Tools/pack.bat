@ECHO OFF
setlocal ENABLEDELAYEDEXPANSION
rem HELP for this file
rem --version 1.0.0 for use version
rem --with-out-debug for build without debug configuration
rem created by Detrav

SET "WITH_OUT_DEBUG=false"
SET "VERSION=1.0.0"


for %%x in (%*) do (
  IF "!VERSION!" == "0.0.0" ( SET "VERSION=%%x" )
  IF [%%x] == [--with-out-debug] ( SET "WITH_OUT_DEBUG=true" )
  IF [%%x] == [--version] ( SET "VERSION=0.0.0" ) 
)

SET "PROP=/p:SolutionDir=%~dp0..\;SolutionFileName=FastReport.OpenSource.sln;Version=!VERSION!"
SET "FR=FastReport.OpenSource\FastReport.OpenSource.csproj"
SET "WEB=FastReport.Core.Web\FastReport.Web.csproj"

pushd %~dp0..

IF "!WITH_OUT_DEBUG!"=="false" (
dotnet clean !FR!     -c Debug    "!PROP!-debug" 
dotnet clean !WEB!    -c Debug    "!PROP!-debug"

dotnet pack  !FR!     -c Debug    "!PROP!-debug"  -o ../../fr_nuget    --include-symbols --include-source
dotnet pack  !WEB!    -c Debug    "!PROP!-debug"  -o ../../fr_nuget --include-symbols --include-source
)

dotnet clean !FR!     -c Release  "!PROP!"
dotnet clean !WEB!    -c Release  "!PROP!"

dotnet pack  !FR!     -c Release  "!PROP!"  -o ../../fr_nuget
dotnet pack  !WEB!    -c Release  "!PROP!"  -o ../../fr_nuget

popd