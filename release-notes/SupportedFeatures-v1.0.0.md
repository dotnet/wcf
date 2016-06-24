WCF Features in .Net Core 1.0.0
======================================================

Supported features
------------
:white_check_mark: -- Works with no known major issues  
:warning: --  Partially works with known issues or only partially tested  
&ensp;&ensp;&ensp;&ensp;&ensp;&ensp;&ensp;**Tip:** Click on symbol for more details.  
:no_entry_sign: -- Not Supported  
&ensp;&ensp;&ensp;&ensp;&ensp;&ensp;&ensp;**Tip:** Click on symbol for more details.  
:grey_question: -- Not yet tested

| Category     |  Feature              |   [UWP](#platforms)                       |  [Windows](#platforms)       |   [Linux OSs](#platforms)        |   [Mac OS X](#platforms)  |
| :-------     | :--------             | :------------:                      | :-------:                          |  :------:                                  |   :-----:                           |
|Bindings      |BasicHttpBinding       |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                          | :white_check_mark:                  |
|              |CustomBinding          |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                          | :white_check_mark:                  |
|              |NetHttpBinding         |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                          | :white_check_mark:                  |
|              |NetTcpBinding          |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                          | :white_check_mark:    |
|Transports    |Http                   |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |Https                  |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:           |
|              |Tcp                    |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:          |
|              |WebSockets             |[:warning:](#known_issues)           |:white_check_mark:                  |[:no_entry_sign:](#known_issues)           | [:no_entry_sign:](#known_issues)    |
|Channel types |Request/Reply          |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |Duplex                 |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|Encodings     |Text                   |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |Binary                 |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|TransferMode  |Buffered (sync/async)  |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |Streamed (sync/async)  |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:           |
|MessageVersion|SOAP 1.1 UTF8          |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |SOAP 1.2 UTF8          |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|Contracts     |DataContract           |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |XmlSerializer          |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |MessageFormat          |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |FaultContract          |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|Security      |None                   |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |Transport              |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |Message                |[:no_entry_sign:](#known_issues)     |[:no_entry_sign:](#known_issues)    |[:no_entry_sign:](#known_issues)           | [:no_entry_sign:](#known_issues)    |
|Client Authentication|Basic           |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |Digest                 |:white_check_mark:                   |:white_check_mark:                  |[:no_entry_sign:](#known_issues)           | [:no_entry_sign:](#known_issues)    |
|              |NTLM                   |:white_check_mark:                   |:white_check_mark:                  |[:no_entry_sign:](#known_issues)           | [:no_entry_sign:](#known_issues)    |
|              |Kerberos/TCP           |[:no_entry_sign:](#known_issues)     |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |Kerberos/HTTP          |:white_check_mark:                   |:white_check_mark:                  |[:no_entry_sign:](#known_issues)           | [:no_entry_sign:](#known_issues)    |
|              |Certificate/TCP       |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |Certificate/HTTP      |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :grey_question:                     |
|Service Authentication|Certificate    |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |SPNIdentity/HTTP       |:white_check_mark:                   |:white_check_mark:                  |[:warning:](#known_issues)                 | [:warning:](#known_issues)          |
|              |SPNIdentity/TCP        |[:warning:](#known_issues)           |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |UPNIdentity    |[:warning:](#known_issues)          |[:warning:](#known_issues)          |[:warning:](#known_issues)                  | [:warning:](#known_issues)          |
|Client types  |ChannelFactory\<T\>    |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |ChannelBase\<T\>       |:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|Extensibility |IClientMessageInspector|:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|              |IClientMessageFormatter|:white_check_mark:                   |:white_check_mark:                  |:white_check_mark:                         | :white_check_mark:                  |
|Management    |ETW Tracing            |[:no_entry_sign:](#known_issues)      |:white_check_mark:                  |[:no_entry_sign:](#known_issues)          |[:no_entry_sign:](#known_issues)     |          


#### Platforms
&ensp;&ensp;For more details about what UWP is please see this [dotnet Glossary doc](https://github.com/dotnet/corefx/blob/master/Documentation/project-docs/glossary.md).

&ensp;&ensp;Linux OSs includes centos, debian, fedora, linux, opensuse, rhel and ubuntu.

&ensp;&ensp;For supported versions of each platform and other details please see the [.NET Core 1.0.0 Release Notes](https://github.com/dotnet/core/blob/master/release-notes/1.0/Release-Notes-1.0.0.md).