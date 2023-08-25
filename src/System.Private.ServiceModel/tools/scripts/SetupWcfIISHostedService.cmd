@echo off
setlocal

set _id=%1
set _idMaster=Master
set _wcfTestDir=c:\WCFTest
set _repoHome=c:\git
set _pathIISHostedWcfService=src\System.Private.ServiceModel\tools\IISHostedWcfService
set _currentRepo=%_repoHome%\wcf%_id%
set _masterRepo=%_repoHome%\wcf%_idMaster%
set _prServiceName=PRService%_id%
set _wcfServiceName=WcfService%_id%
set _prServiceMaster=PRService%_idMaster%
set _wcfServiceMaster=WcfService%_idMaster%
set _logFile="%~dp0%~n0.log"
set _certExpirationInDays=99
set _githubWcfRepoUrl=https://github.com/dotnet/wcf
set _exitCode=0
set _cmd=
set _cleanup=
set _useExistingRepo=

if '%1'=='' goto :Usage
if '%1'=='/?' goto :Usage
if '%1'=='-?' goto :Usage
if '%1'=='/help' goto :Usage
if '%1'=='-help' goto :Usage

echo.
echo %date% %time%
echo ==============================

:: Parse command line arguments
:NextArg
shift
set arg=%1

if '%arg%'=='' goto :NoMoreArg

if /I '%arg%'=='/c' (
    set _cleanup=true
    goto :NextArg
)

if /I '%arg:~0,2%'=='/p' (
    set _useExistingRepo=true
    set _currentRepo=%arg:~3%
    goto :NextArg
)

echo IGNORED unknown argument ^(use /? for help^): %arg%
goto :NextArg

:NoMoreArg

:: Make sure this script is running in elevated
if EXIST %_logFile% del %_logFile% /f /q
net session>nul 2>&1
if ERRORLEVEL 1 (
    echo. & echo ERROR: Please run this script with elevated permission.
    goto :Failure
)

:: Make sure IIS appcmd.exe is accessible
set _appcmd=%windir%\System32\inetsrv\appcmd.exe
if NOT EXIST %_appcmd% (
    echo Could not find: %_appcmd%.
    echo Please make sure IIS is installed.
    goto :Failure
)

:: Clean up any existing setup
echo Deleting WCF repo at %_currentRepo% if exists and associated application pools and services hosted on IIS...
%_appcmd% delete app "Default Web Site/%_prServiceName%" >nul
%_appcmd% delete apppool %_prServiceName% >nul
%_appcmd% delete app "Default Web Site/%_wcfServiceName%" >nul
%_appcmd% delete apppool %_wcfServiceName% >nul
if EXIST %_currentRepo% if DEFINED _useExistingRepo (echo User provided repo is not deleted: %_currentRepo%) else (rmdir /s /q %_currentRepo%)
if EXIST %_wcfTestDir% if /I '%_id%'=='%_idMaster%' rmdir /s /q %_wcfTestDir%
echo Clean up done.
if DEFINED _cleanup goto :Done

:: Determine if we need to clone a new WCF repo
if DEFINED _useExistingRepo (
    if NOT EXIST %_currentRepo% echo ERROR: The provided WCF repo was not found: %_currentRepo% & goto :Failure
    echo Use provided WCF repo: %_currentRepo%
    goto :SkipCloneRepo
)

:: Make sure git.exe is available
where git.exe /Q
if ERRORLEVEL 1 (
    echo Could not find: git.exe.
    echo Please make sure git is installed and its location is part of PATH environment.
    goto :Failure
)

:: Clone a WCF repo
set _cmd=git clone %_githubWcfRepoUrl% %_currentRepo%
call %_cmd%
if ERRORLEVEL 1 goto :Failure
pushd %_currentRepo%
call :Run git config --add origin.fetch "+refs/pull/*/head:refs/remotes/origin/pr/*"
if ERRORLEVEL 1 goto :Failure
popd

:SkipCloneRepo

:: Create a new application pool and an application for the WCF test service
echo Create IIS application pool: %_wcfServiceName%
call :Run %_appcmd% add apppool /name:%_wcfServiceName% /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated
if ERRORLEVEL 1 goto :Failure
echo Create IIS application: %_wcfServiceName%
call :Run %_appcmd% add app /site.name:"Default Web Site" /path:/%_wcfServiceName% /physicalPath:%_currentRepo%\%_pathIISHostedWcfService% /applicationPool:%_wcfServiceName% /enabledProtocols:"http,net.tcp"
if ERRORLEVEL 1 goto :Failure

