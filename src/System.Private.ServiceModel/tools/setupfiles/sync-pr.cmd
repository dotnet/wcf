@echo off 
setlocal

REM 
REM This notifies a the test web service, specified on the command line,
REM that a test is about to start
REM This script is meant to be called from a GitHub Pull Request. 
REM 

set __args=%*

echo.
echo  WCF Test Service Pull Request sync script
echo. 

if "__args%"=="" (
    echo   [%~n0] A URL must be specified for the pull request synchronization URL
    echo     Usage:  %~n0 [sync url] [optional pr-number]
    echo   sync-url  - URL on remote server for PR synchronization
    echo   pr-number - PR number to sync to
    echo.
    echo   Example:  %~n0 http://wcfci-sync-server/repo/sync.ashx 
    echo   Example:  %~n0 http://wcfci-sync-server/repo/sync.ashx 404
    set __EXITCODE=1
    goto done
)

set __SYNC_HOST_URL=%1

if "%2"=="" (
    if '%ghprbPullId%'=='' (
        echo   [%~n0] This script should only be called only from the context of a GitHub Pull Request
        echo   [%~n0] 'ghprbPullId' environment variable not set
        set __EXITCODE=1
        goto done
    ) else ( 
        set __PR_ID=%ghprbPullId%
    )
) else (
    set __PR_ID=%2
    echo   [%~n0] WARNING: This script should usually only be called only from the context of a GitHub Pull Request
    echo   [%~n0] PR ID overridden: '%2'
) 

set __REQUEST_URL=%__SYNC_HOST_URL%?pr=%__PR_ID%
echo   [%~n0] Making call to '%__REQUEST_URL%'
powershell -NoProfile -ExecutionPolicy unrestricted -Command "(New-Object Net.WebClient).DownloadString('%__REQUEST_URL%'); 

set __EXITCODE=%ERRORLEVEL% 

:done
endlocal
exit /b %__EXITCODE% 

