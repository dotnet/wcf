@if "%_echo%" neq "on" echo off
setlocal

REM Script return code
REM -1: Already running
REM 0: Started the test service

Echo Set Up test services
set __EXITCODE=0

REM If it's already running, we will not set up/clean up service
tasklist /FI "imagename eq SelfHostedWCFService.exe" | findstr /i /C:"SelfHostedWCFService.exe" 1>nul
if [%ERRORLEVEL%]==[0] (
    set __EXITCODE=-1
    goto :PingService
)

REM run the start wcf service elevated
start /D %~dp0 RunElevated.vbs StartWCFSelfHostedSvcDoWork.cmd

:PingService
REM the serial number is not used in the service side. Thus it can be any number
call powershell -NoProfile -ExecutionPolicy unrestricted %~dp0pingWcfService.ps1 "http://localhost/CrlService.svc/GetCrl?serialNum=b52"
SET _ERRORLEVEL=%ERRORLEVEL%
if [%_ERRORLEVEL%] NEQ [0] (
    echo Error: failed to ping WCF Test Service.
	set __EXITCODE=%_ERRORLEVEL%
    goto :done
)

:done

exit /b %__EXITCODE%
