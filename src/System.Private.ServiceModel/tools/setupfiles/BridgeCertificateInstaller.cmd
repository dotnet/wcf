@echo off
setlocal

REM 
REM This script invokes the BridgeCertificateInstaller.exe, which fetches the CA certificate from the Bridge
REM to the location specified on the command line
REM 
REM Note that this isn't needed on Windows to install the CA certs, as the BridgeClientCertificateManager can 
REM install this automatically; the tool is primarily used for Linux, but it's sometimes useful to be able to 
REM obtain the CA cert from the server directly 
REM 

pushd  %~dp0..\..\..\..
REM Locate any CoreRun.exe to be able to execute BridgeCertificateInstaller.exe
REM Because BridgeCertificateInstaller itself is a test project, we are guaranteed
REM it will contain CoreRun.exe even if no other tests have been built.
for /f "delims=" %%A in ('where /F /R bin\tests ScenarioTests.Common.dll') do set "CoreRunVar=%%~dpA\corerun.exe"
if '%CoreRunVar%' EQU '' (
  Echo Could not bootstrap Bridge to obtain Certificate Authority certificate because was unable to locate CoreRun.exe under %~dp0..\..\..\..\bin
  goto done
)

for /f "delims=" %%A in ('where /F /R bin BridgeCertificateInstaller.exe') do set "BootstrapVar=%%A"
if '%CoreRunVar%' EQU '' (
  Echo Could not bootstrap Bridge to obtain Certificate Authority certificate because was unable to locate BridgeCertificateInstaller.exe under %~dp0..\..\..\..\bin
  goto done
)

echo Bootstrapping Bridge using: %CoreRunVar% %BootstrapVar%
%CoreRunVar% %BootstrapVar% %*

:done
popd
exit /b
