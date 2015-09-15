echo off
setlocal

echo BridgeKeepRunning=%BridgeKeepRunning%
if '%BridgeKeepRunning%' neq 'true' (
    echo Stopping the Bridge...
    pushd %~dp0..\..\..\..\bin\wcf\tools\Bridge
    call Bridge.exe -stopIfLocal -reset %*
    popd
) else (
    echo Releasing Bridge resources but leaving it running...
    pushd %~dp0..\..\..\..\bin\wcf\tools\Bridge
    call Bridge.exe -reset %*
    popd
    echo The Bridge was left running because BridgeKeepRunning is true
)

exit /b
