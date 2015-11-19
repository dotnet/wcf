WCF Features in RC1
===========================

Packages released
------------

[System.ServiceModel.Duplex 4.0.1-beta-23516](http://www.nuget.org/packages/System.ServiceModel.Duplex/4.0.1-beta-23516)    
[System.ServiceModel.Http 4.0.11-beta-23516 ](http://www.nuget.org/packages/System.ServiceModel.Http/4.0.11-beta-23516)  
[System.ServiceModel.Primitives 4.1.0-beta-23516](http://www.nuget.org/packages/System.ServiceModel.Primitives/4.1.0-beta-23516)  
[System.ServiceModel.Security 4.0.1-beta-23516 ](http://www.nuget.org/packages/System.ServiceModel.Security/4.0.1-beta-23516)  
[System.ServiceModel.NetTcp 4.1.0-beta-23516](http://www.nuget.org/packages/System.ServiceModel.NetTcp/4.1.0-beta-23516)  

Supported features
-----------
:white_check_mark: -- Supported with no known issues  
:warning: --  Known issues or not fully tested   
:x: -- Not supported  


:question: -- unknown and requires investigation



|Category | Feature | UWP   |Windows |Linux |Mac OS X  |
|-----|---------|------|--------|------|----------|
|Bindings|BasicHttpBinding|:white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
||CustomBinding|:white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
||NetHttpBinding|:white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
||NetTcpBinding|:white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
|Transports|Http|:white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
||Https|:question: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/438)| :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
||Tcp|:white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
||WebSockets|:warning: [more...](https://github.com/dotnet/wcf/issues/404) | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/420) | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
|Channel types|Request/Reply|:white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
||Duplex|:white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
|Encodings|Text|:white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
||Binary|:white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
|Transfer mode|Buffered, sync & async|:white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
||Streamed, sync & async|:white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
|MessageVersion|SOAP 1.1 UTF8|:white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
||SOAP 1.2 UTF8|:white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
|Contracts | DataContract | :white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
| | XmlSerializer | :white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
| | MessageFormat | :white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
| | FaultContract | :white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
|Security | None | :white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
| | Transport | :white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
| | Client certificates | :x: | :white_check_mark: | :warning: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
|Authentication | Basic | :white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
| | Digest | :white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
| | NTLM | :question: | :white_check_mark: | :x: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
| | Kerberos | :question: | :question: | :x: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
|Client types | ChannelFactory\<T\> | :white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
| | ChannelBase\<T\> | :white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
|Extensibility | IClientMessageInspector | :white_check_mark: | :white_check_mark: | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |
| | IClientMessageFormatter | :warning: [more](https://github.com/dotnet/wcf/issues/535 ) | :warning: [more](https://github.com/dotnet/wcf/issues/535 ) | :warning: [more](https://github.com/dotnet/wcf/issues/535 ) | :warning: [more...](https://github.com/dotnet/wcf/issues/534) |



