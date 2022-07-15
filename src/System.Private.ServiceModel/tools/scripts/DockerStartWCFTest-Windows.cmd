echo off
setlocal
echo **********************************
echo Starting WCF Client on Docker
echo **********************************

set _exitCode=0

:: Make sure docker is running 
docker ps>nul 2>&1
if ERRORLEVEL 1 (
    echo. & echo ERROR: Please make sure docker is running.
    goto :Failure
)

echo. & echo Checking WCF service container status and IP address.

::Switch to Windows Containers to check WCF Service status
CALL "C:\Program Files\Docker\Docker\DockerCli.exe" -SwitchWindowsEngine

:: Check if WCF service is already running 
docker container inspect -f '{{.State.Status}}' WCFServiceContainer
if ERRORLEVEL 1 (
    echo. & echo Warning: WCF Service container is not running. 
    CALL DockerStartWCFService.cmd
)

:: set ServiceUri
FOR /F "tokens=* USEBACKQ" %%F IN (`docker inspect --format="{{.NetworkSettings.Networks.nat.IPAddress}}" WCFServiceContainer`) DO (
SET ServiceUri=%%F/WcfService38
)
echo %ServiceUri%

:: change directory to wcf src
cd ../../../..

echo. & echo Building Docker image for WCF client

:: Building docker image.
docker build -f ./Docker/Client/Windows.dockerfile --tag=wcf:client . 
if ERRORLEVEL 1 (
    echo. & echo ERROR: Building docker image failed.
    goto :Failure
)
echo. & echo Building image success..

:: Starting docker container from the image.

echo. & echo Starting WCF client
docker run --name wcfclient_container --rm -it --memory=2g -v "%cd%":"C:\wcf" -e ServiceUri=%ServiceUri% wcf:client
if ERRORLEVEL 1 ( 
    echo. & echo ERROR: Starting WCF client container failed.
    goto :Failure
)


exit /b

:Failure
echo. & echo Error...
set _exitCode=1

:Done
exit /b %_exitCode%
endlocal