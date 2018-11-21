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

rem Directories:
SET "PROP=/p:SolutionDir=%~dp0..\;SolutionFileName=FastReport.OpenSource.sln;Version=!VERSION!"
SET "FR=FastReport.OpenSource\FastReport.OpenSource.csproj"
SET "WEB=FastReport.Core.Web\FastReport.Web.csproj"
SET "DATA=Extras\Core\FastReport.Data"

rem Connections
SET "POSTGRES=FastReport.Data.Postgres\FastReport.OpenSource.Data.Postgres.csproj"
SET "MSSQL=FastReport.Data.MsSql\FastReport.OpenSource.Data.MsSql.csproj"
SET "MYSQL=FastReport.Data.MySql\FastReport.OpenSource.Data.MySql.csproj"
SET "JSON=FastReport.Data.Json\FastReport.OpenSource.Data.Json.csproj"

SET "OUTPUT=%~dp0..\..\fr_nuget"

SET "PROJECTS=!FR! !WEB! !DATA!\!POSTGRES! !DATA!\!MSSQL! !DATA!\!MYSQL! !DATA!\!JSON!"


pushd %~dp0..

IF "!WITH_OUT_DEBUG!"=="false" (

  for %%x in (%PROJECTS%) do (
    dotnet clean %%x -c Debug    "!PROP!-debug"
  )

  for %%x in (!PROJECTS!) do (
    dotnet pack  %%x -c Debug    "!PROP!-debug"  -o "!OUTPUT!" --include-symbols --include-source
  )

)

  for %%x in (!PROJECTS!) do (
    dotnet clean %%x -c Release  "!PROP!"
  )

  for %%x in (!PROJECTS!) do (
    dotnet pack  %%x -c Release  "!PROP!"  -o "!OUTPUT!"
  )

popd