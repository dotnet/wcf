@echo off

REM EnsureBridgeRunning will guarantee the Bridge is built and started.
pushd %~dp0src\System.Private.ServiceModel\tools\setupfiles
call EnsureBridgeRunning.cmd %*
popd

echo You can stop the Bridge using stopBridge.cmd or type 'exit' in the Bridge console window.

exit /b


