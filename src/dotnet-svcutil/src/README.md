## About

The `dotnet-svcutil` tool is a .NET command-line utility for retrieving metadata from web services or WSDL files and generating WCF client proxy classes. It is compatible with .NET Core and .NET Standard projects, similar to the svcutil tool for .NET Framework.

## How to use

Use `dotnet-svcutil` to generate web service proxy code from either running services or static metadata documents.

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download) or later

### Installation

To install the tool globally, run:

> dotnet tool install -g dotnet-svcutil

### Usage

> ```
> dotnet-svcutil <metadataDocumentPath> | <url> | <epr> [options*]
> ```

|Argument|Description| 
|:----|:----------|
|\<metadata document path\>|The path to a metadata document (wsdl or xsd). Standard command-line wildcards can be used in the file path.|
|\<url\>|The URL to a service endpoint that provides metadata or to a metadata document hosted online.|
|\<epr\>|The path to an XML file that contains a WS-Addressing EndpointReference for a service  endpoint that supports WS-Metadata Exchange.|

To view all available options, run:

> dotnet-svcutil --help

## Examples

- Generate client code from a running service or online metadata:

    > dotnet-svcutil http://example.com/service.svc?wsdl

- Generate client code from a running service using settings that emulate the default "WCF Connected Service" behavior in Visual Studio: 

    > dotnet-svcutil http://example.com/service.svc?wsdl -edb -n "*,MyApplication.ServiceReference1" -ct MyLibrary.MyCollection`1 -r "{{MyLibrary,1.0.0}}"

- Update an existing web service reference named `ServiceReference`. Execute this command in the directory containing the project file:

    > dotnet-svcutil -u ServiceReference

## Uninstallation

To uninstall the tool, run:

> dotnet tool uninstall -g dotnet-svcutil

## Additional documentation

For more details, visit the [WCF dotnet-svcutil tool guide](https://learn.microsoft.com/en-us/dotnet/core/additional-tools/dotnet-svcutil-guide?tabs=dotnetsvcutil2x).

## Feedback

For issues or feedback, please [file an issue on GuitHub](https://github.com/dotnet/wcf).
