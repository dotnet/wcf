# WCF

|   |Windows|
|:-:|:-:|
|**Debug**|[![Build status](http://dotnet-ci.cloudapp.net/job/dotnet_wcf_windows_debug/badge/icon)](http://dotnet-ci.cloudapp.net/job/dotnet_wcf_windows_debug/)|
|**Release**|[![Build status](http://dotnet-ci.cloudapp.net/job/dotnet_wcf_windows_release/badge/icon)](http://dotnet-ci.cloudapp.net/job/dotnet_wcf_windows_release/)|
|**Coverage Report**||[![Coverage Status](https://coveralls.io/repos/dotnet/wcf/badge.svg?branch=master)](http://dotnet-ci.cloudapp.net/job/dotnet_wcf_coverage_windows/lastStableBuild/Code_Coverage_Report/)||



This repository contains the source code for the version of the [Windows Communication Foundation](https://msdn.microsoft.com/en-us/library/dd456779.aspx) that targets the [.NET Core](http://github.com/dotnet/core) platform, specifically the [.NET Core Runtime (CoreCLR)](https://github.com/dotnet/coreclr) and [.NET Core Framework (CoreFx)](http://github.com/dotnet/corefx).

It's a subset of the full Windows Communication Foundation available on the desktop and provides support for the library profiles already available for building WCF apps for the Windows Store. These profiles are primarily client-based, making them suited for mobile devices or on mid-tier servers to communicate with existing WCF services.

By targeting .NET Core, WCF now has the opportunity for much wider reach across PCs, laptops, mobile devices, Xbox, HoloLens etc.  And because .NET Core is designed to be cross-platform, it offers the potential for this version of WCF to ultimately run on OS X or Linux operating systems.

We have deliberately opened the doors to the code early in the process so that you can be part of the effort by contributing to the project or providing feedback. Several features are still a work in progress, and we expect to enable them as soon as possible.  Click on [Issues](https://github.com/dotnet/wcf/issues) to follow how we are prioritizing the work. Our goal is to achieve feature parity with the corresponding Windows Store libraries and then improve based on your feedback.

## How to Engage, Contribute and Provide Feedback

Some of the best ways to contribute are to try things out, file bugs, and join in design conversations. 

Want to get more familiar with what's going on in the code?
* [Pull requests](https://github.com/dotnet/wcf/pulls): [Open](https://github.com/dotnet/wcf/pulls?q=is%3Aopen+is%3Apr)/[Closed](https://github.com/dotnet/wcf/pulls?q=is%3Apr+is%3Aclosed)
* [![Backlog](https://cloud.githubusercontent.com/assets/1302850/6260412/38987b1e-b793-11e4-9ade-d3fef4c6bf48.png)](https://github.com/dotnet/wcf/issues?q=is%3Aopen+is%3Aissue+label%3A%220+-+Backlog%22), [![Up Next](https://cloud.githubusercontent.com/assets/1302850/6260418/4c2c7a54-b793-11e4-8ce1-a27ff5378d08.png)](https://github.com/dotnet/wcf/issues?q=is%3Aopen+is%3Aissue+label%3A%221+-+Up+Next%22) and [![In Progress](https://cloud.githubusercontent.com/assets/1302850/6260414/41b0fc30-b793-11e4-9d50-d09563cd138a.png)](https://github.com/dotnet/wcf/issues?q=is%3Aopen+is%3Aissue+label%3A%222+-+In+Progress%22) changes

Looking for something to work on? The list of [up-for-grabs issues](https://github.com/dotnet/wcf/labels/up%20for%20grabs) is a great place to start. See some of our guides for more details:

* [Contributing Guide](https://github.com/dotnet/wcf/wiki/Contributing)
* [Developer Guide](https://github.com/dotnet/wcf/wiki/Developer-Guide)
* [Issue Guide](https://github.com/dotnet/wcf/wiki/Issue-Guide)

You are also encouraged to start a discussion by filing a [New Issue](https://github.com/dotnet/wcf/issues/new).

You can discuss .NET OSS more generally in the [.NET Foundation forums].

Want to chat with other members of the WCF community?

[![Join the chat at https://gitter.im/dotnet/wcf](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/dotnet/wcf?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)


[.NET Foundation forums]: http://forums.dotnetfoundation.org/

## WCF Library Components

This repo contains the following library components.

* **System.ServiceModel.Primitives**.  Provides the common types used by all of the WCF libraries.

* **System.ServiceModel.Http**. Provides the types that permit SOAP messages to be exchanged using Http (example: BasicHttpBinding).

* **System.ServiceModel.NetTcp**. Provides the types that permit SOAP messages to be exchanged using TCP (example: NetTcpBinding).
 
* **System.ServiceModel.Duplex**. Provides the types that permit 2-way ("duplex") exchanges of messages.  This library is currently under construction and not yet fully functional.

* **System.ServiceModel.Security**. Provides the types that support additional security features.  This library is currently under construction and not yet fully functional.

## License

This WCF repo is licensed under the [MIT license](LICENSE).

## .NET Foundation

WCF is a [.NET Foundation](http://www.dotnetfoundation.org/projects) project.

## Related Projects
There are many .NET related projects on GitHub.

- The
[.NET home repo](https://github.com/Microsoft/dotnet) links to 100s of .NET projects, from Microsoft and the community.
- The [.NET Core repo](https://github.com/dotnet/core) links to .NET Core related projects from Microsoft.
- The [ASP.NET home repo](https://github.com/aspnet/home) is the best place to start learning about ASP.NET 5.
- [dotnet.github.io](http://dotnet.github.io) is a good place to discover .NET Foundation projects.
