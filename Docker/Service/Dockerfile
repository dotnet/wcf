# Windows Docker image - Host WCF Service
FROM mcr.microsoft.com/dotnet/framework/wcf

# Open IIS Hosted ports
EXPOSE 80 808 443 44345

# Open Self Hosted ports
EXPOSE 8081 8083 8084 44285 809

# Set Powershell as default Container shell
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'Continue'; $verbosePreference='Continue';"]

# Set ENTRYPOINT to Powershell
# Keeps the Service Container always running
ENTRYPOINT ["powershell"]

# Container Startup command:
# Setup WCF IIS Hosted and Self Hosted Services, using Docker Volume at C:\wcf
CMD ./wcf/src/System.Private.ServiceModel/tools/scripts/SetupWcfIISHostedService.cmd 38 /p:"c:\wcf"; ./wcf/src/System.Private.ServiceModel/tools/scripts/StartWCFSelfHostedSvc.cmd; powershell
