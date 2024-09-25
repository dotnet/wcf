## About

The `dotnet-svcutil` tool is a .NET command-line utility for retrieving metadata from web services or WSDL files and generating WCF client proxy classes. It is compatible with .NET Core and .NET Standard projects, similar to the svcutil tool for .NET Framework.

## How to use

### Installing dotnet-svcutil:

To install the dotnet-svcutil tool globally, run:

`dotnet tool install --global dotnet-svcutil`

For more details, visit the NuGet package page: [dotnet-svcutil on NuGet](https://www.nuget.org/packages/dotnet-svcutil).

### Using dotnet-svcutil:

To use the dotnet-svcutil tool, execute the following command:

`dotnet-svcutil <metadataDocumentPath> | <url> | <epr> [options*]`


For additional usage instructions, refer to the [dotnet-svcutil documentation on GitHub](https://github.com/dotnet/wcf/tree/main/src/dotnet-svcutil#using-dotnet-svcutil).


## Key Features

Dotnet-svcutil can generate code for service contracts, clients and data types from metadata documents. These metadata documents can be on a durable storage, or be retrieved online. Online retrieval follows the WS-Metadata Exchange protocol.

You can use dotnet-svcutil tool to generate service and data contracts based on a predefined WSDL document. Use the `--serviceContract` switch and specify a URL or file location where the WSDL document can be downloaded or found. This generates the service and data contracts defined in the WSDL document that can then be used to implement a complaint service.

## Additional documentation

For more details about the tool usage, visit the [WCF dotnet-svcutil tool guide](https://learn.microsoft.com/en-us/dotnet/core/additional-tools/dotnet-svcutil-guide?tabs=dotnetsvcutil2x).

## Feedback & Contributing

To explore the project or contribute, visit our [GitHub repository](https://github.com/dotnet/wcf/tree/main/src/dotnet-svcutil).
For reporting issues or providing feedback, please [open an issue on GitHub](https://github.com/dotnet/wcf).
