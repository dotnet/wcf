echo off
setlocal

echo Releasing Bridge resources and stopping it if running locally.
pushd %~dp0..\..\..\..\bin\wcf\tools\Bridge
echo Invoking Bridge.exe -stopIfAutoStart -reset %* ...
call Bridge.exe -stopIfAutoStart -reset %*
popd


exit /b
