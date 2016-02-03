@echo off

echo Stopping the Bridge ...
pushd %~dp0bin\wcf\tools\Bridge
call Bridge.exe -stop %*
popd

exit /b


