echo off
setlocal
REM elevated window does not set current directory correctly. 
REM Workaround it by passing the current directory around
pushd %1
Taskkill /IM bridge.exe /F
netsh http delete sslcert ipport=0.0.0.0:44285
certutil.exe -delstore my "85 58 be 22 44 5e 00 96 4d 0e 4c 7e 47 7a a6 3a"
certutil -delstore Root "68 66 63 1a d6 d4 28 ab 49 82 7f ba 24 cc 33 26"
exit