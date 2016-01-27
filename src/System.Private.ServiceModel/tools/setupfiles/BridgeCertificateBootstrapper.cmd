echo off
setlocal

pushd  %~dp0..\..\..\..
REM Locate any CoreRun.exe to be able to execute BridgeCertificateBootstrapper.exe
REM Because BridgeCertificateBootstrapper itself is a test project, we are guaranteed
REM it will contain CoreRun.exe even if no other tests have been built.
for /f "delims=" %%A in ('where /F /R bin\tests ScenarioTests.Common.dll') do set "CoreRunVar=%%~dpA\corerun.exe"
if '%CoreRunVar%' EQU '' (
  Echo Could not bootstrap Bridge to obtain Certificate Authority certificate because was unable to locate CoreRun.exe under %~dp0..\..\..\..\bin
  goto done
)

for /f "delims=" %%A in ('where /F /R bin BridgeCertificateBootstrapper.exe') do set "BootstrapVar=%%A"
if '%CoreRunVar%' EQU '' (
  Echo Could not bootstrap Bridge to obtain Certificate Authority certificate because was unable to locate BridgeCertificateBootstrapper.exe under %~dp0..\..\..\..\bin
  goto done
)

echo Bootstrapping Bridge using: %CoreRunVar% %BootstrapVar%
%CoreRunVar% %BootstrapVar% %*

:done
popd
exit /b
