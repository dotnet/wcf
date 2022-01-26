FROM mcr.microsoft.com/dotnet/framework/wcf
COPY . /wcf

RUN dir
WORKDIR /wcf
RUN dir

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'Continue'; $verbosePreference='Continue';"]
ENTRYPOINT ["powershell"]
CMD src\System.Private.ServiceModel\tools\scripts\SetupWcfIISHostedService.cmd 38 /p:"c:\wcf"; powershell
