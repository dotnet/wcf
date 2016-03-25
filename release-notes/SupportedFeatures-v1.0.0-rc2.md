WCF Features in RC2(In Progress)
===========================

Packages released
------------

[System.ServiceModel.Duplex 4.0.1-beta-23516](http://www.nuget.org/packages/System.ServiceModel.Duplex/4.0.1-beta-23516)    
[System.ServiceModel.Http 4.0.11-beta-23516 ](http://www.nuget.org/packages/System.ServiceModel.Http/4.0.11-beta-23516)  
[System.ServiceModel.Primitives 4.1.0-beta-23516](http://www.nuget.org/packages/System.ServiceModel.Primitives/4.1.0-beta-23516)  
[System.ServiceModel.Security 4.0.1-beta-23516 ](http://www.nuget.org/packages/System.ServiceModel.Security/4.0.1-beta-23516)  
[System.ServiceModel.NetTcp 4.1.0-beta-23516](http://www.nuget.org/packages/System.ServiceModel.NetTcp/4.1.0-beta-23516)  

Supported features
------------
:white_check_mark: -- Works with no known major issues  
:warning: --  Partially works with known issues or only partially tested  
&ensp;&ensp;&ensp;&ensp;&ensp;&ensp;&ensp;**Tip:** Click on warning symbol to see related Issue.  
:no_entry_sign: -- Not Supported  
&ensp;&ensp;&ensp;&ensp;&ensp;&ensp;&ensp;**Tip:** Click on symbol for more details.  
:grey_question: -- Not yet tested

| Category |  Feature  |   [UWP](#uwp)   |  [Win](#windows)  |   [Ubuntu](#ubuntu)   |   [CentOS](#centos)   |   [RedHat](#redhat)   |   [Debian](#debian)   |   [Mac OS X](#mac-os-x)   |
| :------- | :-------- | :------------: | :-------: |  :------:  |   :-----:  |   :-----:  |   :-----:  |   :-----:   |
|Bindings|BasicHttpBinding|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
||CustomBinding|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
||NetHttpBinding|:white_check_mark: | :white_check_mark: | [:warning:] (https://github.com/dotnet/wcf/issues/420) | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
||NetTcpBinding|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :no_entry_sign: |
|Transports|Http|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
||Https| [:warning:] (https://github.com/dotnet/wcf/issues/470) | :white_check_mark: | [:warning:] (https://github.com/dotnet/wcf/issues/438) | :grey_question: | :grey_question: | :grey_question: | :no_entry_sign: |
||Tcp|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :no_entry_sign: |
||WebSockets|[:warning:](https://github.com/dotnet/wcf/issues/526) | :white_check_mark: | [:warning:] (https://github.com/dotnet/wcf/issues/463) | :grey_question: | :grey_question: | :grey_question: |[:warning:] (https://github.com/dotnet/wcf/issues/463) |
|Channel types|Request/Reply|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
||Duplex|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
|Encodings|Text|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
||Binary|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
|Transfer mode|Buffered, sync & async|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
||Streamed, sync & async|[:warning:](https://github.com/dotnet/wcf/issues/470) | [:warning:](https://github.com/dotnet/wcf/issues/470) | [:warning:](https://github.com/dotnet/wcf/issues/470) | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
|MessageVersion|SOAP 1.1 UTF8|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
||SOAP 1.2 UTF8|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
|Contracts | DataContract | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
| | XmlSerializer | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
| | MessageFormat | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
| | FaultContract | [:warning:](https://github.com/dotnet/wcf/issues/769) | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
|Security | None | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
| | Transport | :white_check_mark: | :white_check_mark:  | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
| | Message | [:no_entry_sign:](#message-security) | [:no_entry_sign:](#message-security) | [:no_entry_sign:](#message-security) | [:no_entry_sign:](#message-security) | [:no_entry_sign:](#message-security) | [:no_entry_sign:](#message-security) | [:no_entry_sign:](#message-security) |
| | Client certificates | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :grey_question: |
|Authentication | Basic | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :no_entry_sign: |
| | Digest | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :grey_question: |
| | NTLM | :white_check_mark: | :white_check_mark: | [:no_entry_sign:](#ntlm-authentication) | :grey_question: | :grey_question: | [:no_entry_sign:](#ntlm-authentication) | :no_entry_sign: |
| | Kerberos | :white_check_mark: | :white_check_mark: | [:no_entry_sign:](#kerberos-authentication) | :grey_question: | :grey_question: | [:no_entry_sign:](#kerberos-authentication) | :no_entry_sign: |
|Client types | ChannelFactory\<T\> | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
| | ChannelBase\<T\> | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
|Extensibility | IClientMessageInspector | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
| | IClientMessageFormatter | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |


Platforms
------------

#### UWP
The Universal Windows Platform (UWP) is the platform that is used for building modern, touch-enabled Windows applications as well as headless devices for Internet of Things (IoT). It's designed to unify the different types of devices that you may want to target, including PCs, tablets, phablets, phones, and even the Xbox.  

UWP provides many services, such as a centralized app store, an execution environment (AppContainer), and a set of Windows APIs to use instead of Win32 (WinRT). UWP has no dependency on .NET; apps can be written in C++, C#, VB.NET, and JavaScript. When using C# and VB.NET the .NET APIs are provided by .NET Core.  
> **Source:** https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/glossary.md

#### Windows
Supported Versions...  
&ensp;&ensp;&ensp;> Windows 7  
&ensp;&ensp;&ensp;> Windows 8.1  
&ensp;&ensp;&ensp;> Windows 10  

#### Ubuntu
Supported Versions...  
&ensp;&ensp;&ensp;> Ubuntu 14.04  

#### CentOS
Supported Versions...  
&ensp;&ensp;&ensp;> CentOS 7.1  

#### RedHat
Supported Versions...  
&ensp;&ensp;&ensp;> RedHat 7.2  

#### Debian
Supported Versions...  
&ensp;&ensp;&ensp;> Debian 8.2  

#### Mac OS X
Supported Versions...  
&ensp;&ensp;&ensp;> Mac OS X 10.11  

Not Supported Features
------------

#### Message Security
Fill in with details.  

#### NTLM Authentication
&ensp;&ensp; Support is built into CentOS, RedHat and Mac OS X but it hasn't been tested yet.  
&ensp;&ensp; Support is not built in to Ubuntu and Debian by default.  
&ensp;&ensp; For more details see [Issue #943](https://github.com/dotnet/wcf/issues/943)  

#### Kerberos Authentication
&ensp;&ensp; Support is built into CentOS, RedHat and Mac OS X but it hasn't been tested yet.  
&ensp;&ensp; Support is not built in to Ubuntu and Debian by default.  
&ensp;&ensp;&ensp;&ensp; A solution has been tested on Ubuntu that works if an SPN is specified.  
&ensp;&ensp;&ensp;&ensp; The User2User scenario (i.e., when a UPN is specified) is not yet supported.  