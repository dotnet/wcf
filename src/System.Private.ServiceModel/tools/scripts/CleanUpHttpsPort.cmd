@if "%_echo%" neq "on" echo off
setlocal

echo Remove Https configuration

net session >nul 2>&1
set __EXITCODE=%ERRORLEVEL%
if NOT [%__EXITCODE%]==[0] (
    echo Error: this script must be called in an elevated window
	goto :done
)

call :RemoveHttps 0.0.0.0:8084

call :RemoveHttps 0.0.0.0:44285

call :RemoveHttps 0.0.0.0:443

goto :done

:RemoveHttps
netsh http delete sslcert ipport=%1
REM If the rules are not there, we get a errorleve 1, thus errorlevel 1 should be ignored
set _ErrorLevel=%ERRORLEVEL%
if NOT [%_ErrorLevel%]==[0] if NOT [%_ErrorLevel%]==[1] (
    set __EXITCODE=%_ErrorLevel%
    echo WARNING: An error occurred while deleting sslcert from port %1
  )
goto :eof

:done

exit /b %__EXITCODE%
