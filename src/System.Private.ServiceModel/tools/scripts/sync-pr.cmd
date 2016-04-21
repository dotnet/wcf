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
    echo     Usage:  %~n0 [id] [branch/pr] [sync-url] [optional branch-name / pr-number]
    echo   
    echo   id          - ID of the repo on the PR Service
    echo   branch/pr   - Sync to branch or PR [choose one]
    echo   sync-url    - URL on remote server for PR synchronization
    echo   branch-name - branch name to sync to
    echo   pr-number   - PR number to sync to
    echo.
    echo   If the branch-name or pr-number are left blank, then the script will use 
    echo   the GIT_BRANCH or ghprbPullId environment variable depending on the operation mode
    echo.
    echo   Example:  %~n0 1 branch http://wcfci-sync-server/PRService/pr.ashx
    echo   Example:  %~n0 2 branch http://wcfci-sync-server/PRService/pr.ashx master
    echo   Example:  %~n0 3 pr http://wcfci-sync-server/PRService/pr.ashx
    echo   Example:  %~n0 4 pr http://wcfci-sync-server/PRService/pr.ashx 404
    set __EXITCODE=1
    goto done
)

set __REPO_ID=%1

set __OPERATION_MODE=%2

if not "%__OPERATION_MODE%"=="branch" if not "%__OPERATION_MODE%"=="pr" (
    echo   Invalid operation mode specified 
    goto done
)

set __SYNC_HOST_URL=%3

if "%4"=="" (
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
    set __BRANCH_OR_PR_ID=%4
    echo   [%~n0] WARNING: This script should usually only be called only from the context of a GitHub Pull Request
    echo   [%~n0] Branch or PR ID overridden: '%4'
) 

REM Disregard the chevrons and quotataions in the next line - it's to output the correct ampersand character to PowerShell
REM We have to duplicate the request URL below for output and for PowerShell because of Powershell's crazy escape sequences interacting 
REM badly with cmd's crazy and conflicting escape sequences

echo   [%~n0] Making call to %__SYNC_HOST_URL%?id=%__REPO_ID%^&%__OPERATION_MODE%=%__BRANCH_OR_PR_ID%
powershell -NoProfile -ExecutionPolicy unrestricted -Command "(New-Object Net.WebClient).DownloadString('%__SYNC_HOST_URL%?id=%__REPO_ID%&%__OPERATION_MODE%=%__BRANCH_OR_PR_ID%');"

set __EXITCODE=%ERRORLEVEL% 

:done
endlocal
exit /b %__EXITCODE% 

