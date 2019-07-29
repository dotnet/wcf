@echo off
setlocal

if not defined VisualStudioVersion (
    if defined VS160COMNTOOLS (
        call "%VS160COMNTOOLS%\VsDevCmd.bat"
        goto :EnvSet
    )

    echo Error: %~nx0 requires Visual Studio 2019 because the .NET Core 3.0 SDK is needed.
    echo        Please see https://github.com/dotnet/wcf/blob/master/Documentation/developer-guide.md for build instructions.
    exit /b 1
)

:EnvSet

:: Log build command line
set _buildproj=%~dp0..\CertificateGenerator\CertificateGenerator.sln
set _buildlog=%~dp0..\..\..\..\msbuildCertificateGenerator.log
set _buildprefix=echo
set _buildpostfix=^> "%_buildlog%"

set cmd=msbuild /p:Configuration=Release /t:restore;build "%_buildproj%" /nologo /maxcpucount /verbosity:minimal /nodeReuse:false /fileloggerparameters:Verbosity=diag;LogFile="%_buildlog%";Append %*
echo %cmd%
%cmd%
set BUILDERRORLEVEL=%ERRORLEVEL%

:AfterBuild

echo.
:: Pull the build summary from the log file
findstr /ir /c:".*Warning(s)" /c:".*Error(s)" /c:"Time Elapsed.*" "%_buildlog%"
echo Build Exit Code = %BUILDERRORLEVEL%

exit /b %BUILDERRORLEVEL%
