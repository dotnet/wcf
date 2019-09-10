@echo off
REM This script uninstalls the current global version of dotnet-svcutil and installs the version passed in as a parameter.

if "%1"=="" (
  echo Please specify the verison of dotnet-svcutil to install.
  echo Current packages in bin\debug are:
  dir bin\Debug | findstr /l "dotnet-svcutil."
) else (
  dotnet tool uninstall -g dotnet-svcutil
  dotnet tool install -g --add-source bin\debug dotnet-svcutil --version %1
 )