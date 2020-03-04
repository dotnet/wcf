@ECHO OFF
SETLOCAL

CALL %~dp0\setup.cmd %~dp0

REM For Linux testing, we generate an image to execute normal usage scenario with necessary bits (test cases, test binary and nuget package) only.

PUSHD %~dp0

REM Start the test service, then find the host ip address and test container id.
CALL startTestServices.cmd
FOR /F "delims=" %%i IN ('"powershell . "%~dp0\TestServices\getHostIp.ps1"; (Get-Variable HOSTIP).value"') DO SET HOSTIP=%%i
FOR /F "delims=" %%i IN ('docker ps -aqf "name=svcutil-test-container"') DO SET SERVICEID=%%i

COPY dockerfile %TARGET%

"%ProgramFiles%\Docker\Docker\DockerCli.exe" -SwitchLinuxEngine
docker build -t dotnet.svcutil.test %TARGET%
docker run -e HOSTIP=%HOSTIP% -e SERVICEID=%SERVICEID% -e DOTNET_SVCUTIL_TEST_MODE=true dotnet.svcutil.test
SET RETVAL=%errorlevel%
POPD

FOR /f "delims=" %%i IN ('docker ps -laq') do @SET cid=%%i
RD /S /Q %BINSOURCE%\dockerTestOutput
docker cp %cid%:/app/TestOutput %BINSOURCE%\dockerTestOutput
docker rm %cid%

REM Stop the test service since we're done.
CALL %~dp0\stopTestServices.cmd

exit /b %RETVAL%

ENDLOCAL
