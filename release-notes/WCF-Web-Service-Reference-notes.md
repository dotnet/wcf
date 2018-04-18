# WCF Web Service Reference Provider - Release Notes

WCF Web Service Reference connected services provider is now part of Visual Studio 2017. Instructions can be found in the [usage guide](https://go.microsoft.com/fwlink/?linkid=858814) document. Please [let us know](https://github.com/dotnet/wcf/issues/new) if you run into any issues or have any feedback.

## Release History

### 4/18/2018 - Visual Studio 2017 v15.7 Preview 4.0 [27617.01.d15.7]
* Bug fixes ([#2635](https://github.com/dotnet/wcf/issues/2635))

### 4/9/2018 - Visual Studio 2017 v15.7 Preview 3.0 [27603.0.d15.7]
* Bug fixes ([#2719](https://github.com/dotnet/wcf/issues/2719))

### 3/21/2018 - Visual Studio 2017 v15.7 Preview 2.0 [27520.0.d15.7]
* Improved support for future framework versions and bug fixes ([#2510](https://github.com/dotnet/wcf/issues/2510), [#2569](https://github.com/dotnet/wcf/issues/2569))

### 3/13/2018 - Visual Studio 2017 v15.7 Preview 1.0 [27512.0.d15.7]
* UI improvements and bug fixes ([#2104](https://github.com/dotnet/wcf/issues/2104))

### 2/7/2018 - Visual Studio 2017 v15.6 Preview 4.0 [27406.0.d15.6]
* Improved metadata processing. 
* Bug fixes ([#2269](https://github.com/dotnet/wcf/issues/2269), [#2465](https://github.com/dotnet/wcf/issues/2465))

### 1/25/2018 - Visual Studio 2017 v15.6 Preview 3.0 [27323.2.d15.6]
* Building the user’s project is no longer required as part of the client code generation process ([#2384](https://github.com/dotnet/wcf/issues/2384)).
* Bug fixes ([#2461](https://github.com/dotnet/wcf/issues/2461), [#2197](https://github.com/dotnet/wcf/issues/2197), [#2410](https://github.com/dotnet/wcf/issues/2410))


### 12/7/2017 - Visual Studio 2017 v15.6 Preview 1.0 [27205.0.d15.6]

* Added support of updating an existing service reference. This simplifies the process for regenerating the WCF client proxy code for an updated web service (read [preview 1 blog post](https://blogs.msdn.microsoft.com/visualstudio/2017/12/07/visual-studio-2017-version-15-6-preview/) for more details).
* Added support for generating code even if the provided service metadata does not contain any endpoints ([#2238](https://github.com/dotnet/wcf/issues/2238)).
* UI improvements and bug fixes ([#2401](https://github.com/dotnet/wcf/issues/2401))

### 12/4/2017 - Visual Studio 2017 v15.5

* All features and bug fixes included in v15.5 preview releases are now in the v15.5 final release.
* Preview releases of this tool (if installed) can be uninstalled through *Tools -> Extensions and Updates* menu in VS ([#2429](https://github.com/dotnet/wcf/issues/2429)) 

### 10/23/2017 - Visual Studio 2017 v15.5 Preview 2.0 [27019.01.d15rel]

* User interface improvements, for example, added UI to display operations of selected service to help selecting the right service when multiple similar services are available.
* Fixed an issue with error message "It was not possible to find any compatible framework version..." which can happen when .NET Core 1.0 SDK is not installed ([#2340](https://github.com/dotnet/wcf/issues/2340)).
* Fixed an issue with error message “the service at following uri does not have a valid endpoint” which can happen when a wsdl file imports other wsdl/schema documents ([#2254](https://github.com/dotnet/wcf/issues/2254)).
* Other bug fixes.

### 10/11/2017 - Visual Studio 2017 v15.5 Preview 1.0 [27009.1.d15rel]

* WCF Web Service Reference connected services provider is included in VS 2017 starting with this release.
* This tool is now available for all the languages that VS 2017 supports.
* Added support for .NET Core projects that target latest .NET Framework and future releases with TargetFramework moniker (TFM) like net47, net471 etc.
* Bug fixes ([#2172](https://github.com/dotnet/wcf/issues/2172), [#2234](https://github.com/dotnet/wcf/issues/2234), and more)

### Previous releases

* This tool was previously available in preview mode from the [Visual Studio Market Place](https://marketplace.visualstudio.com/items?itemName=WCFCORETEAM.VisualStudioWCFConnectedService).
