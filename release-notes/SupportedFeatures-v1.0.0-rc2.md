WCF Features in RC2
===========================
*This is part of [WCF release notes](https://github.com/dotnet/wcf/releases/tag/v1.0.0-rc2) for .NET Core 1.0 RC2. Please check out release notes for what's new since RC1 release.*

Table of Contents
------------
[Supported Features](#supported-features)  
[Platforms](#platforms)  
[Partially Working Features](#partially-working-features)  
[Not Supported Features](#not-supported-features)  

Supported features
------------
:white_check_mark: -- Works with no known major issues  
:warning: --  Partially works with known issues or only partially tested  
&ensp;&ensp;&ensp;&ensp;&ensp;&ensp;&ensp;**Tip:** Click on symbol for more details.  
:no_entry_sign: -- Not Supported  
&ensp;&ensp;&ensp;&ensp;&ensp;&ensp;&ensp;**Tip:** Click on symbol for more details.  
:grey_question: -- Not yet tested

| Category     |  Feature              |   [UWP](#uwp)                      |  [Win](#all-other-platforms)       |   [Ubuntu](#all-other-platforms)          |   [CentOS](#all-other-platforms)       |   [RedHat](#all-other-platforms)    |   [Debian](#all-other-platforms)          |   [Mac OS X](#all-other-platforms)  |
| :-------     | :--------             | :------------:                     | :-------:                          |  :------:                                 |   :-----:                              |   :-----:                           |   :-----:                                 |   :-----:                           |
|Bindings      |BasicHttpBinding       |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|              |CustomBinding          |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|              |NetHttpBinding         |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |[:warning:](#bindings_nethttpbinding)      |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|              |NetTcpBinding          |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | [:no_entry_sign:](#nettcp)          |
|Transports    |Http                   |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|              |Https                  |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |[:warning:](#transports_https)             |:grey_question:                         | :grey_question:                     |:grey_question:                            | [:warning:](#transports_https)      |
|              |Tcp                    |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | [:no_entry_sign:](#nettcp)          |
|              |WebSockets             |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |[:warning:](#transports_websockets)        |:grey_question:                         | :grey_question:                     |:grey_question:                            | :grey_question:                     |
|Channel types |Request/Reply          |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|              |Duplex                 |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|Encodings     |Text                   |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|              |Binary                 |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|TransferMode  |Buffered (sync/async)  |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|              |Streamed (sync/async)  |[:no_entry_sign:](#uwp)             |[:warning:](#transfermode_streamed) |[:warning:](#transfermode_streamed)        |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|MessageVersion|SOAP 1.1 UTF8          |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|              |SOAP 1.2 UTF8          |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|Contracts     |DataContract           |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|              |XmlSerializer          |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|              |MessageFormat          |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|              |FaultContract          |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|Security      |None                   |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|              |Transport              |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:grey_question:                         | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|              |Message                |[:no_entry_sign:](#security_message)|[:no_entry_sign:](#security_message)|[:no_entry_sign:](#security_message)       |[:no_entry_sign:](#security_message)    | [:no_entry_sign:](#security_message)|[:no_entry_sign:](#security_message)       | [:no_entry_sign:](#security_message)|
|              |Client certificates    |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:grey_question:                         | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|Authentication|Basic                  |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:grey_question:                            |:grey_question:                         | :grey_question:                     |:grey_question:                            | :grey_question:                     |
|              |Digest                 |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:grey_question:                            |:grey_question:                         | :grey_question:                     |:grey_question:                            | :grey_question:                     |
|              |NTLM                   |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |[:no_entry_sign:](#ntlm-authentication)    |:grey_question:                         | :grey_question:                     |[:no_entry_sign:](#ntlm-authentication)    | :grey_question:                     |
|              |Kerberos               |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |[:warning:](#kerberos-authentication)      |:grey_question:                         | :grey_question:                     |[:no_entry_sign:](#kerberos-authentication)| :white_check_mark:                  |
|Client types  |ChannelFactory\<T\>    |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|              |ChannelBase\<T\>       |[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|Extensibility |IClientMessageInspector|[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|              |IClientMessageFormatter|[:no_entry_sign:](#uwp)             |:white_check_mark:                  |:white_check_mark:                         |:white_check_mark:                      | :grey_question:                     |:grey_question:                            | :white_check_mark:                  |
|Management    |ETW Tracing            |[:no_entry_sign:](#etw-tracing)     |:white_check_mark:                  |[:no_entry_sign:](#etw-tracing)            |[:no_entry_sign:](#etw-tracing)         | [:no_entry_sign:](#etw-tracing)     |[:no_entry_sign:](#etw-tracing)            | [:no_entry_sign:](#etw-tracing)     |


Platforms
------------

#### UWP
For more details about what UWP is please see this [dotnet Glossary doc](https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/glossary.md).

:bangbang: *.NET Core 1.0 RC2 does not work in UWP projects.*  

#### All Other Platforms
For supported versions of each platform and other details please see the [.NET Core 1.0 RC2 Release Notes](https://github.com/dotnet/core/blob/master/release-notes/1.0/Release-Notes-RC2.md)  

Partially Working Features
------------
:bangbang: *Linked issues are open in RC-2 time-frame, as they are closed look for the fixes in the next release.*  
#### Bindings_NetHttpBinding  

> **On Platform:** *Ubuntu*  
>&ensp;&ensp; Active issues: [Issue #420](https://github.com/dotnet/wcf/issues/420)

#### Transports_Https  
> **On Platform:** *UWP/Windows/Ubuntu*  
>&ensp;&ensp;&ensp;&ensp; Active Issue: [Issue #4429](https://github.com/dotnet/corefx/issues/4429)  
>&ensp;&ensp;&ensp;&ensp; This is the same core issue as listed for the Transports_WebSockets and TransferMode_Streamed features.  

> **On Platform:** *OS X*  
>&ensp;&ensp;&ensp;&ensp; Active Issue: [Issue #954](https://github.com/dotnet/wcf/issues/954)  
>&ensp;&ensp;&ensp;&ensp; This is most likely a machine setup issue.

#### Transports_WebSockets  
> **On Platform:** *UWP/Windows/Ubuntu*  
>&ensp;&ensp;&ensp;&ensp; Active Issue: [Issue #4429](https://github.com/dotnet/corefx/issues/4429)  
>&ensp;&ensp;&ensp;&ensp; This is the same core issue as listed for the Transports_Https and TransferMode_Streamed features.  

#### TransferMode_Streamed  
> **On Platform:** *UWP/Windows/Ubuntu*  
>&ensp;&ensp;&ensp;&ensp; Active Issue: [Issue #4429](https://github.com/dotnet/corefx/issues/4429)  
>&ensp;&ensp;&ensp;&ensp; This is the same core issue as listed for the Transports_Https and Transports_WebSockets features.  

#### Contracts_FaultContract  
> **On Platform:** *UWP*  
>&ensp;&ensp;&ensp;&ensp; Active Issue: [Issue #769](https://github.com/dotnet/wcf/issues/769)  

Not Supported Features
------------

#### Security_Message  
&ensp;&ensp; Message Security is not supported in WCF on .NET Core.  

#### NTLM Authentication  
&ensp;&ensp; Support is built into CentOS, RedHat and Mac OS X but it hasn't been tested yet.  
&ensp;&ensp; Support is not built in to Ubuntu and Debian by default.  
&ensp;&ensp; For more details see [Issue #943](https://github.com/dotnet/wcf/issues/943)  

#### Kerberos Authentication  
&ensp;&ensp; Support is built into CentOS, RedHat and Mac OS X but only Mac OS X has been tested.  
&ensp;&ensp; Support is not built in to Ubuntu and Debian by default.  
&ensp;&ensp;&ensp;&ensp; A solution has been tested on Ubuntu that works if an SPN is specified.  
&ensp;&ensp;&ensp;&ensp; The User2User scenario (i.e., when a UPN is specified) is not yet supported.  

#### NetTcp  
&ensp;&ensp; SetLingerOption isn't working correctly on OS X. This is only used on the Close(timeout) code path so the test itself is likely passing. This api required special handling on linux as this is a windows socket concept.  
&ensp;&ensp;&ensp;&ensp; For more details see: [Issue #7403](https://github.com/dotnet/corefx/issues/7403)  

#### ETW Tracing
&ensp;&ensp; ETW Tracing has been enabled for Windows in this release, support on other platforms will be available in future releases.