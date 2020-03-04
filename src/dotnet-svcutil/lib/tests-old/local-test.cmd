@ECHO OFF
SETLOCAL

IF NOT "%BuildConfiguration%"=="Release" SET BuildConfiguration=Debug

SET WORKINGDIR=%~dp0..\..\..\..\artifacts\bin\dotnet-svcutil.test\%BuildConfiguration%\netcoreapp2.1\
SET DOTNET_SVCUTIL_TEST_MODE=true

REM Start the test service.
CALL %~dp0\startTestServices.cmd

PUSHD "%WORKINGDIR%"
dotnet vstest /logger:trx;LogFileName=..\TestOutput\local-out.trx dotnet-svcutil.test.dll
SET RETVAL=%errorlevel%
POPD

REM Stop the test service since we're done.
CALL %~dp0\stopTestServices.cmd

exit /b %RETVAL%

ENDLOCAL
