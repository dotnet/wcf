@echo off
setlocal

set Id=%1
set IdMaster=Master
set WcfTestDir=c:\WCFTest
set RepoHome=c:\git
set CurrentRepo=%RepoHome%\wcf%Id%
set MasterRepo=%RepoHome%\wcf%IdMaster%
set PRServiceName=PRService%Id%
set WcfServiceName=WcfService%Id%
set PRServiceMaster=PRService%IdMaster%
set WcfServiceMaster=WcfService%IdMaster%
set LogFile="%~dp0%~n0.log"
set CertExpirationInDays=99
set ExitCode=0
set cmd=

if '%1'=='' goto :Usage
if '%1'=='/?' goto :Usage
if '%1'=='-?' goto :Usage
if '%1'=='/help' goto :Usage
if '%1'=='-help' goto :Usage

:: Make sure IIS appcmd.exe is accessible
set appcmd=%windir%\System32\inetsrv\appcmd.exe
if NOT EXIST %appcmd% (
    echo Could not find: %appcmd%.
    echo Please make sure IIS is installed.
    goto :Failure
)

:: Clean up any existing setup
echo Deleting WCF repo at %CurrentRepo% if exists and associated application pools and services hosted on IIS...
if exist %CurrentRepo% rmdir /s /q %CurrentRepo%
if exist %WcfTestDir% if /I '%MasterRepo%'=='%CurrentRepo%' rmdir /s /q %WcfTestDir%
%appcmd% delete app "Default Web Site/%PRServiceName%" >nul
%appcmd% delete apppool %PRServiceName% >nul
%appcmd% delete app "Default Web Site/%WcfServiceName%" >nul
%appcmd% delete apppool %WcfServiceName% >nul
echo Clean up done.
if /I '%2'=='/c' goto :Done

:: Make sure git.exe is available
where git.exe /Q
if ERRORLEVEL 1 (
    echo Could not find: git.exe.
    echo Please make sure git is installed and its location is part of PATH environment.
    goto :Failure
)

:: Clone a WCF repo
set cmd=git clone https://github.com/dotnet/wcf %CurrentRepo%
call %cmd%
if ERRORLEVEL 1 goto :Failure
pushd %CurrentRepo%
call :Run git config --add origin.fetch "+refs/pull/*/head:refs/remotes/origin/pr/*"
if ERRORLEVEL 1 goto :Failure
popd

:: Create a new application pool and an application for the WCF test service
echo Create IIS application pool: %WcfServiceName%
call :Run %appcmd% add apppool /name:%WcfServiceName% /processModel.IdentityType:LocalSystem /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated
if ERRORLEVEL 1 goto :Failure
echo Create IIS application: %WcfServiceName%
call :Run %appcmd% add app /site.name:"Default Web Site" /path:/%WcfServiceName% /physicalPath:%CurrentRepo%\src\System.Private.ServiceModel\tools\IISHostedWcfService /applicationPool:%WcfServiceName% /enabledProtocols:"http,net.tcp"
if ERRORLEVEL 1 goto :Failure

:: Grant app pool %WcfServiceName% "Read and Execute" access to the IISHostedWcfService and its subdirectories
echo Grant app pool %WcfServiceName% "Read and Execute" access to the IISHostedWcfService and its subdirectories
call :Run icacls %CurrentRepo%\src\System.Private.ServiceModel\tools\IISHostedWcfService /grant:r "IIS APPPOOL\%WcfServiceName%":RX /T /Q
if ERRORLEVEL 1 goto :Failure

:: Setup PR service only if this is a master repo
:: Uncomment line below when we switch to use one central PRService
REM if /I NOT '%MasterRepo%'=='%CurrentRepo%' goto :SkipPRService

:: Create a new application pool and an application for the PR service
echo Create IIS applicaton pool: %PRServiceName%
call :Run %appcmd% add apppool /name:%PRServiceName% /managedRuntimeVersion:v4.0 /managedPipelineMode:Integrated
if ERRORLEVEL 1 goto :Failure
echo Create IIS applicatoin: %PRServiceName%
call :Run %appcmd% add app /site.name:"Default Web Site" /path:/%PRServiceName% /physicalPath:%CurrentRepo%\src\System.Private.ServiceModel\tools\PRService -applicationPool:%PRServiceName%
if ERRORLEVEL 1 goto :Failure

:: Grant app pool %PRServiceName% "Modify" access to %CurrentRepo% and its subdirectories
echo Grant app pool %PRServiceName% "Modify" access to %CurrentRepo% and its subdirectories
call :Run icacls %CurrentRepo% /grant:r "IIS APPPOOL\%PRServiceName%":M /T /Q
if ERRORLEVEL 1 goto :Failure
:: Uncomment below and delete above when we switch to central PR service
REM echo Grant app pool %PRServiceName% "Read and Execute" access to the PRService and its subdirectories
REM call :Run icacls %CurrentRepo%\src\System.Private.ServiceModel\tools\PRService /grant:r "IIS APPPOOL\%PRServiceName%":RX /T /Q
REM if ERRORLEVEL 1 goto :Failure

