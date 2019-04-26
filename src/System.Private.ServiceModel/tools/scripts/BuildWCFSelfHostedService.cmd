@echo off
setlocal

if not defined VisualStudioVersion (
    if defined VS140COMNTOOLS (
        call "%VS140COMNTOOLS%\VsDevCmd.bat"
        goto :EnvSet
    )

    if defined VS120COMNTOOLS (
        call "%VS120COMNTOOLS%\VsDevCmd.bat"
        goto :EnvSet
    )

    echo Error: %~nx0 requires Visual Studio 2013 or 2015.
    echo        Please see https://github.com/dotnet/wcf/wiki/Developer-Guide for build instructions.
    exit /b 1
)

:EnvSet

:: Log build command line
set _buildproj1=%~dp0..\SelfHostedWcfService\SelfHostedWCFService.sln
set _buildlog=%~dp0..\..\..\..\msbuildSelfHostedWcfService.log
set _buildprefix=echo
set _buildpostfix=^> "%_buildlog%"

call :build %*

:: Build
set _buildprefix=
set _buildpostfix=
call :build %*

goto :AfterBuild

:build
%_buildprefix% msbuild /p:Configuration=Release "%_buildproj1%" /nologo /maxcpucount /verbosity:minimal /nodeReuse:false /fileloggerparameters:Verbosity=diag;LogFile="%_buildlog%";Append %* %_buildpostfix%
set BUILDERRORLEVEL=%ERRORLEVEL%
goto :eof

:AfterBuild

echo.
:: Pull the build summary from the log file
findstr /ir /c:".*Warning(s)" /c:".*Error(s)" /c:"Time Elapsed.*" "%_buildlog%"
echo Build Exit Code = %BUILDERRORLEVEL%

exit /b %BUILDERRORLEVEL%
