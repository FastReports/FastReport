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

if [ $WITH_OUT_DEBUG = false ];
then
  dotnet clean $SCRIPTPATH/../$FR     -c Debug    "$PROP-debug" 
  dotnet clean $SCRIPTPATH/../$WEB    -c Debug    "$PROP-debug"

  dotnet pack  $SCRIPTPATH/../$FR     -c Debug    "$PROP-debug"  -o ../../fr_nuget    --include-symbols --include-source
  dotnet pack  $SCRIPTPATH/../$WEB    -c Debug    "$PROP-debug"  -o ../../fr_nuget --include-symbols --include-source
fi

dotnet clean $SCRIPTPATH/../$FR     -c Release  "$PROP"
dotnet clean $SCRIPTPATH/../$WEB    -c Release  "$PROP"

dotnet pack  $SCRIPTPATH/../$FR     -c Release  "$PROP"  -o ../../fr_nuget
dotnet pack  $SCRIPTPATH/../$WEB    -c Release  "$PROP"  -o ../../fr_nuget
