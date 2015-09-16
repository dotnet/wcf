echo off
setlocal

pushd %~dp0
echo Building the Bridge...
call BuildWCFTestService.cmd
popd

pushd %~dp0..\..\..\..\bin\wcf\tools\Bridge
echo starting the Bridge with arguments: /BridgeResourceFolder:..\..\Bridge\Resources %*
start /MIN Bridge.exe /BridgeResourceFolder:..\..\Bridge\Resources %*
popd

exit /b

