@if "%_echo%" neq "on" echo off
setlocal

REM This script should always be called from StartWCFSelfHostedSvc.cmd.
REM If PSEXEC_PATH is set to the full path of the folder containing psexec.exe,
REM it will be used to start the WCF self-hosted service as the Local System account.
REM If PSEXEC_PATH is not set, it will start the service under the current user.

set _setuplog=%~dp0..\..\..\..\SelfHostedWcfServiceSetup.log
set _setupSemaphoreFile=%~dp0..\..\..\..\SelfHostedWcfServiceSemaphore.log

echo Preparing to launch the WCF self hosted service

if EXIST %_setupSemaphoreFile% del %_setupSemaphoreFile%

REM Build tools
echo Building the WCF Self hosted service...
call %~dp0BuildWCFSelfHostedService.cmd >>%_setuplog% 
SET __EXITCODE=%ERRORLEVEL%
if NOT [%__EXITCODE%]==[0] (
    echo ERROR: An error occurred while building WCF Self hosted Service. >>%_setuplog%
    goto :done
  )
  
echo Building the certificate generator ...
call %~dp0BuildCertUtil.cmd >>%_setuplog%
set __EXITCODE=%ERRORLEVEL%
if NOT [%__EXITCODE%]==[0] (
    echo ERROR: An error occurred while building the Certificate generator. >>%_setuplog%
    goto :done
  )

REM Config Certs
REM we need the direcotry to save the test.crl file. We are investigate a way to get rid of it
md c:\wcftest
REM Certificate configuration errors are all non fatal currently because we non cert tests will still pass
echo Generating certificates ...
%~dp0..\..\..\..\artifacts\bin\CertificateGenerator\Release\net471\CertificateGenerator.exe >>%_setuplog%
if NOT [%ERRORLEVEL%]==[0] (
    echo Warning: An error occurred while running certificate generator. >>%_setuplog%
  )

echo Configuring firewall...
call %~dp0OpenFirewallPorts.cmd >>%_setuplog%
if NOT [%ERRORLEVEL%]==[0] (
    echo Warning: An error occurred while running certificate generator. >>%_setuplog%
  )

echo Configuring Https ports...
powershell -NoProfile -ExecutionPolicy unrestricted %~dp0ConfigHttpsPort.ps1 >>%_setuplog%
if NOT [%ERRORLEVEL%]==[0] (
    echo Warning: An error occurred while configuration https port. >>%_setuplog%
  )

REM Write into our semaphore file to indicate our SelfHost test is running
echo Started Self host WCF service >>%_setupSemaphoreFile%

REM
REM Start the self hosted WCF Test Service
if [%PSEXEC_PATH%]==[] (
  echo Starting the WCF Self hosted service under the local user account. >>%_setuplog%
  REM This next call blocks until the service is terminated
  call %~dp0..\..\..\..\artifacts\bin\SelfHostedWcfService\Release\net471\SelfHostedWcfService.exe
  set __EXITCODE=%ERRORLEVEL%
  goto Cleanup
)

if exist %PSEXEC_PATH%\psexec.exe (
  echo Starting the WCF Self hosted service under the Local System account using: >>%_setuplog%
  echo call %PSEXEC_PATH%\psexec.exe -s -h %~dp0..\..\..\..\artifacts\bin\SelfHostedWcfService\Release\net471\SelfHostedWcfService.exe >>%_setuplog%
  REM This next call blocks until the service is terminated
  call %PSEXEC_PATH%\psexec.exe -s -h %~dp0..\..\..\..\artifacts\bin\SelfHostedWcfService\Release\net471\SelfHostedWcfService.exe
  set __EXITCODE=%ERRORLEVEL%
  goto Cleanup
)

echo %PSEXEC_PATH% is not a valid path to psexec.exe
set __EXITCODE=1

:Cleanup
echo Cleaning up after the Self hosted service has completed
REM Update our semaphore file to indicate our SelfHost test is cleaning up
echo Cleaning up after Self host WCF service terminated. >>%_setupSemaphoreFile%
call %~dp0CleanUpWCFSelfHostedSvc.cmd
:done

REM Deleting the semaphore file is an indication the cleanup script is done
REM and no longer writing to the setup log file.
if EXIST %_setupSemaphoreFile% del %_setupSemaphoreFile%

exit /b %__EXITCODE%
