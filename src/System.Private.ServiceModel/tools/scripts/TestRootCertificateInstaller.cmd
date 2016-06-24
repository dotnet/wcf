@echo off
setlocal

REM 
REM This script invokes the TestRootCertificateInstaller.exe, which fetches the CA certificate from the test service
REM to the location specified on the command line
REM 
REM Note that this isn't needed on Windows to install the CA certs, as the test client's CertificateManager can 
REM install this automatically; the tool is primarily used for Linux, but it's sometimes useful to be able to 
REM obtain the CA cert from the server directly 
REM 

pushd  %~dp0..\..\..\..
REM Locate any CoreRun.exe to be able to execute TestClientCertificateInstaller.exe
REM Because TestClientCertificateInstaller itself is a test project, we are guaranteed
REM it will contain CoreRun.exe even if no other tests have been built.
for /f "delims=" %%A in ('where /F /R . corerun.exe') do set "CoreRunVar=%%~dpA\corerun.exe"
if '%CoreRunVar%' EQU '' (
  echo Could not obtain Certificate Authority certificate because was unable to locate CoreRun.exe under %~dp0..\..\..\..\
  goto done
)

for /f "delims=" %%A in ('where /F /R bin TestRootCertificateInstaller.exe') do set "BootstrapVar=%%A"
if '%CoreRunVar%' EQU '' (
  echo Could not acquire the Test Root Certificate Authority certificate because was unable to locate TestClientCertificateInstaller.exe under %~dp0..\..\..\..\bin
  echo Try building all outerloop tests using 'build.cmd /p:WithCategories=OuterLoop /p:ServiceUri=[service-uri]' from the repo root'
  goto done
)

echo Bootstrapping Service using: %CoreRunVar% %BootstrapVar%
%CoreRunVar% %BootstrapVar% %*

:done
popd
exit /b
