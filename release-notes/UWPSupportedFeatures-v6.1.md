WCF Features in UWP 6.1
======================================================
*WCF UWP 6.1 is part of .NET native tools 2.1 (UWP6.1). Please see release notes [here](https://github.com/Microsoft/dotnet/blob/master/releases/UWP/net-native2.1/README.md). 

Supported features
------------
:white_check_mark: -- Works with no known major issues  
:warning: --  Partially works with known issues or only partially tested  
:no_entry_sign: -- Not Supported  

| Category     |  Feature              |   [UWP](#platforms)                       |
| :-------     | :--------             | :------------:                      |
|Bindings      |BasicHttpBinding       |:white_check_mark:                   |
|              |CustomBinding          |:white_check_mark:                   |
|              |NetHttpBinding         |:white_check_mark:                   |
|              |NetTcpBinding          |:white_check_mark:                   |
|Transports    |Http                   |:white_check_mark:                   |
|              |Https                  |:white_check_mark:                   |
|              |Tcp                    |:white_check_mark:                   |
|              |WebSockets             |[:warning:](https://github.com/dotnet/wcf/releases/tag/v2.0.0)           |
|Channel types |Request/Reply          |:white_check_mark:                   |
|              |Duplex                 |:white_check_mark:                   |
|              |Custom                 |:white_check_mark:                   |
|Encodings     |Text                   |:white_check_mark:                   |
|              |Binary                 |:white_check_mark:                   |
|TransferMode  |Buffered (sync/async)  |:white_check_mark:                   |
|              |Streamed (sync/async)  |:white_check_mark:                   |
|MessageVersion|SOAP 1.1 UTF8          |:white_check_mark:                   |
|              |SOAP 1.2 UTF8          |:white_check_mark:                   |
|Contracts     |DataContract           |:white_check_mark:                   |
|              |XmlSerializer          |:white_check_mark:                   |
|              |MessageFormat          |:white_check_mark:                   |
|              |FaultContract          |:white_check_mark:                   |
|WSDL Operation Format     |Literal           |:white_check_mark:                   |
|              |Encoded          |[:no_entry_sign:](https://github.com/dotnet/wcf/releases/tag/v2.0.0)                   |
|Security      |None                   |:white_check_mark:                   |
|              |Transport              |:white_check_mark:                   |
|              |Message                |[:no_entry_sign:](https://github.com/dotnet/wcf/releases/tag/v2.0.0)     |
|Client Authentication|Basic           |:white_check_mark:                   |
|              |Digest                 |:white_check_mark:                   |
|              |NTLM                   |:white_check_mark:                   |
|              |Kerberos/TCP           |[:no_entry_sign:](https://github.com/dotnet/wcf/releases/tag/v2.0.0)     |
|              |Kerberos/HTTP          |:white_check_mark:                   |
|              |Certificate/TCP       |:white_check_mark:                   |
|              |Certificate/HTTP      |:white_check_mark:                   |
|              |Certificate/PeerTrust      |:white_check_mark:                   |
|Service Authentication|Certificate    |:white_check_mark:                   |
|              |SPNIdentity/TCP    |[:no_entry_sign:](https://github.com/dotnet/wcf/releases/tag/v2.0.0)          |
|              |SPNIdentity/HTTP    |[:no_entry_sign:](https://github.com/dotnet/wcf/releases/tag/v2.0.0)          |
|              |UPNIdentity    |[:no_entry_sign:](https://github.com/dotnet/wcf/releases/tag/v2.0.0)          |
|Client types  |ChannelFactory\<T\>    |:white_check_mark:                   |
|              |ChannelBase\<T\>       |:white_check_mark:                   |
|Proxy Support  |Http&Https    |[:no_entry_sign:](https://github.com/dotnet/wcf/releases/tag/v2.0.0)                   |
|Extensibility |IClientMessageInspector|:white_check_mark:                   |
|              |IClientMessageFormatter|:white_check_mark:                   |
|Management    |ETW Tracing            |[:no_entry_sign:](https://github.com/dotnet/wcf/releases/tag/v2.0.0)      |        


#### More Information
&ensp;&ensp;For features supported on Windows, Linux OSs and macOS, please see [feature list](https://github.com/dotnet/wcf/blob/master/release-notes/SupportedFeatures-v2.1.0.md).
