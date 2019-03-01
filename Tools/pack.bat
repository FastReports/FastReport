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
SET "COUCH=FastReport.Data.Couchbase\FastReport.OpenSource.Data.Couchbase.csproj"
SET "MONGO=FastReport.Data.MongoDB\FastReport.OpenSource.Data.MongoDB.csproj"
SET "ORA=FastReport.Data.OracleODPCore\FastReport.OpenSource.Data.OracleODPCore.csproj"
SET "RAVEN=FastReport.Data.RavenDB\FastReport.OpenSource.Data.RavenDB.csproj"
SET "SQLITE=FastReport.Data.SQLite\FastReport.OpenSource.Data.SQLite.csproj"

SET "FR_SIMPLE_PDF=Extras\OpenSource\FastReport.OpenSource.Export.PdfSimple\FastReport.OpenSource.Export.PdfSimple\FastReport.OpenSource.Export.PdfSimple.csproj"

SET "OUTPUT=%~dp0..\..\fr_nuget"

SET "PROJECTS=!FR! !WEB! !DATA!\!POSTGRES! !DATA!\!MSSQL! !DATA!\!MYSQL! !DATA!\!JSON! !DATA!\!COUCH! !DATA!\!MONGO! !DATA!\!ORA! !DATA!\!RAVEN! !DATA!\!SQLITE! !FR_SIMPLE_PDF!" 


pushd %~dp0..

IF "!WITH_OUT_DEBUG!"=="false" (

  for %%x in (%PROJECTS%) do (
    dotnet restore %%x "!PROP!-debug"
  )

  for %%x in (%PROJECTS%) do (
    dotnet clean %%x -c Debug    "!PROP!-debug"
  )

  for %%x in (!PROJECTS!) do (
    dotnet pack  %%x -c Debug    "!PROP!-debug"  -o "!OUTPUT!" --include-symbols --include-source
  )

)

  for %%x in (!PROJECTS!) do (
    dotnet restore %%x "!PROP!"
  )

  for %%x in (!PROJECTS!) do (
    dotnet clean %%x -c Release  "!PROP!"
  )

  for %%x in (!PROJECTS!) do (
    dotnet pack  %%x -c Release  "!PROP!"  -o "!OUTPUT!"
  )

popd