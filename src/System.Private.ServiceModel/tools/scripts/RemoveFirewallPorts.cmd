@if "%_echo%" neq "on" echo off
setlocal

echo Start Remove all ports WCF test services use

SET __EXITCODE=0

net session >nul 2>&1
set __EXITCODE=%ERRORLEVEL%
if NOT [%__EXITCODE%]==[0] (
    echo Error: this script must be called in an elevated window
	goto :done
)

call :DeleteRule "_WCF Test Server PortHttp80"

call :DeleteRule "_WCF Test Server PortHttp"

call :DeleteRule "_WCF Test Server PortHttps"

call :DeleteRule "_WCF Test Server PortTcp"

call :DeleteRule "_WCF Test Server PortWebSocket"

call :DeleteRule "_WCF Test Server PortWebSockets"

goto :done

:DeleteRule
netsh advfirewall firewall delete rule name=%1
set _ErrorLevel=%ERRORLEVEL%
REM errorlevel 1 means the rule is not found, thus we should ignore it.
if NOT [%_ErrorLevel%]==[0] if NOT [%_ErrorLevel%]==[1] (
    echo WARNING: An error occurred while removing firewall rule %1.
    set __EXITCODE=%_ErrorLevel%
  )

goto:eof

:done

exit /b %__EXITCODE%

