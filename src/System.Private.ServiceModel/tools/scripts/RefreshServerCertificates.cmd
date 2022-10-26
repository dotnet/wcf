@ECHO off 
:: This script closes the self-hosted WCF service, refreshes the certificates, then restarts the service
:: 
:: This script should be set as a scheduled task to run 
:: - Every 80 days 
::
:: Note that for first run, recommend:
:: 1. Run %_GITREPO%\src\System.Private.ServiceModel\tools\scripts\StartWCFSelfHostedSvc.cmd
:: 2. Close the self hosted service non gracefully (so that the certificates are saved) 
:: 3. Run this script
::
:: This file is not designed to live inside a repo, hence why the deep file paths into the repo
:: By convention, we place this file into C:\wcftest
::

setlocal

SET _GITREPO=C:\git\wcf
SET _SCRIPTSDIR=%_GITREPO%\src\System.Private.ServiceModel\tools\scripts
SET _LOGFILE=%~dp0\RefreshServerCertificates_ScriptLog.txt
SET _RESTARTSCRIPT=%~dp0\ScheduledRestartWcfSelfHostedService.cmd

echo [%~n0] %DATE% %TIME% Running script to regenerate certificates... >> %_LOGFILE%

if not exist %_RESTARTSCRIPT% (
    echo [%~n0] Cannot find ScheduledRestartWcfSelfHostedService.cmd in %~dp0           >> %_LOGFILE%
    echo        We require that script to start the WCF Self Hosted Service after       >> %_LOGFILE%
    echo        certificates are regenerated                                            >> %_LOGFILE%
    
    set _EXITCODE=1
    goto end
)

:: Kill WCF self-hosted service
TASKKILL /F /IM SelfHostedWCFService.exe                                                >> %_LOGFILE%

:: Regenerate certificates
echo call %_SCRIPTSDIR%\BuildCertUtil.cmd                                               >> %_LOGFILE%
call %_SCRIPTSDIR%\BuildCertUtil.cmd                                                    >> %_LOGFILE%

echo [%~n0] cmd /c %_GITREPO%\artifacts\bin\CertificateGenerator\Release\net471\CertificateGenerator.exe       >> %_LOGFILE%
cmd /c %_GITREPO%\artifacts\bin\CertificateGenerator\Release\net471\CertificateGenerator.exe                   >> %_LOGFILE% 2>&1

if NOT "%ERRORLEVEL%"=="0" (
    echo Warning: An error occurred when calling CertificateGenerator.exe.                               >> %_LOGFILE%
    set _EXITCODE=1
    goto end
)

:: Configure HTTPS ports to use new certificate
echo [%~n0] powershell -NoProfile -ExecutionPolicy unrestricted %_SCRIPTSDIR%\ConfigHttpsPort.ps1   >> %_LOGFILE% 
powershell -NoProfile -ExecutionPolicy unrestricted %_SCRIPTSDIR%\ConfigHttpsPort.ps1               >> %_LOGFILE% 2>&1

if NOT "%ERRORLEVEL%"=="0" (
    echo Warning: An error occurred while configuring the https port.                               >> %_LOGFILE%
    set _EXITCODE=1
    goto end
)

echo [%~n0] cmd /c iisreset >> %_LOGFILE%                   
cmd /c iisreset             >> %_LOGFILE% 2>&1

if NOT "%ERRORLEVEL%"=="0" (
    echo Warning: An error occurred while running iisreset.                               >> %_LOGFILE%
    set _EXITCODE=1
    goto end
)

:: Start self-hosted service 
echo [%~n0] %DATE% %TIME% Calling  %_RESTARTSCRIPT%...  >> %_LOGFILE%
call %_RESTARTSCRIPT%

if NOT "%ERRORLEVEL%"=="0" (
    echo Warning: An error occurred while restarting the sevice                           >> %_LOGFILE%
    set _EXITCODE=1
    goto end
)

:end 

echo [%~n0] %DATE% %TIME% Ending script for regenerating certificates...  >> %_LOGFILE%
echo. >> %_LOGFILE%
echo. >> %_LOGFILE%

exit /b %_EXITCODE%
endlocal
