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
:white_check_mark: -- Works with no known major issues  
:warning: --  Partially works with known issues or only partially tested   
:x: -- Does not work  
:question: -- Not yet tested



|Category | Feature | UWP   |Windows |Linux |Mac OS X  |
|-----|---------|------|--------|------|----------|
|Bindings|BasicHttpBinding|:white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
||CustomBinding|:white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
||NetHttpBinding|:white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
||NetTcpBinding|:white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
|Transports|Http|:white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
||Https|:warning: [more...](https://github.com/dotnet/wcf/issues/470) | :white_check_mark: | :warning: [more...](https://github.com/dotnet/wcf/issues/438)| :question: |
||Tcp|:white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
||WebSockets|:warning: [more...](https://github.com/dotnet/wcf/issues/404) | :warning: [more...](https://github.com/dotnet/wcf/issues/468)| :warning: [more...](https://github.com/dotnet/wcf/issues/420) | :question: |
|Channel types|Request/Reply|:white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
||Duplex|:white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
|Encodings|Text|:white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
||Binary|:white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
|Transfer mode|Buffered, sync & async|:white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
||Streamed, sync & async|:white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
|MessageVersion|SOAP 1.1 UTF8|:white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
||SOAP 1.2 UTF8|:white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
|Contracts | DataContract | :warning:[more...](https://github.com/dotnet/wcf/issues/325) | :white_check_mark: | :white_check_mark: | :question: |
| | XmlSerializer | :warning:[more...](https://github.com/dotnet/wcf/issues/555) | :white_check_mark: | :white_check_mark: | :question: |
| | MessageFormat | :white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
| | FaultContract | :white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
|Security | None | :white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
| | Transport | :warning:[more...](https://github.com/dotnet/wcf/issues/458) | :warning:[more...](https://github.com/dotnet/wcf/issues/458)  | :warning: [more...](https://github.com/dotnet/wcf/issues/458) | :question: |
| | Message | :x: | :x: | :x: | :x: |
| | Client certificates | :x: | :x: | :x: | :question:|
|Authentication | Basic | :white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
| | Digest | :white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
| | NTLM | :warning: [more...](https://github.com/dotnet/wcf/issues/568) | :white_check_mark: | :x: | :question: |
| | Kerberos | :warning:[more...](https://github.com/dotnet/wcf/issues/568) | :white_check_mark: | :x: | :question: |
|Client types | ChannelFactory\<T\> | :white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
| | ChannelBase\<T\> | :white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
|Extensibility | IClientMessageInspector | :white_check_mark: | :white_check_mark: | :white_check_mark: | :question: |
| | IClientMessageFormatter | :warning: [more](https://github.com/dotnet/wcf/issues/535 ) | :warning: [more](https://github.com/dotnet/wcf/issues/535 ) | :warning: [more](https://github.com/dotnet/wcf/issues/535 ) | :question: |
