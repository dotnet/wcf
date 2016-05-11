@if "%_echo%" neq "on" echo off
setlocal

REM This CMD stops the Self hosted WCF test service.
REM It will self-elevate to kill the local process.

set _EXITCODE=0
set _cleanuplog=%~dp0..\..\..\..\SelfHostedWcfServiceCleanup.log
set _setupSemaphoreFile=%~dp0..\..\..\..\SelfHostedWcfServiceSemaphore.log
set _maxWaitIterations=30

REM Detect if the process is running locally
tasklist /FI "imagename eq SelfHostedWCFService.exe" | findstr /i /C:"SelfHostedWCFService.exe" 1>nul
if [%ERRORLEVEL%] == [0] goto stopService
 
REM If it's not running, exit with 0 to indicate nothing was done  
echo Detected Self hosted WCF Test Service is not running. >>%_cleanuplog%
goto done

:stopService

REM We will open a separate elevated window to do clean up only if the current window is not elevated
REM This is to improve the debugging experience
net session >nul 2>&1
if NOT [%ERRORLEVEL%]==[0] (
    set _runelevated=%~dp0RunElevated.vbs
)

echo Stopping SelfHostedWCFService.exe process... >>%_cleanuplog%

REM Note: when elevating, we don't redirect output to the _cleanuplog
REM due to contention for the log file.  Normal use is already elevated
REM and captures everything to the log file
if '%_runelevated%' == '' (
  call %~dp0StopWcfSelfHostedProcess.cmd >>%_cleanuplog%
) else (
  call %_runelevated% %~dp0StopWcfSelfHostedProcess.cmd >nul
)

if NOT [%ERRORLEVEL%]==[0] (
  _EXITCODE=%ERRORLEVEL%
  echo Warning: An error occurred attempting to stop the SelfHostedWCFService.exe. >>%_cleanuplog%
  goto done
	)
	
REM Terminating the Self host WCF process started with StartSelfHostWcfSvc.cmd
REM results in that script gaining control to finish cleanup.  If we are in that
REM state, we need to wait for that cleanup to finish, or the bin folder will still
REM be in use.

for /l %%x in (1, 1, %_maxWaitIterations%) do (
  echo Waiting for Self hosted WCF service cleanup to complete... >>%_cleanuplog%
  if NOT EXIST %_setupSemaphoreFile% (
    echo Self hosted WCF service cleanup has completed. >>%_cleanuplog%
    goto done
  )
  
  REM The "proper" way to wait is 'timeout' but that unconditionally
  REM exits the process with "Input redirection is not supported".
  REM The community preferred approach to wait is this.
  REM The count of '6' equates to 5 seconds.
  ping 127.0.0.1 -n 6 > nul

echo %%x

)

REM Timed out waiting for cleanup script to finish
echo Failed to finish cleanup for Self hosted WCF service
set _EXITCODE=1

:done

exit /b %_EXITCODE%
