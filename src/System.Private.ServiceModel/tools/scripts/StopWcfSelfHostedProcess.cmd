@if "%_echo%" neq "on" echo off
setlocal

REM This CMD forcibly stops the SelfHostedWCFService.exe process.
REM It is expected to be called elevated.

call taskkill.exe /F /IM SelfHostedWCFService.exe 

REM error 128 will be returned if The process not found
if NOT [%ERRORLEVEL%]==[0]  if NOT [%ERRORLEVEL%]==[128] (
	echo Warning: An error occurred while killing the SelfHostedWCFService.exe.
	exit /b 1
	)

exit /b 0
