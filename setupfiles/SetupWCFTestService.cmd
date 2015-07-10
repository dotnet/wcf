echo off
setlocal
REM elevated window does not set current directory correctly. 
REM Workaround it by passing the current directory around
pushd %1
certutil -addstore -f root RootCATest.cer
certutil -f -p test -importpfx "WcfTestServer.pfx"
netsh http add sslcert ipport=0.0.0.0:44285 certhash=1d85a3f6cd2c022c5ca54e5cb200a47f89ba0d3d appid={00000000-0000-0000-0000-000000000000}
start powershell -ExecutionPolicy Bypass -File ..\tools\Bridge\bin\ensureBridge.ps1
exit