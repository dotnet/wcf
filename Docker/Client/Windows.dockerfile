# Windows Docker image - Host WCF Client
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8

# Set Powershell as default Container shell
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'Continue'; $verbosePreference='Continue';"]

# Run commands:
# Setup environment variables
# Run WCF Tests
CMD C:\wcf\src\System.Private.ServiceModel\tools\scripts\SetClientEnv-Windows.cmd; C:\wcf\eng\common\cibuild.cmd -configuration Release -prepareMachine /p:Restore=true /p:Sign=false /p:Test=true /p:IntegrationTest=true /p:Pack=false /p:Publish=false; 


