@ECHO OFF

"%ProgramFiles%\Docker\Docker\DockerCli.exe" -SwitchWindowsEngine
docker rm -f svcutil-test-container

pushd %~dp0\TestServices

docker build -t svcutil-test-service .
docker run -d -p 8080:8080 --name svcutil-test-container svcutil-test-service

popd

echo Container running at the following IP Address:
docker inspect --format="{{.NetworkSettings.Networks.nat.IPAddress}}" svcutil-test-container