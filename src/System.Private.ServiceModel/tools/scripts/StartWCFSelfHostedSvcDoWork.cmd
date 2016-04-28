@if "%_echo%" neq "on" echo off
setlocal

REM This script should always be called from StartWCFSelfHostedSvc.cmd

echo Do the actual work to start WCF self hosted service

REM Build tools
call %~dp0BuildWCFSelfHostedService.cmd
SET __EXITCODE=%ERRORLEVEL%
if NOT [%__EXITCODE%]==[0] (
    echo ERROR: An error occurred while building WCF Self hosted Service.
    goto :done
  )
call %~dp0BuildCertUtil.cmd
set __EXITCODE=%ERRORLEVEL%
if NOT [%__EXITCODE%]==[0] (
    echo ERROR: An error occurred while building the Certificate generator.
    goto :done
  )

REM Config Certs
REM we need the direcotry to save the test.crl file. We are investigate a way to get rid of it
md c:\wcftest
REM Certificate configuration errors are all non fatal currently because we non cert tests will still pass
%~dp0..\..\..\..\bin\Wcf\tools\CertificateGenerator\CertificateGenerator.exe
if NOT [%ERRORLEVEL%]==[0] (
    echo Warning: An error occurred while running certificate generator.
  )

call %~dp0OpenFirewallPorts.cmd
if NOT [%ERRORLEVEL%]==[0] (
    echo Warning: An error occurred while running certificate generator.
  )

powershell -NoProfile -ExecutionPolicy unrestricted %~dp0ConfigHttpsPort.ps1
if NOT [%ERRORLEVEL%]==[0] (
    echo Warning: An error occurred while configuration https port.
  )

REM
REM Start the self hosted WCF Test Service
call %~dp0..\..\..\..\bin\Wcf\tools\SelfHostedWcfService\SelfHostedWcfService.exe
set __EXITCODE=%ERRORLEVEL%

:Cleanup
call %~dp0CleanUpWCFSelfHostedSvc.cmd
:done

exit /b %__EXITCODE%
