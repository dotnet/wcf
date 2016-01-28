echo off
setlocal

REM Ensure that the Bridge is running.
REM If the Bridge is already running, this script simply exits.
REM If the Bridge is not running, it will be started in a new process,
REM and this script will block until it is started (or the wait times out).

tasklist /FI "IMAGENAME eq Bridge.exe" 2>NUL | find /I /N "Bridge.exe">NUL
if "%ERRORLEVEL%"=="0" (
  goto built
)

REM The Bridge process is not running, so ensure it is built
pushd %~dp0
echo Building the Bridge...
call BuildWCFTestService.cmd
popd

REM The Bridge is either running or has just been built.
REM Invoke it either to start it or ping it is alive.
REM In either case, this script will return as soon as the Bridge
REM is known running or fails to start.
:built
if '%BridgeResourceFolder%' == '' (
   set _bridgeResourceFolder=..\..\Bridge\Resources
) else (
   set _bridgeResourceFolder=%BridgeResourceFolder%
)

pushd %~dp0..\..\..\..\bin\wcf\tools\Bridge
echo Invoking Bridge.exe -require -BridgeResourceFolder:%_bridgeResourceFolder% %*
call Bridge.exe -require -BridgeResourceFolder:%_bridgeResourceFolder% %*
popd


exit /b

