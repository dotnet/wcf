WCF Features in RC2
===========================

Packages released
------------

[System.ServiceModel.Duplex 4.0.1-rc2-*****](<NEED LINK>)    
[System.ServiceModel.Http 4.0.11-rc2-*****](<NEED LINK>)  
[System.ServiceModel.Primitives 4.1.0-rc2-*****](<NEED LINK>)  
[System.ServiceModel.Security 4.0.1-rc2-*****](<NEED LINK>)  
[System.ServiceModel.NetTcp 4.1.0-rc2-*****](<NEED LINK>)  

Supported features
------------
:white_check_mark: -- Works with no known major issues  
:warning: --  Partially works with known issues or only partially tested  
&ensp;&ensp;&ensp;&ensp;&ensp;&ensp;&ensp;**Tip:** Click on symbol for more details.  
:no_entry_sign: -- Not Supported  
&ensp;&ensp;&ensp;&ensp;&ensp;&ensp;&ensp;**Tip:** Click on symbol for more details.  
:grey_question: -- Not yet tested

| Category |  Feature  |   [UWP](#uwp)   |  [Win](#windows)  |   [Ubuntu](#ubuntu)   |   [CentOS](#centos)   |   [RedHat](#redhat)   |   [Debian](#debian)   |   [Mac OS X](#mac-os-x)   |
| :------- | :-------- | :------------: | :-------: |  :------:  |   :-----:  |   :-----:  |   :-----:  |   :-----:   |
|Bindings|BasicHttpBinding|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
||CustomBinding|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
||NetHttpBinding|:white_check_mark: | :white_check_mark: | [:warning:](#bindings_nethttpbinding) | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
||NetTcpBinding|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | [:no_entry_sign:](#NetTcp) |
|Transports|Http|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
||Https| [:warning:](#transports_https) | :white_check_mark: | [:warning:](#transports_https) | :grey_question: | :grey_question: | :grey_question: | [:warning:](#transports_https) |
||Tcp|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | [:no_entry_sign:](#NetTcp) |
||WebSockets|[:warning:](#transports_websockets) | :white_check_mark: | [:warning:](#transports_websockets) | :grey_question: | :grey_question: | :grey_question: | :grey_question: |
|Channel types|Request/Reply|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
||Duplex|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
|Encodings|Text|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
||Binary|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
|TransferMode|Buffered (sync/async)|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
||Streamed (sync/async)|[:warning:](#transfermode_streamed) | [:warning:](#transfermode_streamed) | [:warning:](#transfermode_streamed) | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
|MessageVersion|SOAP 1.1 UTF8|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
||SOAP 1.2 UTF8|:white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
|Contracts | DataContract | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
| | XmlSerializer | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
| | MessageFormat | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
| | FaultContract | [:warning:](#contracts_faultcontract) | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
|Security | None | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
| | Transport | :white_check_mark: | :white_check_mark:  | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
| | Message | [:no_entry_sign:](#security_message) | [:no_entry_sign:](#security_message) | [:no_entry_sign:](#security_message) | [:no_entry_sign:](#security_message) | [:no_entry_sign:](#security_message) | [:no_entry_sign:](#security_message) | [:no_entry_sign:](#security_message) |
| | Client certificates | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :grey_question: |
|Authentication | Basic | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :grey_question: |
| | Digest | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :grey_question: |
| | NTLM | :white_check_mark: | :white_check_mark: | [:no_entry_sign:](#ntlm-authentication) | :grey_question: | :grey_question: | [:no_entry_sign:](#ntlm-authentication) | :grey_question: |
| | Kerberos | :white_check_mark: | :white_check_mark: | [:no_entry_sign:](#kerberos-authentication) | :grey_question: | :grey_question: | [:no_entry_sign:](#kerberos-authentication) | :grey_question: |
|Client types | ChannelFactory\<T\> | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
| | ChannelBase\<T\> | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
|Extensibility | IClientMessageInspector | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
| | IClientMessageFormatter | :white_check_mark: | :white_check_mark: | :white_check_mark: | :grey_question: | :grey_question: | :grey_question: | :white_check_mark: |
|Management | ETW Tracing | [:no_entry_sign:](#etw-tracing) | :white_check_mark: | [:no_entry_sign:](#etw-tracing) | [:no_entry_sign:](#etw-tracing) | [:no_entry_sign:](#etw-tracing) | [:no_entry_sign:](#etw-tracing) | [:no_entry_sign:](#etw-tracing) |


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

Partially Working Features
------------
:bangbang: *Linked issues are open in RC-2 time-frame, as they are closed look for the fixes in the next release.*  
#### Bindings_NetHttpBinding  

> **On Platform:** *Ubuntu*  
>&ensp;&ensp; Active issues: https://github.com/dotnet/wcf/issues/420

#### Transports_Https  
> **On Platform:** *UWP/Windows/Ubuntu*  
>&ensp;&ensp;&ensp;&ensp; Active Issue: https://github.com/dotnet/corefx/issues/4429  
>&ensp;&ensp;&ensp;&ensp; This is the same core issue as listed for the Transports_WebSockets and TransferMode_Streamed features.  

> **On Platform:** *OS X*  
>&ensp;&ensp;&ensp;&ensp; Active Issue: https://github.com/dotnet/wcf/issues/954  
>&ensp;&ensp;&ensp;&ensp; This is most likely a machine setup issue.

#### Transports_WebSockets  
> **On Platform:** *UWP/Windows/Ubuntu*  
>&ensp;&ensp;&ensp;&ensp; Active Issue: https://github.com/dotnet/corefx/issues/4429  
>&ensp;&ensp;&ensp;&ensp; This is the same core issue as listed for the Transports_Https and TransferMode_Streamed features.  

#### TransferMode_Streamed  
> **On Platform:** *UWP/Windows/Ubuntu*  
>&ensp;&ensp;&ensp;&ensp; Active Issue: https://github.com/dotnet/corefx/issues/4429  
>&ensp;&ensp;&ensp;&ensp; This is the same core issue as listed for the Transports_Https and Transports_WebSockets features.  

#### Contracts_FaultContract  
> **On Platform:** *UWP*  
>&ensp;&ensp;&ensp;&ensp; Active Issue: https://github.com/dotnet/wcf/issues/769  

Not Supported Features
------------

#### Security_Message  
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

#### NetTcp  
&ensp;&ensp; SetLingerOption isn't working correctly on OS X. This is only used on the Close(timeout) code path so the test itself is likely passing. This api required special handling on linux as this is a windows socket concept.  
&ensp;&ensp;&ensp;&ensp; Active Issue: https://github.com/dotnet/corefx/issues/7403  

#### ETW Tracing
&ensp;&ensp; ETW Tracing has been enabled for Windows in this release, support on other platforms will be available in future releases.