@echo off
setlocal

set _VSWHERE="%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"
if exist %_VSWHERE% (
  for /f "usebackq tokens=*" %%i in (`%_VSWHERE% -latest -prerelease -property installationPath`) do set _VSCOMNTOOLS=%%i\Common7\Tools
)
if not exist "%_VSCOMNTOOLS%" set _VSCOMNTOOLS=%VS140COMNTOOLS%
if not exist "%_VSCOMNTOOLS%" goto :MissingVersion

set VSCMD_START_DIR="%~dp0"
call "%_VSCOMNTOOLS%\VsDevCmd.bat"
goto :EnvSet

:MissingVersion
:: Can't find VS 2017, 2019
echo Error: %~nx0 requires Visual Studio 2017 or 2019.
echo        Please see https://github.com/dotnet/wcf/wiki/Developer-Guide for build instructions.
exit /b 1

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
