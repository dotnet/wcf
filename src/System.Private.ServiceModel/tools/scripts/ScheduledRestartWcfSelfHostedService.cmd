@ECHO off 
:: This script closes the self-hosted WCF service, syncs and rebuilds the service, 
:: then restarts the service
::
:: This script should be set as a scheduled task to run 
:: - Upon machine restart 
:: - At a set time every evening for runs
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
SET _LOGFILE=%~dp0\ScheduledRestartWcfSelfHostedService_ScriptLog.txt
SET _SVCLOG=%~dp0\ScheduledRestartWcfSelfHostedService_ServiceLog.txt

echo [%~n0] %DATE% %TIME% Running script to restart self hosted WCF Server... >> %_LOGFILE%

tasklist | findstr /i SelfHostedWcfService

if not "%ERRORLEVEL%"=="0" (
   echo [%~n0] WARNING: Self-hosted WCF Service was not running when the script started.        >> %_LOGFILE%
   echo                 Check if this is the first run after reboot or as part of another script
   echo                 If this is true, no action needed    >> %_LOGFILE%
   echo                 If not, then the process may have terminated for other reasons and may  >> %_LOGFILE%
   echo                 warrant further investigation 
)

echo [%~n0] Kill SelfHostedWCFService.exe so that we can clean up the bin directory >> %_LOGFILE%
TASKKILL /F /IM SelfHostedWCFService.exe >> %_LOGFILE% 2>&1

:: Pull the latest code from GitHub

pushd %_GITREPO%
echo [%~n0] git checkout main >> %_LOGFILE% 
CALL git checkout main >> %_LOGFILE% 2>&1

echo [%~n0] git clean -xdfq >> %_LOGFILE% 
CALL git clean -xdfq >> %_LOGFILE% 2>&1

echo [%~n0] git pull >> %_LOGFILE%
CALL git pull >> %_LOGFILE% 2>&1

:: Build and start the selfhosted service

echo [%~n0] Building Selfhosted Service >> %_LOGFILE%
echo [%~n0] %_SCRIPTSDIR%\BuildWCFSelfHostedService.cmd >> %_LOGFILE%
CALL %_SCRIPTSDIR%\BuildWCFSelfHostedService.cmd >> %_LOGFILE% 2>&1

echo [%~n0] Starting Selfhosted Service >> %_LOGFILE%
echo [%~n0] START %_GITREPO%\artifacts\bin\SelfHostedWcfService\Release\net471\SelfHostedWcfService.exe >> %_LOGFILE%
start %_GITREPO%\artifacts\bin\SelfHostedWcfService\Release\net471\SelfHostedWcfService.exe > %_SVCLOG% 2>&1

:: Find out whether or not service is running
:: We can't use the errorlevel of Start because it returns 0 if launched, but we don't know if the launch results 
:: in success

tasklist | findstr /i SelfHostedWcfService > NUL
set __EXITCODE=%ERRORLEVEL%

if not "%__EXITCODE%"=="0" (
   echo [%~n0] ERROR: Self-hosted WCF Service is not running at the completion of this script >> %_LOGFILE%
)

popd

echo [%~n0] %DATE% %TIME% Ending script for restarting self hosted WCF Server...  >> %_LOGFILE%
echo. >> %_LOGFILE%
echo. >> %_LOGFILE%

exit /b %__EXITCODE%
endlocal
