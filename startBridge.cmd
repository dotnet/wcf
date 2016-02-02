@echo off

REM EnsureBridgeRunning will guarantee the Bridge is built and
REM started.  The 'BridgeStartedByScript' environment variable
REM will reflect whether it was started or found to be already running.
pushd %~dp0src\System.Private.ServiceModel\tools\setupfiles
call EnsureBridgeRunning.cmd %*

REM Undo any decision EnsureBridgeRunning made about starting the
REM Bridge so the next test run will leave the Bridge running.
set BridgeStartedByScript=false

echo Because you started the Bridge manually, it will remain running until you stop it manually.
echo To stop the Bridge manually, use 'Bridge.exe -stop' or type 'exit' into the Bridge console window.

popd
exit /b

