echo off
setlocal

REM The Bridge process is not running, so ensure it is built
pushd %~dp0
echo Building the Bridge...
call BuildWCFTestService.cmd
popd

REM The BridgeResourceFolder is currently required so we use
REM either the default or optionally an environment variable

if '%BridgeResourceFolder%' == '' (
   set _bridgeResourceFolder=..\..\Bridge\Resources
) else (
   set _bridgeResourceFolder=%BridgeResourceFolder%
)

REM The Bridge is either running or has just been built.
REM Invoke it either to start it or ping it is alive.
REM In either case, this script will return as soon as the Bridge
REM is known running or fails to start.

pushd %~dp0..\..\..\..\bin\wcf\tools\Bridge
echo Invoking Bridge.exe -require -BridgeResourceFolder:%_bridgeResourceFolder% %*
call Bridge.exe -require -BridgeResourceFolder:%_bridgeResourceFolder%  %*
set _bridgeReturnCode=%ERRORLEVEL%
popd

if '%_bridgeReturnCode%' == '0' goto weStartedBridge
if '%_bridgeReturnCode%' == '1' goto bridgeAlreadyRunning
if '%_bridgeReturnCode%' == '-1' goto bridgeFailed

echo Unknown Bridge return code %_bridgeReturnCode% will be treated as failure

:bridgeFailed
echo EnsureBridgeRunning: The Bridge failed to start.
endlocal
exit /B -1

:weStartedBridge
echo EnsureBridgeRunning: The Bridge was started in response to this request.
endlocal
exit /B 0
  
:bridgeAlreadyRunning
echo EnsureBridgeRunning: The Bridge was already running
endlocal
exit /B 1


