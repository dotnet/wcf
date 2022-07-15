FROM mcr.microsoft.com/dotnet/framework/sdk:4.8

SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'Continue'; $verbosePreference='Continue';"]

CMD C:\wcf\src\System.Private.ServiceModel\tools\scripts\SetClientEnv-Windows.cmd; C:\wcf\eng\common\cibuild.cmd -configuration Release -prepareMachine /p:Restore=false /p:Sign=false /p:Test=true /p:IntegrationTest=true /p:Pack=false /p:Publish=false; 


