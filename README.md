# Windows Communication Foundation (WCF)

|   |Windows|
|:-:|:-:|
|**Debug**|[![Build status](http://dotnet-ci.cloudapp.net/job/dotnet_wcf_windows_debug/badge/icon)](http://dotnet-ci.cloudapp.net/job/dotnet_wcf_windows_debug/)|
|**Release**|[![Build status](http://dotnet-ci.cloudapp.net/job/dotnet_wcf_windows_release/badge/icon)](http://dotnet-ci.cloudapp.net/job/dotnet_wcf_windows_release/)|
|**Coverage Report**||[![Coverage Status](https://coveralls.io/repos/dotnet/wcf/badge.svg?branch=master)](http://dotnet-ci.cloudapp.net/job/dotnet_wcf_coverage_windows/lastStableBuild/Code_Coverage_Report/)||



The wcf repo contains the library implementation for [Windows Communication Foundation](https://github.com/dotnet/wcf). It includes System.ServiceModel.Http, System.ServiceModel.NetTcp and other libraries. It builds and runs on Windows. You can ['watch'](https://github.com/dotnet/wcf/subscription) the repo to see progress as additional libraries are added.

The version of WCF in this repo runs on top of the [.NET Core](http://github.com/dotnet/core) platform, namely the [.NET Core Runtime (CoreCLR)](https://github.com/dotnet/coreclr) and [.NET Core Framework (CoreFx)](http://github.com/dotnet/corefx)

This wcf repo is a subset of the full WCF product available on the desktop, and it supports the library profiles already available for building WCF apps for the Windows Store.  Those profiles are primarily client-based, making them suited for mobile devices or on a mid-tier server to communicate with existing WCF services.

## How to Engage, Contribute and Provide Feedback

Some of the best ways to contribute are to try things out, file bugs, and join in design conversations. 

Want to get more familiar with what's going on in the code?
* [Pull requests](https://github.com/dotnet/wcf/pulls): [Open](https://github.com/dotnet/wcf/pulls?q=is%3Aopen+is%3Apr)/[Closed](https://github.com/dotnet/wcf/pulls?q=is%3Apr+is%3Aclosed)
* [![Backlog](https://cloud.githubusercontent.com/assets/1302850/6260412/38987b1e-b793-11e4-9ade-d3fef4c6bf48.png)](https://github.com/dotnet/wcf/issues?q=is%3Aopen+is%3Aissue+label%3A%220+-+Backlog%22), [![Up Next](https://cloud.githubusercontent.com/assets/1302850/6260418/4c2c7a54-b793-11e4-8ce1-a27ff5378d08.png)](https://github.com/dotnet/wcf/issues?q=is%3Aopen+is%3Aissue+label%3A%221+-+Up+Next%22) and [![In Progress](https://cloud.githubusercontent.com/assets/1302850/6260414/41b0fc30-b793-11e4-9d50-d09563cd138a.png)](https://github.com/dotnet/wcf/issues?q=is%3Aopen+is%3Aissue+label%3A%222+-+In+Progress%22) changes

Looking for something to work on? The list of [up-for-grabs issues](https://github.com/dotnet/wcf/labels/up%20for%20grabs) is a great place to start. See some of our guides for more details:

* [Contributing Guide](https://github.com/dotnet/wcf/wiki/Contributing)
* [Developer Guide](https://github.com/dotnet/wcf/wiki/Developer-Guide)
* [Issue Guide](https://github.com/dotnet/wcf/wiki/Issue-Guide)

You are also encouraged to start a discussion by filing an issue or creating a
gist.

You can discuss .NET OSS more generally in the [.NET Foundation forums].

[.NET Foundation forums]: http://forums.dotnetfoundation.org/

## WCF Library Components

The repo contains the following library components.

* **System.ServiceModel.Primitives**.  Provides the common types used by all of the WCF libraries.

* **System.ServiceModel.Http**. Provides the types that permit SOAP messages to be exchanged using Http (example: BasicHttpBinding).

* **System.ServiceModel.NetTcp**. Provides the types that permit SOAP messages to be exchanged using TCP (example: NetTcpBinding).
 
* **System.ServiceModel.Duplex**. Provides the types that permit 2-way ("duplex") exchanges of messages.  This library is currently under construction and not yet fully functional.

* **System.ServiceModel.Security**. Provides the types that support additional security features.  This library is currently under construction and not yet fully functional.

## License

Windows Communication Foundation (including the wcf repo) is licensed under the [MIT license](LICENSE).

## .NET Foundation

WCF is a [.NET Foundation](http://www.dotnetfoundation.org/projects) project.

## Related Projects
There are many .NET related projects on GitHub.

- The
[.NET home repo](https://github.com/Microsoft/dotnet) links to 100s of .NET projects, from Microsoft and the community.
- The [.NET Core repo](https://github.com/dotnet/core) links to .NET Core related projects from Microsoft.
- The [ASP.NET home repo](https://github.com/aspnet/home) is the best place to start learning about ASP.NET 5.
- [dotnet.github.io](http://dotnet.github.io) is a good place to discover .NET Foundation projects.
