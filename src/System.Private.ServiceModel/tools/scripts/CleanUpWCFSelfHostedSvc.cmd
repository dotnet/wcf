@if "%_echo%" neq "on" echo off
setlocal

set _cleanuplog=%~dp0..\..\..\..\SelfHostedWcfServiceCleanup.log

REM
echo Cleanup test services
REM  Note that we always return successful code in clean up routine because clean up errors are not fatal

REM We will open a separate elevated window to do clean up only if the current window is not elevated
REM This is to improve the debugging experience
net session >nul 2>&1
if NOT [%ERRORLEVEL%]==[0] (
    echo Upgrading to elevated...
    set runelvated=%~dp0RunElevated.vbs
)

echo Stopping SelfHostedWCFService.exe... 
call %runelvated% %~dp0StopWcfSelfHostedSvc.cmd
REM error 128 will be returned if The process not found
if NOT [%ERRORLEVEL%]==[0]  if NOT [%ERRORLEVEL%]==[128] (
	echo Warning: An error occurred while killing the SelfHostedWCFService.exe. >%_cleanuplog%
	)

If NOT exist %~dp0..\..\..\..\bin\Wcf\tools\CertificateGenerator\CertificateGenerator.exe (
  echo Building certificate generator...
	call %~dp0BuildCertUtil.cmd >>%_cleanuplog%
	set __EXITCODE=%ERRORLEVEL%
	if NOT [%__EXITCODE%]==[0] (
		echo Warning: An error occurred while building the Certificate generator. >>%_cleanuplog%
	 )
)

echo Cleaning up the Https ports...
call %runelvated% %~dp0CleanUpHttpsPort.cmd >>%_cleanuplog%
if NOT [%ERRORLEVEL%]==[0] (
	echo Warning: An error occurred while removing https port. >>%_cleanuplog%
	)

echo Removing firewall rules...
call %runelvated% %~dp0RemoveFirewallPorts.cmd >>%_cleanuplog%
if NOT [%ERRORLEVEL%]==[0] (
	echo Warning: An error occurred while removing Firewall ports. >>%_cleanuplog%
	)

echo Removing certificates...
call %runelvated% %~dp0..\..\..\..\bin\Wcf\tools\CertificateGenerator\CertificateGenerator.exe -Uninstall >>%_cleanuplog%
if NOT [%ERRORLEVEL%]==[0] (
	echo Warning: An error occurred while removing test certificates. >>%_cleanuplog%
	)

:done

exit /b 0
