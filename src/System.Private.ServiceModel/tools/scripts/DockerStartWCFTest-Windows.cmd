echo off
setlocal
echo **********************************
echo Starting WCF Client on Docker
echo **********************************

:: Use this script to start docker Windows container for WCF Client

:: Variables
set _exitCode=0
set "TestContainerName=WCFTestContainerWin"
set "ServiceContainerName=WCFServiceContainer"
set "ServiceHostName=wcfservicehost"

:: Check if docker is running 
docker ps>nul 2>&1
if ERRORLEVEL 1 (
    echo. & echo ERROR: Please make sure docker is running.
    goto :Failure
)

echo. & echo Checking WCF service container status and IP address.

:: Switch to Windows Containers to check WCF Service status
START /B /WAIT "" "C:\Program Files\Docker\Docker\DockerCli.exe" -SwitchWindowsEngine

:: Check if WCF service is already running 
docker container inspect -f '{{.State.Status}}' %ServiceContainerName%
if ERRORLEVEL 1 (
    echo. & echo Warning: WCF Service container is not running. 
    CALL DockerStartWCFService.cmd
)

:: set ServiceUri
FOR /F "tokens=* USEBACKQ" %%F IN (`docker inspect --format="{{.NetworkSettings.Networks.nat.IPAddress}}" %ServiceContainerName%`) DO (
:: SET ServiceIPAddress=%%F
SET ServiceUri=%ServiceHostName%/WcfService38
)
echo %ServiceUri%

:: Change directory to wcf src
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
:: run docker container and mount current directory for wcf source as container volume C:\wcf
docker run --name %TestContainerName% --rm -it --memory=4g -v "%cd%":"C:\wcf" -e ServiceUri=%ServiceUri% wcf:client
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
