echo off
setlocal

pushd %~dp0..\..\..\..\bin\wcf\tools\Bridge

REM BridgeStartedByScript is set by EnsureBridgeRunning based
REM on whether the Bridge was started by that script or was
REM already running.
if '%BridgeStartedByScript%' == 'true' GOTO stopBridge
echo Releasing Bridge resources...
echo Invoking Bridge.exe -reset %*
call Bridge.exe -reset %*
goto done

:stopBridge
echo Stopping the Bridge...
echo Invoking Bridge.exe -stop %* 
call Bridge.exe -stop %*

:done
popd
endlocal
exit /b
