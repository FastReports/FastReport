#!/bin/bash
set +v
# HELP for this file
# --version 1.0.0 for use version
# --with-out-debug for build without debug configuration
# created by Detrav

WITH_OUT_DEBUG=false
VERSION="1.0.0"

for var in "$@";
do
  if [ $VERSION = "0.0.0" ];
  then
      VERSION=$var
  fi
  if [ $var = "--with-out-debug" ];
  then
      WITH_OUT_DEBUG=true
  fi
  if [ $var = "--version" ];
  then
      VERSION="0.0.0"
  fi  
done

SCRIPTPATH="$( cd "$(dirname "$0")" ; pwd -P )"
PROP="/p:SolutionDir=$SCRIPTPATH/../;SolutionFileName=FastReport.OpenSource.sln;TargetFrameworks=\"netstandard2.0\";Version=$VERSION"
FR="FastReport.OpenSource/FastReport.OpenSource.csproj"
WEB="FastReport.Core.Web/FastReport.Web.csproj"
MSSQL="Extras/Core/FastReport.Data/FastReport.Data.MsSql/FastReport.OpenSource.Data.MsSql.csproj"
MYSQL="Extras/Core/FastReport.Data/FastReport.Data.MySql/FastReport.OpenSource.Data.MySql.csproj"

POSTGRES="Extras/Core/FastReport.Data/FastReport.Data.Postgres/FastReport.OpenSource.Data.Postgres.csproj"
JSON="Extras/Core/FastReport.Data/FastReport.Data.Json/FastReport.OpenSource.Data.Json.csproj"
COUCH="Extras/Core/FastReport.Data/FastReport.Data.Couchbase/FastReport.OpenSource.Data.Couchbase.csproj"
MONGO="Extras/Core/FastReport.Data/FastReport.Data.MongoDB/FastReport.OpenSource.Data.MongoDB.csproj"
ORA="Extras/Core/FastReport.Data/FastReport.Data.OracleODPCore/FastReport.OpenSource.Data.OracleODPCore.csproj"
RAVEN="Extras/Core/FastReport.Data/FastReport.Data.RavenDB/FastReport.OpenSource.Data.RavenDB.csproj"
SQLITE="Extras/Core/FastReport.Data/FastReport.Data.SQLite/FastReport.OpenSource.Data.SQLite.csproj"

if [ $WITH_OUT_DEBUG = false ];
then
  dotnet clean $SCRIPTPATH/../$FR     	-c Debug    "$PROP-debug" 
  dotnet clean $SCRIPTPATH/../$WEB    	-c Debug    "$PROP-debug"
  dotnet clean $SCRIPTPATH/../$MSSQL  	-c Debug    "$PROP-debug"
  dotnet clean $SCRIPTPATH/../$MYSQL  	-c Debug    "$PROP-debug" 
  dotnet clean $SCRIPTPATH/../$POSTGRES -c Debug    "$PROP-debug"
  dotnet clean $SCRIPTPATH/../$JSON  	-c Debug    "$PROP-debug"
  dotnet clean $SCRIPTPATH/../$COUCH  	-c Debug    "$PROP-debug"
  dotnet clean $SCRIPTPATH/../$MONGO  	-c Debug    "$PROP-debug"
  dotnet clean $SCRIPTPATH/../$ORA  	-c Debug    "$PROP-debug"
  dotnet clean $SCRIPTPATH/../$RAVEN  	-c Debug    "$PROP-debug"
  dotnet clean $SCRIPTPATH/../$SQLITE  	-c Debug    "$PROP-debug"

  dotnet pack  $SCRIPTPATH/../$FR     	-c Debug    "$PROP-debug"  -o ../../fr_nuget --include-symbols --include-source
  dotnet pack  $SCRIPTPATH/../$WEB    	-c Debug    "$PROP-debug"  -o ../../fr_nuget --include-symbols --include-source
  dotnet pack  $SCRIPTPATH/../$MSSQL  	-c Debug    "$PROP-debug"  -o ../../fr_nuget --include-symbols --include-source
  dotnet pack  $SCRIPTPATH/../$MYSQL  	-c Debug    "$PROP-debug"  -o ../../fr_nuget --include-symbols --include-source
  dotnet pack  $SCRIPTPATH/../$POSTGRES -c Debug    "$PROP-debug"  -o ../../fr_nuget --include-symbols --include-source
  dotnet pack  $SCRIPTPATH/../$JSON  	-c Debug    "$PROP-debug"  -o ../../fr_nuget --include-symbols --include-source
  dotnet pack  $SCRIPTPATH/../$COUCH  	-c Debug    "$PROP-debug"  -o ../../fr_nuget --include-symbols --include-source
  dotnet pack  $SCRIPTPATH/../$MONGO  	-c Debug    "$PROP-debug"  -o ../../fr_nuget --include-symbols --include-source
  dotnet pack  $SCRIPTPATH/../$ORA  	-c Debug    "$PROP-debug"  -o ../../fr_nuget --include-symbols --include-source
  dotnet pack  $SCRIPTPATH/../$RAVEN  	-c Debug    "$PROP-debug"  -o ../../fr_nuget --include-symbols --include-source
  dotnet pack  $SCRIPTPATH/../$SQLITE  	-c Debug    "$PROP-debug"  -o ../../fr_nuget --include-symbols --include-source