:SkipPRService

:: Grant master PR service's app pool access to current repo if master PRService exists
if /I '%MasterRepo%'=='%CurrentRepo%' goto :SkipGrantAccess
if NOT EXIST %MasterRepo% goto :SkipGrantAccess
%appcmd% list apppool %PRServiceMaster% >nul
if ERRORLEVEL 1 goto :SkipGrantAccess
echo Grant master PRServer app pool: %PRServiceMaster% "Modify" access to %CurrentRepo% and its sudirectories
call :Run icacls %CurrentRepo% /grant:r "IIS APPPOOL\%PRServiceMaster%":M /T /Q
if ERRORLEVEL 1 goto :Failure

:SkipGrantAccess

:: Install certificates if %WcfTestDir% does not exist; otherwise, assume certs are installed already
if EXIST %WcfTestDir% goto :SkipCertInstall

:: Make sure there is an HTTPS binding in IIS site "Default Web Site"
%appcmd% list site "Default Web Site" /text:* | findstr /i bindings | findstr /i https >nul
if ERRORLEVEL 1 (
    echo Add an HTTPS binding to "Default Web Site"
    call :Run %appcmd% set site "Default Web Site" /+bindings.[protocol='https',bindingInformation='*:443:']
    if ERRORLEVEL 1 goto :Failure
)

:: Use current repo for cert gen if master repo does not exist
if NOT EXIST %MasterRepo% (
    set MasterRepo=%CurrentRepo%
    set WcfServiceMaster=%WcfServiceName%
)

echo Build CertificateGenerator tool
call :Run %MasterRepo%\src\System.Private.ServiceModel\tools\scripts\BuildCertUtil.cmd
if ERRORLEVEL 1 goto :Failure

echo Run CertificateGenerator tool. This will take a little while...
md %WcfTestDir%
set certGen=%MasterRepo%\bin\Wcf\tools\CertificateGenerator\CertificateGenerator.exe
echo ^<?xml version="1.0" encoding="utf-8"?^>^<configuration^>^<appSettings^>^<add key="testserverbase" value="%WcfServiceMaster%"/^>^<add key="CertExpirationInDay" value="%CertExpirationInDays%"/^>^<add key="CrlFileLocation" value="%WcfTestDir%\test.crl"/^>^</appSettings^>^<startup^>^<supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5"/^>^</startup^>^</configuration^>>%certGen%.config
call :Run %certGen%
if ERRORLEVEL 1 goto :Failure

echo Configue SSL certificate ports
call :Run powershell -NoProfile -ExecutionPolicy unrestricted %MasterRepo%\src\System.Private.ServiceModel\tools\scripts\ConfigHttpsPort.ps1
if ERRORLEVEL 1 goto :Failure

:: TODO: Grant all existing app pools named WCFService# 'Read' access to %WcfTestDir%. This is not needed for now

:SkipCertInstall

:: Grant app pool %WcfServiceName% "Read" access to %WcfTestDir% and its subdirectories
echo Grant app pool %WcfServiceName% "Read" access to %WcfTestDir% and its subdirectories
call :Run icacls %WcfTestDir% /grant:r "IIS APPPOOL\WcfService%Id%":R /T /Q
if ERRORLEVEL 1 goto :Failure

:: Clean up log file if everything worked perfectly
if EXIST %LogFile% del %LogFile% /f /q

echo.
echo All operations completed successfully!
goto :Done

:Usage
echo.
echo Setup WCF test services hosted on IIS for the testing of WCF on .NET Core
echo.
echo Usage: %~n0 Id [/c]
echo    Id: The Id of a WCF repo and its associated IIS hosted services to be created.
echo        The Id will be appended to the name of all assets to be created.
echo        %IdMaster%: if Id is '%IdMaster%', additional IIS hosted services such as PRService
echo        will be created to serve as central services for other WCF repos. 
echo    /c: If specified, this will clean up any existing setup of Id instead of creating new.
echo.
echo Examples:
echo    %~n0 42
echo    :Create a WCF repo named 'wcf42' and IIS hosted services such as 'WcfService42'
echo    %~n0 42 /c
echo    :Clean up WCF repo 'wcf42' and all associated IIS hosted services such as 'WcfService42'
goto :Done

:Run
set cmd=%*
if EXIST %LogFile% del %LogFile% /f /q
call %cmd%>%LogFile%
exit /b

:Failure
set ExitCode=1
if NOT '%cmd%'=='' (
    echo.
    echo Failed to run:
    echo %cmd%
)
if EXIST %LogFile% type %LogFile%

:Done
exit /b %ExitCode%
endlocal