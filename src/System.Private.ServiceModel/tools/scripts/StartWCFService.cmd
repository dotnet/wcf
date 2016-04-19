@echo off 
setlocal

REM 
echo Start WCF Self Hosted Service on local machine
REM

REM Build tools
call %~dp0BuildWCFSelfHostedService.cmd
if NOT [%ERRORLEVEL%]==[0] (
    echo ERROR: An error occurred while building WCF Self hosted Service.
    set __EXITCODE=%ERRORLEVEL% 
    goto :done
  )
call %~dp0BuildCertUtil.cmd
if NOT [%ERRORLEVEL%]==[0] (
    echo ERROR: An error occurred while building the Certificate generator.
    set __EXITCODE=%ERRORLEVEL%
    goto :done
  )


REM Config Certs
REM we need the direcotry to save the test.crl file. We are investigate a way to get rid of it
REM
md c:\wcftest
call %~dp0..\..\..\..\bin\Wcf\tools\CertificateGenerator\CertificateGenerator.exe %*
set __EXITCODE=%ERRORLEVEL%

if NOT [%__EXITCODE%]==[0] (
    echo Warning: An error occurred while running certificate generator.
  )

REM skip the checking of error code as OpenFirewallPorts can return different error code even it's successful
call %~dp0OpenFirewallPorts.cmd

powershell -NoProfile -ExecutionPolicy unrestricted %~dp0ConfigHttpsPort.ps1
if NOT [%ERRORLEVEL%]==[0] (
    echo Warning: An error occurred while configuration https port.
  )

REM Start the self hosted WCF Test Service
call %~dp0..\..\..\..\bin\Wcf\tools\SelfHostedWcfService\SelfHostedWcfService.exe %*
set __EXITCODE=%ERRORLEVEL%

:done

endlocal
exit /b %__EXITCODE% 

