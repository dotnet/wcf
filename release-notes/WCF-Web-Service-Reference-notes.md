# WCF Web Service Reference Provider - Release Notes

WCF Web Service Reference connected services provider is now part of Visual Studio 2017. Instructions can be found in the [usage guide](/Documentation/WCF-Web-Service-Reference-guide.md) document. Please [let us know](https://github.com/dotnet/wcf/issues/new) if you run into any issues or have any feedback.

## Release History

### 10/23/2017 - Visual Studio 2017 v15.5 Preview 2.0 [27019.01.d15rel]

* User interface improvements, for example, added UI to display service operations of selected service to help selecting the right service when multiple similar services are available.
* Fixed an issue with error message "It was not possible to find any compatible framework version..." which can happen when .NET Core 1.0 SDK is not installed ([#2340](https://github.com/dotnet/wcf/issues/2340)).
* Other bug fixes.

### 10/11/2017 - Visual Studio 2017 v15.5 Preview 1.0 [27009.1.d15rel]

* WCF Web Service Reference connected services provider is included in VS 2017 starting with this release.
* This tool is now available for all the languages that VS 2017 supports.
* Added support for .NET Core projects that target latest .NET Framework and future releases with TargetFramework moniker (TFM) like net47, net471 etc.
* Bug fixes ([#2172](https://github.com/dotnet/wcf/issues/2172), [#2234](https://github.com/dotnet/wcf/issues/2234), and more)

### Previous releases

* This tool was previously available in preview mode from the [Visual Studio Market Place](https://marketplace.visualstudio.com/items?itemName=WCFCORETEAM.VisualStudioWCFConnectedService).
