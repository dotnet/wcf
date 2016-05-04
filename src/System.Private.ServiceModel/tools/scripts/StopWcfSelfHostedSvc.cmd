@if "%_echo%" neq "on" echo off
setlocal

REM This CMD stops the Self hosted WCF test service.
REM It will self-elevate to kill the local process.

set _cleanuplog=%~dp0..\..\..\..\SelfHostedWcfServiceCleanup.log

REM We will open a separate elevated window to do clean up only if the current window is not elevated
REM This is to improve the debugging experience
net session >nul 2>&1
if NOT [%ERRORLEVEL%]==[0] (
    set _runelevated=%~dp0RunElevated.vbs
)

echo Stopping SelfHostedWCFService.exe process. >>%_cleanuplog%

REM Note: when elevating, we don't redirect output to the _cleanuplog
REM due to contention for the log file.  Normal use is already elevated
REM and captures everything to the log file
if '%_runelevated%' == '' (
  call %~dp0StopWcfSelfHostedProcess.cmd >>%_cleanuplog%
) else (
  call %_runelevated% %~dp0StopWcfSelfHostedProcess.cmd >nul
)

if NOT [%ERRORLEVEL%]==[0] (
	echo Warning: An error occurred while killing the SelfHostedWCFService.exe. >>%_cleanuplog%
	exit /b 1
	)
	
exit /b 0


