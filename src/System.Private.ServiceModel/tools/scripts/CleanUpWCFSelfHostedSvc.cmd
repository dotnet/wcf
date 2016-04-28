@if "%_echo%" neq "on" echo off
setlocal

REM
echo Cleanup test services
REM  Note that we always return successful code in clean up routine because clean up errors are not fatal

TASKKILL /F /IM SelfHostedWCFService.exe
REM error 128 will be returned if The process not found
if NOT [%ERRORLEVEL%]==[0]  if NOT [%ERRORLEVEL%]==[128] (
	echo Warning: An error occurred while killing the SelfHostedWCFService.exe.
	)

If NOT exist %~dp0..\..\..\..\bin\Wcf\tools\CertificateGenerator\CertificateGenerator.exe (
	call %~dp0BuildCertUtil.cmd
	set __EXITCODE=%ERRORLEVEL%
	if NOT [%__EXITCODE%]==[0] (
		echo Warning: An error occurred while building the Certificate generator.
	 )
)

REM We will open a separate elevated window to do clean up only if the current window is not elevated
REM This is to improve the debugging experience
net session >nul 2>&1
if NOT [%ERRORLEVEL%]==[0] (
    set runelvated=%~dp0RunElevated.vbs
)

call %runelvated% %~dp0CleanUpHttpsPort.cmd
if NOT [%ERRORLEVEL%]==[0] (
	echo Warning: An error occurred while removing https port.
	)

call %runelvated% %~dp0RemoveFirewallPorts.cmd
if NOT [%ERRORLEVEL%]==[0] (
	echo Warning: An error occurred while removing Firewall ports.
	)

call %runelvated% %~dp0..\..\..\..\bin\Wcf\tools\CertificateGenerator\CertificateGenerator.exe -Uninstall
if NOT [%ERRORLEVEL%]==[0] (
	echo Warning: An error occurred while removing test certificates.
	)

:done

exit /b 0
