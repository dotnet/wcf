@ECHO OFF

"%ProgramFiles%\Docker\Docker\DockerCli.exe" -SwitchWindowsEngine
docker rm -f svcutil-test-container

echo "Services stopped."