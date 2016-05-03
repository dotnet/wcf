@if "%_echo%" neq "on" echo off
setlocal

set _cleanuplog=%~dp0..\..\..\..\SelfHostedWcfServiceCleanup.log

echo Stopping SelfHostedWCFService.exe process... 
call taskkill.exe /F /IM SelfHostedWCFService.exe >>%_cleanuplog%
REM error 128 will be returned if The process not found
if NOT [%ERRORLEVEL%]==[0]  if NOT [%ERRORLEVEL%]==[128] (
	echo Warning: An error occurred while killing the SelfHostedWCFService.exe. >>%_cleanuplog%
	)

:done

exit /b 0
