@echo off 
setlocal

echo Start Open all ports WCF test services use

SET __EXITCODE=0

net session >nul 2>&1
set __EXITCODE=%ERRORLEVEL%
if NOT [%__EXITCODE%]==[0] (
    echo Error: this script must be called in an elevated window
	goto :done
)

netsh advfirewall firewall add rule name="_WCF Test Server PortHttp80"  dir=in action=allow profile=any localport=80 protocol=tcp
set __EXITCODE=%ERRORLEVEL%
if NOT [%__EXITCODE%]==[0] (
  echo Error: error adding rule name _WCF Test Server PortHttp80
  goto :done
  )

netsh advfirewall firewall add rule name="_WCF Test Server PortHttp"  dir=in action=allow profile=any localport=8081 protocol=tcp
set __EXITCODE=%ERRORLEVEL%
if NOT [%__EXITCODE%]==[0] (
  echo Error: error adding rule name _WCF Test Server PortHttp
  goto :done
  )

netsh advfirewall firewall add rule name="_WCF Test Server PortHttps"  dir=in action=allow profile=any localport=44285 protocol=tcp
set __EXITCODE=%ERRORLEVEL%
if NOT [%__EXITCODE%]==[0] (
  echo Error: error adding rule name _WCF Test Server PortHttps
  goto :done
  )

netsh advfirewall firewall add rule name="_WCF Test Server PortTcp"  dir=in action=allow profile=any localport=8808 protocol=tcp
set __EXITCODE=%ERRORLEVEL%
if NOT [%__EXITCODE%]==[0] (
  echo Error: error adding rule name _WCF Test Server PortTcp
  goto :done
  )

netsh advfirewall firewall add rule name="_WCF Test Server PortWebSocket"  dir=in action=allow profile=any localport=8083 protocol=tcp
set __EXITCODE=%ERRORLEVEL%
if NOT [%__EXITCODE%]==[0] (
  echo Error: error adding rule name _WCF Test Server PortWebSocket
  goto :done
  )

netsh advfirewall firewall add rule name="_WCF Test Server PortWebSockets"  dir=in action=allow profile=any localport=8084 protocol=tcp
set __EXITCODE=%ERRORLEVEL%
if NOT [%__EXITCODE%]==[0] (
  echo Error: error adding rule name _WCF Test Server PortWebSockets
  goto :done
  )

:done
exit /b %__EXITCODE%
