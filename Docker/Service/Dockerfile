FROM mcr.microsoft.com/dotnet/framework/wcf

# IIS Hosted
EXPOSE 80 808 443 44345

# Self Hosted
EXPOSE 8081 8083 8084 44285 809

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'Continue'; $verbosePreference='Continue';"]
ENTRYPOINT ["powershell"]

# Setup WCF web hosted and self hosted services - Container Startup CMD
CMD ./wcf/src/System.Private.ServiceModel/tools/scripts/SetupWcfIISHostedService.cmd 38 /p:"c:\wcf"; ./wcf/src/System.Private.ServiceModel/tools/scripts/StartWCFSelfHostedSvc.cmd; powershell