:: Grant app pool %_wcfServiceName% "Read and Execute" access to the IISHostedWcfService and its subdirectories
echo Grant app pool %_wcfServiceName% "Read and Execute" access to %_currentRepo%\%_pathIISHostedWcfService%
call :Run icacls %_currentRepo%\%_pathIISHostedWcfService% /grant:r "IIS APPPOOL\%_wcfServiceName%":(OI)(CI)RX /Q
if ERRORLEVEL 1 goto :Failure

echo Grant app pool %_wcfServiceName% permission to reserve HTTP URL: "http://+:80/WindowsCommunicationFoundationTest/%_wcfServiceName%"
call :Run netsh http delete urlacl url=http://+:80/WindowsCommunicationFoundationTest/%_wcfServiceName%/
call :Run netsh http add urlacl url=http://+:80/WindowsCommunicationFoundationTest/%_wcfServiceName%/ user="IIS APPPOOL\%_wcfServiceName%"
if ERRORLEVEL 1 goto :Failure

:: Setup PR service only if this is a master service
if /I NOT '%_id%'=='%_idMaster%' (
    echo SKIP creating PRService. PRService will be created when Id=%_idMaster%.
    goto :SkipPRService
)

:: Create a new application pool and an application for the PR service
echo Create IIS application pool: %_prServiceName%
call :Run %_appcmd% add apppool /name:%_prServiceName% /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated
if ERRORLEVEL 1 goto :Failure
echo Create IIS application: %_prServiceName%
call :Run %_appcmd% add app /site.name:"Default Web Site" /path:/%_prServiceName% /physicalPath:%_currentRepo%\src\System.Private.ServiceModel\tools\PRService -applicationPool:%_prServiceName%
if ERRORLEVEL 1 goto :Failure

:: Grant app pool %_prServiceName% "Modify" access to %_currentRepo% and its subdirectories
echo Grant app pool %_prServiceName% "Read and Execute" access to the PRService and its subdirectories
call :Run icacls %_currentRepo%\src\System.Private.ServiceModel\tools\PRService /grant:r "IIS APPPOOL\%_prServiceName%":(OI)(CI)RX /Q
if ERRORLEVEL 1 goto :Failure

:SkipPRService

:: Grant master PR service's app pool access to current repo if master PRService exists
if /I '%_id%'=='%_idMaster%' goto :SkipGrantAccess
%_appcmd% list apppool %_prServiceMaster% >nul
if ERRORLEVEL 1 (
    echo SKIP granting PRService M permission to current repo. PRService not found: %_prServiceMaster%.
    goto :SkipGrantAccess
)
echo Grant master PRService app pool: %_prServiceMaster% "Modify" access to %_currentRepo% and its sudirectories
call :Run icacls %_currentRepo% /grant:r "IIS APPPOOL\%_prServiceMaster%":(OI)(CI)M /Q
if ERRORLEVEL 1 goto :Failure

:SkipGrantAccess

:: Make sure there is an HTTPS binding in IIS site "Default Web Site"
%_appcmd% list site "Default Web Site" /text:* | findstr /i bindings | findstr /i https >nul
if ERRORLEVEL 1 (
    echo Add an HTTPS binding to "Default Web Site"
    call :Run %_appcmd% set site "Default Web Site" /+bindings.[protocol='https',bindingInformation='*:443:']
    if ERRORLEVEL 1 goto :Failure
    :: Delete %_wcfTestDir% to force certificate regeneration later, which will fill in correct cert for https binding
    if EXIST %_wcfTestDir% rmdir /s /q %_wcfTestDir%
) else (
    echo Found and will use existing HTTPS binding in "Default Web Site"
)

:: Install certificates if %_wcfTestDir% does not exist; otherwise, assume certs are installed already
if EXIST %_wcfTestDir% (
    echo SKIP certificate generation. To force certificate generation, delete %_wcfTestDir% before running this script.
    goto :SkipCertInstall
)

:: Use master repo for cert generation if master repo exists; otherwise use current repo
if EXIST %_masterRepo% (
    set _certRepo=%_masterRepo%
    set _certService=%_wcfServiceMaster%
) else (
    set _certRepo=%_currentRepo%
    set _certService=%_wcfServiceName%
)
echo Use %_certService% for certificate service

echo Build CertificateGenerator tool
call :Run %_certRepo%\src\System.Private.ServiceModel\tools\scripts\BuildCertUtil.cmd
if ERRORLEVEL 1 goto :Failure

