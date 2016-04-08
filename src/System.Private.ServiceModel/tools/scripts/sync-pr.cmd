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

if "%__args%"=="" (
    echo   [%~n0] A URL must be specified for the pull request synchronization URL
    echo     Usage:  %~n0 [branch/pr] [sync-url] [optional branch-name / pr-number]
    echo   
    echo   branch/pr   - Sync to branch or PR [choose one]
    echo   sync-url    - URL on remote server for PR synchronization
    echo   branch-name - branch name to sync to
    echo   pr-number   - PR number to sync to
    echo.
    echo   Example:  %~n0 branch http://wcfci-sync-server/PRService1/pr.ashx 
    echo   Example:  %~n0 branch http://wcfci-sync-server/PRService1/pr.ashx master
    echo   Example:  %~n0 pr http://wcfci-sync-server/PRService1/pr.ashx 
    echo   Example:  %~n0 pr http://wcfci-sync-server/PRService1/pr.ashx 404
    set __EXITCODE=1
    goto done
)

set __OPERATION_MODE=%1

if not "%__OPERATION_MODE%"=="branch" if not "%__OPERATION_MODE%"=="pr" (
    echo   Invalid operation mode specified 
    goto done
)

set __SYNC_HOST_URL=%2

if "%3"=="" (
    if '%__OPERATION_MODE%'=='branch' (
        if '%GIT_BRANCH%'=='' (
            echo   [%~n0] This script should only be called only from the context of a GitHub Pull Request
            echo   [%~n0] 'GIT_BRANCH' environment variable not set and no branch specified
            set __EXITCODE=1
            goto done
        ) else ( 
            set __BRANCH_OR_PR_ID=%GIT_BRANCH%
        )
    ) else if '%__OPERATION_MODE%'=='pr' (
        if '%ghprbPullId%'=='' (
            echo   [%~n0] This script should only be called only from the context of a GitHub Pull Request
            echo   [%~n0] 'ghprbPullId' environment variable not set and no PR ID specified
            set __EXITCODE=1
            goto done
        ) else ( 
            set __BRANCH_OR_PR_ID=%ghprbPullId%
        )
    )
) else (
    set __BRANCH_OR_PR_ID=%3
    echo   [%~n0] WARNING: This script should usually only be called only from the context of a GitHub Pull Request
    echo   [%~n0] Branch or PR ID overridden: '%3'
) 

set __REQUEST_URL=%__SYNC_HOST_URL%?%__OPERATION_MODE%=%__BRANCH_OR_PR_ID%

echo   [%~n0] Making call to '%__REQUEST_URL%'
powershell -NoProfile -ExecutionPolicy unrestricted -Command "(New-Object Net.WebClient).DownloadString('%__REQUEST_URL%'); 

set __EXITCODE=%ERRORLEVEL% 

:done
endlocal
exit /b %__EXITCODE% 

