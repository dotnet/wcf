echo off
setlocal

pushd %~dp0
echo Building the Bridge...
call BuildWCFTestService.cmd
popd

echo Starting the Bridge with parameters %*
pushd %~dp0..\..\..\..\bin\wcf\tools\Bridge
start /MIN Bridge.exe %*
popd

exit /b

