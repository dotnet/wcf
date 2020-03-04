# This script is called from the dockerfile and executed inside the container to set up the needed test services.

Import-Module WebAdministration
New-WebBinding -name "Default Web Site" -port 8080 -Protocol http -IPAddress "*"
New-WebBinding -name "Default Web Site" -port 4430 -Protocol https -IPAddress "*"
ConvertTo-WebApplication -PsPath "IIS:\Sites\Default Web Site\WcfForSvcUtil"
ConvertTo-WebApplication -PsPath "IIS:\Sites\Default Web Site\WcfProjectNService"
Set-ItemProperty "IIS:\Sites\Default Web Site\WcfProjectNService" -Name enabledProtocols -value "http,net.tcp"
