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
    set _runelevated=%~dp0RunElevated.vbs
)

REM Stop the Self hosted WCF services.
REM The CMD we call self-elevates and logs its results to %_cleanuplog%
REM Errors stopping the service are logged but do not stop processing
call %~dp0StopWcfSelfHostedSvc.cmd

If NOT exist %~dp0..\..\..\..\bin\CertificateGenerator\Release\net471\CertificateGenerator.exe (
  echo Building certificate generator...
	call %~dp0BuildCertUtil.cmd >>%_cleanuplog%
	set __EXITCODE=%ERRORLEVEL%
	if NOT [%__EXITCODE%]==[0] (
		echo Warning: An error occurred while building the Certificate generator. >>%_cleanuplog%
	 )
)

REM Note: when elevating, we don't redirect output to the _cleanuplog
REM due to contention for the log file.  Normal use is already elevated
REM and captures everything to the log file

echo Cleaning up the Https port. >>%_cleanuplog%
if '%_runelevated%' == '' (
  call %~dp0CleanUpHttpsPort.cmd >>%_cleanuplog%
) else (
  call %_runelevated% %~dp0CleanUpHttpsPort.cmd >nul
)
if NOT [%ERRORLEVEL%]==[0] (
	echo Warning: An error occurred while removing https port. >>%_cleanuplog%
	)

echo Removing firewall rules. >>%_cleanuplog%
if '%_runelevated%' == '' (
  call %~dp0RemoveFirewallPorts.cmd >>%_cleanuplog%
) else (
  call %_runelevated% %~dp0RemoveFirewallPorts.cmd >nul
)
if NOT [%ERRORLEVEL%]==[0] (
	echo Warning: An error occurred while removing Firewall ports. >>%_cleanuplog%
	)

echo Removing certificates. >>%_cleanuplog%
if '%_runelevated%' == '' (
  call %~dp0..\..\..\..\bin\CertificateGenerator\Release\net471\CertificateGenerator.exe -Uninstall >>%_cleanuplog%
) else (
  call %_runelevated% %~dp0..\..\..\..\bin\CertificateGenerator\Release\net471\CertificateGenerator.exe -Uninstall >nul
)

if NOT [%ERRORLEVEL%]==[0] (
	echo Warning: An error occurred while removing test certificates. >>%_cleanuplog%
	)

:done

exit /b 0
