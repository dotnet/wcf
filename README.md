# WCF -- Windows Communication Foundation Client Libraries

[![Build Status](https://dev.azure.com/dnceng/internal/_apis/build/status/dotnet/wcf/dotnet.wcf?branchName=master)](https://dev.azure.com/dnceng/internal/_build/latest?definitionId=318&branchName=master)

This repository contains the [.NET Core](http://github.com/dotnet/core) version of the [Windows Communication Foundation](https://msdn.microsoft.com/en-us/library/dd456779.aspx) client libraries.

It's a subset of the .NET Framework version of Windows Communication Foundation and currently supports the same API surface available for Windows 8.1 Store apps. It is used to build .NET Core apps, including [Windows UWP](https://msdn.microsoft.com/en-us/library/windows/apps/dn609832.aspx) and [ASP.NET Core](https://docs.microsoft.com/aspnet/core). These client libraries are suitable for mobile devices or on mid-tier servers to communicate with existing WCF services.

By targeting .NET Core, WCF now has the opportunity for much wider reach across PCs, laptops, mobile devices, Xbox, HoloLens etc. It can also be ported to other operating systems since it runs on .NET Core, which is adding support for Linux and OS X.

We have deliberately opened the doors to the code early in the process so that you can be part of the effort by contributing to the project or providing feedback. Several features are still a work in progress, and we expect to enable them as soon as possible. Click on [Issues](https://github.com/dotnet/wcf/issues) to follow how we are prioritizing the work. Our goal is to achieve feature parity with the corresponding Windows Store libraries and then improve based on your feedback.

## How to Engage, Contribute and Provide Feedback

Some of the best ways to contribute are to try things out, file bugs, and join in design conversations. 

Want to get more familiar with what's going on in the code?
* [Pull requests](https://github.com/dotnet/wcf/pulls): [Open](https://github.com/dotnet/wcf/pulls?q=is%3Aopen+is%3Apr)/[Closed](https://github.com/dotnet/wcf/pulls?q=is%3Apr+is%3Aclosed)
* [![Backlog](https://cloud.githubusercontent.com/assets/1302850/6260412/38987b1e-b793-11e4-9ade-d3fef4c6bf48.png)](https://github.com/dotnet/wcf/issues?q=is%3Aopen+is%3Aissue+label%3A%220+-+Backlog%22), [![Up Next](https://cloud.githubusercontent.com/assets/1302850/6260418/4c2c7a54-b793-11e4-8ce1-a27ff5378d08.png)](https://github.com/dotnet/wcf/issues?q=is%3Aopen+is%3Aissue+label%3A%221+-+Up+Next%22) and [![In Progress](https://cloud.githubusercontent.com/assets/1302850/6260414/41b0fc30-b793-11e4-9d50-d09563cd138a.png)](https://github.com/dotnet/wcf/issues?q=is%3Aopen+is%3Aissue+label%3A%222+-+In+Progress%22) changes

Looking for something to work on? The list of [up-for-grabs issues](https://github.com/dotnet/wcf/labels/up-for-grabs) is a great place to start. See some of our guides for more details:

* [Contributing Guide](Documentation/contributing.md)
* [Developer Guide](Documentation/developer-guide.md)
* [Issue Guide](Documentation/issue-guide.md)

You are also encouraged to start a discussion by filing a [New Issue](https://github.com/dotnet/wcf/issues/new).

You can discuss .NET OSS more generally in the [.NET Foundation forums].

Want to chat with other members of the WCF community?

[![Join the chat at https://gitter.im/dotnet/wcf](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/dotnet/wcf?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

This project has adopted the code of conduct defined by the [Contributor Covenant](http://contributor-covenant.org/)
to clarify expected behavior in our community.
For more information see the [.NET Foundation Code of Conduct](http://www.dotnetfoundation.org/code-of-conduct).

[.NET Foundation forums]: http://forums.dotnetfoundation.org/

### Reporting security issues and security bugs

Security issues and bugs should be reported privately, via email, to the
Microsoft Security Response Center (MSRC) <secure@microsoft.com>. You should
receive a response within 24 hours. If for some reason you do not, please follow
up via email to ensure we received your original message. Further information,
including the MSRC PGP key, can be found in the
[Security TechCenter](https://technet.microsoft.com/en-us/security/ff852094.aspx).

## WCF Library Components

This repo contains the following library components.

|Component|Description|
|:--------|:----------|
|**System.ServiceModel.Primitives**<br>[![NuGet Package](https://img.shields.io/nuget/v/System.ServiceModel.Primitives.svg)](https://www.nuget.org/packages/System.ServiceModel.Primitives)<br>[![NuGet Package](https://img.shields.io/nuget/vpre/System.ServiceModel.Primitives.svg)](https://www.nuget.org/packages/System.ServiceModel.Primitives)|Provides the common types used by all of the WCF libraries.| 
|**System.ServiceModel.Http**<br>[![NuGet Package](https://img.shields.io/nuget/v/System.ServiceModel.Http.svg)](https://www.nuget.org/packages/System.ServiceModel.Http)<br>[![NuGet Package](https://img.shields.io/nuget/vpre/System.ServiceModel.Http.svg)](https://www.nuget.org/packages/System.ServiceModel.Http)|Provides the types that permit SOAP messages to be exchanged using Http (example: BasicHttpBinding).|
|**System.ServiceModel.NetTcp**<br>[![NuGet Package](https://img.shields.io/nuget/v/System.ServiceModel.NetTcp.svg)](https://www.nuget.org/packages/System.ServiceModel.NetTcp)<br>[![NuGet Package](https://img.shields.io/nuget/vpre/System.ServiceModel.NetTcp.svg)](https://www.nuget.org/packages/System.ServiceModel.NetTcp)|Provides the types that permit SOAP messages to be exchanged using TCP (example: NetTcpBinding).|
|**System.ServiceModel.Duplex**<br>[![NuGet Package](https://img.shields.io/nuget/v/System.ServiceModel.Duplex.svg)](https://www.nuget.org/packages/System.ServiceModel.Duplex)<br>[![NuGet Package](https://img.shields.io/nuget/vpre/System.ServiceModel.Duplex.svg)](https://www.nuget.org/packages/System.ServiceModel.Duplex)|Provides the types that permit 2-way ("duplex") exchanges of messages.|
|**System.ServiceModel.Security**<br>[![NuGet Package](https://img.shields.io/nuget/v/System.ServiceModel.Security.svg)](https://www.nuget.org/packages/System.ServiceModel.Security)<br>[![NuGet Package](https://img.shields.io/nuget/vpre/System.ServiceModel.Security.svg)](https://www.nuget.org/packages/System.ServiceModel.Security)|Provides the types that support additional security features.|

## License

This project is licensed under the [MIT license](LICENSE).

## .NET Foundation

WCF is a [.NET Foundation](http://www.dotnetfoundation.org/projects) project.

## Related Projects
There are many .NET related projects on GitHub.

- The
[.NET home repo](https://github.com/Microsoft/dotnet) links to 100s of .NET projects, from Microsoft and the community.
- The [.NET Core repo](https://github.com/dotnet/core) links to .NET Core related projects from Microsoft.
- The [ASP.NET home repo](https://github.com/aspnet/home) is the best place to start learning about ASP.NET Core.
- [https://dotnetfoundation.org/projects](https://dotnetfoundation.org/projects) is a good place to discover .NET Foundation projects.

## The Windows Communication Foundation Story

[Windows Communication Foundation](https://msdn.microsoft.com/en-us/library/dd456779.aspx) has been part of the full .NET Framework for a number of years. Microsoft continues to develop this version and has made the source code available via [Reference Source](https://github.com/microsoft/referencesource).

With the introduction of Windows 8, Microsoft made some of the client-oriented functionality available in the [Windows Store](https://msdn.microsoft.com/en-us/library/hh556233(v=vs.110).aspx). This allowed mobile devices to interact with WCF services that were built with the .NET framework version.

This new WCF project differs from these other products in 2 ways. First, it is built on .NET Core so that it can be used on a wider range of devices and operating system. Second, it is open-source and managed by the [.NET Foundation](http://www.dotnetfoundation.org/projects). Microsoft builds and publishes NuGet packages from the code in this repository. UWP and ASP.NET 5 applications use those packages.

WCF *service* applications should still be created with the full .NET Framework version.
