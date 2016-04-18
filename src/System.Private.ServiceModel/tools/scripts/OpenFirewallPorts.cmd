@echo off 
setlocal

REM 
echo Start Open all ports WCF test services use
REM

netsh advfirewall firewall add rule name="_WCF Test Server PortHttp" dir=in action=allow profile=any localport=8081 protocol=tcp
netsh advfirewall firewall add rule name="_WCF Test Server PortHttps" dir=in action=allow profile=any localport=44285 protocol=tcp
netsh advfirewall firewall add rule name="_WCF Test Server PortTcp" dir=in action=allow profile=any localport=809 protocol=tcp
netsh advfirewall firewall add rule name="_WCF Test Server PortWebSocket" dir=in action=allow profile=any localport=8083 protocol=tcp
netsh advfirewall firewall add rule name="_WCF Test Server PortWebSockets" dir=in action=allow profile=any localport=8084 protocol=tcp

REM we do not currently check the error level for the netsh advfirewall command. Will need to investigate the expected error
REM code in all successful cases and add the check. Will also need to add remove the firewall rules script for clean up
set __EXITCODE=%ERRORLEVEL%

endlocal
exit /b %__EXITCODE% 

