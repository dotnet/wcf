echo off
setlocal

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
call Bridge.exe -require -BridgeResourceFolder:%_bridgeResourceFolder%  %*
set BridgeReturnCode=%ERRORLEVEL%

echo Bridge.exe returned exit code %BridgeReturnCode%

if '%BridgeReturnCode%' == '2' goto weStartedBridge
if '%BridgeReturnCode%' == '1' goto bridgeAlreadyRunning
echo EnsureBridgeRunning: The attempt to start the Bridge failed with exit code %BridgeReturnCode%
goto done

:weStartedBridge
  echo EnsureBridgeRunning: The Bridge was started in response to this request
  set BridgeStartedByScript=true
  goto done
  
:bridgeAlreadyRunning
  echo EnsureBridgeRunning: The Bridge was already running
  set BridgeStartedByScript=false
  goto done
  
:done
popd

endlocal & set BridgeStartedByScript=%BridgeStartedByScript%
exit /b

