@echo off
setlocal ENABLEDELAYEDEXPANSION
REM DelayedExpansion needed to allow %ERRORLEVEL% to be set/used multiple times

:: Note: We've disabled node reuse because it causes file locking issues.
::       The issue is that we extend the build with our own targets which
::       means that that rebuilding cannot successfully delete the task
::       assembly. 

set outloop=false
set setupFilesFolder=%~dp0src\System.Private.ServiceModel\tools\setupfiles

REM this is a temporary step until the build tools provide a proper hook. 
REM it will need to deal with multiple include and exclude categories.
REM See dotnet/corefx#1477

echo %* | findstr /i /C:"OuterLoop"  1>nul
if %errorlevel% equ 0 (
  set outloop=true
)

if not defined VisualStudioVersion (
    if defined VS140COMNTOOLS (
        call "%VS140COMNTOOLS%\VsDevCmd.bat"
        goto :EnvSet
    )

    if defined VS120COMNTOOLS (
        call "%VS120COMNTOOLS%\VsDevCmd.bat"
        goto :EnvSet
    )

    echo Error: build.cmd requires Visual Studio 2013 or 2015. 
    echo        Please see https://github.com/dotnet/wcf/blob/master/Documentation/developer-guide.md for build instructions.
    exit /b 1
)

:EnvSet

call %~dp0init-tools.cmd

:: Clear the 'Platform' env variable for this session,
:: as it's a per-project setting within the build, and
:: misleading value (such as 'MCD' in HP PCs) may lead
:: to build breakage (issue: #69).
set Platform=

:: Log build command line
set _buildproj=%~dp0build.proj
set _buildlog=%~dp0msbuild.log
set _buildprefix=echo
set _buildpostfix=^> "%_buildlog%"

if "%outloop%" equ "true"  (
    pushd %setupFilesFolder%
    call EnsureBridgeRunning.cmd
    set _bridgeReturnCode=!ERRORLEVEL!
    echo EnsureBridgeRunning returned !_bridgeReturnCode!
    popd
)

call :build %*

:: Build
set _buildprefix=
set _buildpostfix=
echo [%time%] Building Managed Libraries...
call :build %*

goto :AfterBuild

:build
%_buildprefix% msbuild "%_buildproj%" /nologo /maxcpucount /verbosity:minimal /nodeReuse:false /fileloggerparameters:Verbosity=diag;LogFile="%_buildlog%";Append %* %_buildpostfix%
set BUILDERRORLEVEL=!ERRORLEVEL!
goto :eof

:AfterBuild
echo.
:: Pull the build summary from the log file
findstr /ir /c:".*Warning(s)" /c:".*Error(s)" /c:"Time Elapsed.*" "%_buildlog%"
echo [%time%] Build Exit Code = %BUILDERRORLEVEL%

:doneBridge
if "%outloop%" equ "true"  (
    pushd %~dp0bin\wcf\tools\Bridge
    if '!_bridgeReturnCode!' == '0' (
      echo Stopping the Bridge ...
      call Bridge.exe -stop %*
    ) else (
      echo Releasing Bridge resources ...
      call Bridge.exe -reset %*
    )
    popd

    pushd %setupFilesFolder%
    call CleanupCertificates.cmd
    popd
)

endlocal
exit /b %BUILDERRORLEVEL%