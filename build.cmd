@echo off

:: *** start WCF Content ***
setlocal ENABLEDELAYEDEXPANSION
REM DelayedExpansion needed to allow %ERRORLEVEL% to be set/used multiple times
:: *** end WCF Content ***

:: Note: We've disabled node reuse because it causes file locking issues.
::       The issue is that we extend the build with our own targets which
::       means that that rebuilding cannot successfully delete the task
::       assembly. 

:: *** start WCF Content ***
set outloop=false
set setupFilesFolder=%~dp0src\System.Private.ServiceModel\tools\setupfiles

REM this is a temporary step until the build tools provide a proper hook. 
REM it will need to deal with multiple include and exclude categories.
REM See dotnet/corefx#1477

echo %* | findstr /i /C:"OuterLoop"  1>nul
if %errorlevel% equ 0 (
  set outloop=true
)

echo %* | findstr /i /C:"FloatingTestRuntimeDependencies=true"  1>nul
set _FloatingDependencies=!ERRORLEVEL!
if '!_FloatingDependencies!'=='0' (
  set TestFixedRuntimeProjectJson=%~dp0src\Common\test-runtime\LatestDependencies\project.json
  echo %* | findstr /i /C:"/p:Configuration="  1>nul
  if !ERRORLEVEL! neq 0 (
  set _defaultBuildConfig=/p:Configuration=Windows_NT_Debug
  )
)
:: *** end WCF Content ***

:ReadArguments
:: Read in the args to determine whether to run the native build, managed build, or both (default)
set "__args=%*"
if /i [%1] == [native] (set __buildSpec=native&&set "__args=%__args:~6%"&&shift&&goto Tools)
if /i [%1] == [managed] (set __buildSpec=managed&&set "__args=%__args:~7%"&&shift&&goto Tools)

:Tools
:: Setup VS
if not defined VisualStudioVersion (
    if defined VS140COMNTOOLS (
        call "%VS140COMNTOOLS%\VsDevCmd.bat"
        goto :Build
    )

    echo Error: build.cmd requires Visual Studio 2015.
	:: *** start WCF Content ***
    echo        Please see https://github.com/dotnet/wcf/blob/master/Documentation/developer-guide.md for build instructions.
	:: *** end WCF Content ***
    exit /b 1
)

:Build
:: Restore the Tools directory
call %~dp0init-tools.cmd

:: Call the builds
if "%__buildSpec%"=="managed"  goto :BuildManaged

:BuildNative
:: Run the Native Windows build
echo [%time%] Building Native Libraries...
call %~dp0src\native\Windows\build-native.cmd %__args% >nativebuild.log
IF ERRORLEVEL 1 (
    echo Native component build failed see nativebuild.log for more details.
) else (
    echo [%time%] Successfully built Native Libraries.
)

:: If we only wanted to build the native components, exit
if "%__buildSpec%"=="native" goto :eof 

:BuildManaged
:: Clear the 'Platform' env variable for this session,
:: as it's a per-project setting within the build, and
:: misleading value (such as 'MCD' in HP PCs) may lead
:: to build breakage (issue: #69).
set Platform=

:: Log build command line
set _buildproj=%~dp0build.proj
set _buildlog=%~dp0msbuild.log
set _binclashLoggerDll=%~dp0Tools\net45\Microsoft.DotNet.Build.Tasks.dll  
set _binclashlog=%~dp0binclash.log  
set _buildprefix=echo
set _buildpostfix=^> "%_buildlog%"

:: *** start WCF Content ***
if "%outloop%" equ "true"  (
    pushd %setupFilesFolder%
    call EnsureBridgeRunning.cmd %*
    set _bridgeReturnCode=!ERRORLEVEL!
    echo EnsureBridgeRunning returned !_bridgeReturnCode!
    popd
)
:: *** end WCF Content ***

call :build %__args%

:: Build
set _buildprefix=
set _buildpostfix=
echo [%time%] Building Managed Libraries...
call :build %__args%

goto :AfterBuild

:build
:: *** start WCF Content <WCF uses additional msbuild args> ***
%_buildprefix% msbuild "%_buildproj%" %_defaultBuildConfig% /nologo /maxcpucount /v:minimal /clp:Summary /nodeReuse:false /flp:v=diag;LogFile="%_buildlog%";Append "/l:BinClashLogger,%_binclashLoggerDll%;LogFile=%_binclashlog%" %__args% %_buildpostfix%
set BUILDERRORLEVEL=!ERRORLEVEL!
:: *** end WCF Content ***
goto :eof

:AfterBuild
echo.
:: Pull the build summary from the log file
findstr /ir /c:".*Warning(s)" /c:".*Error(s)" /c:"Time Elapsed.*" "%_buildlog%"
echo [%time%] Build Exit Code = %BUILDERRORLEVEL%

:: *** start WCF Content ***
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
:: *** end WCF Content ***

exit /b %BUILDERRORLEVEL%