fi

dotnet clean $SCRIPTPATH/../$FR     	-c Release  "$PROP"
dotnet clean $SCRIPTPATH/../$WEB    	-c Release  "$PROP"
dotnet clean $SCRIPTPATH/../$MSSQL  	-c Release  "$PROP"
dotnet clean $SCRIPTPATH/../$MYSQL  	-c Release  "$PROP"
dotnet clean $SCRIPTPATH/../$POSTGRES  	-c Release  "$PROP"
dotnet clean $SCRIPTPATH/../$JSON  		-c Release  "$PROP"
dotnet clean $SCRIPTPATH/../$COUCH  	-c Release  "$PROP"
dotnet clean $SCRIPTPATH/../$MONGO  	-c Release  "$PROP"
dotnet clean $SCRIPTPATH/../$ORA  		-c Release  "$PROP"
dotnet clean $SCRIPTPATH/../$RAVEN  	-c Release  "$PROP"
dotnet clean $SCRIPTPATH/../$SQLITE  	-c Release  "$PROP"

dotnet pack  $SCRIPTPATH/../$FR     	-c Release  "$PROP"  -o ../../fr_nuget
dotnet pack  $SCRIPTPATH/../$WEB    	-c Release  "$PROP"  -o ../../fr_nuget
dotnet pack  $SCRIPTPATH/../$MSSQL  	-c Release  "$PROP"  -o ../../fr_nuget
dotnet pack  $SCRIPTPATH/../$MYSQL  	-c Release  "$PROP"  -o ../../fr_nuget
dotnet pack  $SCRIPTPATH/../$POSTGRES  	-c Release  "$PROP"  -o ../../fr_nuget
dotnet pack  $SCRIPTPATH/../$JSON  		-c Release  "$PROP"  -o ../../fr_nuget
dotnet pack  $SCRIPTPATH/../$COUCH  	-c Release  "$PROP"  -o ../../fr_nuget
dotnet pack  $SCRIPTPATH/../$MONGO  	-c Release  "$PROP"  -o ../../fr_nuget
dotnet pack  $SCRIPTPATH/../$ORA  		-c Release  "$PROP"  -o ../../fr_nuget
dotnet pack  $SCRIPTPATH/../$RAVEN  	-c Release  "$PROP"  -o ../../fr_nuget
dotnet pack  $SCRIPTPATH/../$SQLITE  	-c Release  "$PROP"  -o ../../fr_nuget
