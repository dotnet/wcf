# dotnet-svcutil - Release Notes

Getting Started instructions can be found in the [usage guide](https://go.microsoft.com/fwlink/?linkid=874971) document. Please [let us know](https://github.com/dotnet/wcf/issues/new) if you run into any issues or have any feedback.

### Information
* [NuGet Package](https://nuget.org/packages/dotnet-svcutil)

## Version History

### > 2.0.1

* Please [review the release page](https://github.com/dotnet/wcf/releases) for changes.  
* e.g. [dotnet-svcutil 2.1.0 Release](https://github.com/dotnet/wcf/releases/tag/v2.1.0-dotnet-svcutil)

### 2.0.1 (7/29/2019)
* Added --sync option to generate synchronous operations in addition to async operations ([#654](https://github.com/dotnet/wcf/issues/654)).
* Bug fixes ([#3494](https://github.com/dotnet/wcf/issues/3494), [#3542](https://github.com/dotnet/wcf/issues/3542), [#3681](https://github.com/dotnet/wcf/issues/3681), [#3226](https://github.com/dotnet/wcf/issues/3226), [#2332](https://github.com/dotnet/wcf/issues/2332), [#3682](https://github.com/dotnet/wcf/issues/3682)).

### 2.0.0 (2/18/2019)
* Changed from a per-project CLI tool to a global CLI tool. ([#3071](https://github.com/dotnet/wcf/issues/3071)). Instead of adding a DotNetCliToolReference to each project in order to use the tool, you can now install it once globally with `dotnet tool install --global dotnet-svcutil`. This requires the .NET Core 2.1 SDK or later.
* Added new options to support the tool being run from a outside a project context. These are normally inferred based on the project the tool is run on. Since it can now be run with no project context these options allow you to control these settings manually. New options include: --outputDir, --outputFile, --projectFile,  --runtimeIdentifier, and --targetFramework. See `dotnet-svcutil --help` for usage.
* Added automatic reference reuse. Similar to the "Reuse types in all referenced assemblies" option in the WCF Web Service Reference Provider, dotnet-svcutil will by default try to reuse all types in the project's references. This can be disabled with the --noTypeReuse option, or by passing the specific references to reuse with the --reference option.
* Added an update option (--update) which allows updating a service reference. This is only supported if the service reference was added using dotnet-svcutil 2.0.0 or later.
* Bug fixes ([#3253](https://github.com/dotnet/wcf/issues/3253))

### 1.0.4 (10/30/2018)
* Added anonymous telemetry information collection (set environment variable DOTNET_SVCUTIL_TELEMETRY_OPTOUT=1 to opt out).
* Bug fixes ([#3149](https://github.com/dotnet/wcf/issues/3149), [#2715](https://github.com/dotnet/wcf/issues/2715))

### 1.0.3 (08/29/2018)
* Bug fixes ([#3081](https://github.com/dotnet/wcf/issues/3081), [#3103](https://github.com/dotnet/wcf/issues/3103))

### 1.0.2 (08/06/2018)
* Added debug verbosity option (--verbosity debug).
* Bug fixes ([#2848](https://github.com/dotnet/wcf/issues/2848), [#2591](https://github.com/dotnet/wcf/issues/2591), [#2765](https://github.com/dotnet/wcf/issues/2765))

### 1.0.1 (07/02/2018)
* Added support for loading web service metadata from multiple WSDL/XML Schema files ([#2920](https://github.com/dotnet/wcf/issues/2920), [#2958](https://github.com/dotnet/wcf/issues/2958))

### 1.0.0 (06/04/2018)
* First stable release

### 1.0.0-preview-20522-1161 (05/23/2018)
* Initial release