echo Run CertificateGenerator tool. This will take a little while...
md %_wcfTestDir%
set certGen=%_certRepo%\artifacts\bin\CertificateGenerator\Release\net471\CertificateGenerator.exe
echo ^<?xml version="1.0" encoding="utf-8"?^>^<configuration^>^<appSettings^>^<add key="testserverbase" value="%_certService%"/^>^<add key="CertExpirationInDay" value="%_certExpirationInDays%"/^>^<add key="CrlFileLocation" value="%_wcfTestDir%\test.crl"/^>^</appSettings^>^<startup^>^<supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5"/^>^</startup^>^</configuration^>>%certGen%.config
call :Run %certGen%
if ERRORLEVEL 1 goto :Failure

echo Configue SSL certificate ports
call :Run powershell -NoProfile -ExecutionPolicy unrestricted %_certRepo%\src\System.Private.ServiceModel\tools\scripts\ConfigHttpsPort.ps1
if ERRORLEVEL 1 goto :Failure

:: TODO: Grant all existing app pools named WCFService# 'Read' access to %_wcfTestDir%. This is not needed for now

:: Grant app pool %_certService% "Read" access to %_wcfTestDir% and its subdirectories
echo Grant app pool %_certService% "Read" access to %_wcfTestDir% and its subdirectories
call :Run icacls %_wcfTestDir% /grant:r "IIS APPPOOL\%_certService%":(OI)(CI)R /Q
if ERRORLEVEL 1 goto :Failure

:SkipCertInstall

echo Configue SSL certificate private key permissions
call :Run powershell -NoProfile -ExecutionPolicy unrestricted %_currentRepo%\src\System.Private.ServiceModel\tools\scripts\CertificatePrivateKeyPermissions.ps1 'IIS APPPOOL\%_wcfServiceName%'
if ERRORLEVEL 1 goto :Failure

:: Unlock the configuration of sslFlags and Authentication
echo Unlock the IIS config section to allow sslFlags and Authentication methods to be overriden
call :Run %_appcmd% unlock config /section:"system.webServer/security/access"
if ERRORLEVEL 1 goto :Failure

call :Run %_appcmd% unlock config /section:"system.webServer/security/authentication/anonymousAuthentication"
if ERRORLEVEL 1 goto :Failure

call :Run %_appcmd% unlock config /section:"system.webServer/security/authentication/basicAuthentication"
if ERRORLEVEL 1 goto :Failure

call :Run %_appcmd% unlock config /section:"system.webServer/security/authentication/digestAuthentication"
if ERRORLEVEL 1 goto :Failure

call :Run %_appcmd% unlock config /section:"system.webServer/security/authentication/windowsAuthentication"
if ERRORLEVEL 1 goto :Failure

:: Clean up log file if everything worked perfectly
if EXIST %_logFile% del %_logFile% /f /q

echo.
echo All operations completed successfully!
goto :Done

:Usage
echo.
echo Setup WCF test services hosted on IIS for the testing of WCF on .NET Core
echo.
echo Usage: %~n0 Id [/c] [/p:{PathToWcfRepo}]
echo    Id: The Id of a WCF repo and its associated IIS hosted services to be created.
echo        The Id will be appended to the name of all assets to be created.
echo        %_idMaster%: if Id is '%_idMaster%', additional IIS hosted services such as PRService
echo        will be created to serve as central services for other WCF repos. 
echo    /c: If specified, this will clean up any existing setup of Id instead of creating new.
echo    /p: If an existing WCF repo is preferred to be used, use this parameter to provide the
echo        path to a WCF repo. Otherwise, the script will clone a new WCF repo at %_repoHome%
echo        to use as the source of WCF test service.
echo.
echo Examples:
echo    %~n0 42
echo    :Create a WCF repo named 'wcf42' and IIS hosted services such as 'WcfService42'
echo    %~n0 42 /c
echo    :Clean up WCF repo 'wcf42' and all associated IIS hosted services such as 'WcfService42'
echo    %~n0 42 /p:"c:\my\existing\wcf\repo"
echo    :Create an IIS hosted service named 'wcfService42' by using existing WCF repo located at "c:\my\existing\wcf\repo".
goto :Done

:Run
set _cmd=%*
if EXIST %_logFile% del %_logFile% /f /q
call %_cmd% >%_logFile% 2>&1
exit /b

:Failure
set _exitCode=1
if DEFINED _cmd echo. & echo Failed to run: & echo %_cmd%
if EXIST %_logFile% type %_logFile%

:Done
exit /b %_exitCode%
endlocal
