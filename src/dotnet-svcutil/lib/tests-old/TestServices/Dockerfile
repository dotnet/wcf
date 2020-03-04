FROM microsoft/wcf:4.6.2
RUN windows\system32\inetsrv\appcmd.exe set app 'Default Web Site/' /enabledProtocols:"http,net.tcp"
EXPOSE 808
WORKDIR /inetpub/wwwroot
COPY . .

RUN ["powershell", "-Command", "/inetpub/wwwroot/setupServices.ps1"]