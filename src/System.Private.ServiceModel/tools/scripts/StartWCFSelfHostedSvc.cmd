@if "%_echo%" neq "on" echo off
setlocal

REM Script return code
REM -1: Already running
REM 0: Started the test service

Echo Set Up test services
set __EXITCODE=0
set _setuplog=%~dp0..\..\..\..\SelfHostedWcfServiceSetup.log

echo Checking whether Self hosted WCF Test Service is already running. >%_setuplog%
    
REM If it's already running, we will not set up/clean up service
tasklist /FI "imagename eq SelfHostedWCFService.exe" | findstr /i /C:"SelfHostedWCFService.exe" 1>nul
if [%ERRORLEVEL%]==[0] (
    set __EXITCODE=-1
    echo Detected Self hosted WCF Test Service is already running. >>%_setuplog%
    goto :PingService
)

REM run the start wcf service elevated
echo Starting the Self hosted WCF test service elevated. >>%_setuplog%
start /D %~dp0 RunElevated.vbs StartWCFSelfHostedSvcDoWork.cmd

:PingService
REM the serial number is not used in the service side. Thus it can be any number
echo Waiting for Self hosted WCF service to respond to ping... >>%_setuplog%
call powershell -NoProfile -ExecutionPolicy unrestricted %~dp0pingWcfService.ps1 "http://localhost/TestHost.svc/Ping"
SET _ERRORLEVEL=%ERRORLEVEL%
if [%_ERRORLEVEL%] NEQ [0] (
	set __EXITCODE=%_ERRORLEVEL%
  echo Error: failed to ping Self hosted WCF Test Service. >>%_setuplog%
  goto :done
) else (
  echo Self hosted WCF Test Service responded OK to ping. >>%_setuplog%
)

:done

exit /b %__EXITCODE%
