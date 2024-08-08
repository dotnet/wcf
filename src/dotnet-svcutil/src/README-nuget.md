## About

The `dotnet-svcutil` tool is a .NET command-line utility for retrieving metadata from web services or WSDL files and generating WCF client proxy classes. It is compatible with .NET Core and .NET Standard projects, similar to the svcutil tool for .NET Framework.

## How to use

Install and run the tool:
> ```
> dotnet tool install --global dotnet-svcutil
> dotnet-svcutil <metadataDocumentPath> | <url> | <epr> [options*]
> ```

To view all available options, run:

> dotnet-svcutil --help

## Key Features

Dotnet-svcutil can generate code for service contracts, clients and data types from metadata documents. These metadata documents can be on a durable storage, or be retrieved online. Online retrieval follows the WS-Metadata Exchange protocol.

You can use dotnet-svcutil tool to generate service and data contracts based on a predefined WSDL document. Use the `--serviceContract` switch and specify a URL or file location where the WSDL document can be downloaded or found. This generates the service and data contracts defined in the WSDL document that can then be used to implement a complaint service.

## Additional documentation

For more details about the tool usage, visit the [WCF dotnet-svcutil tool guide](https://learn.microsoft.com/en-us/dotnet/core/additional-tools/dotnet-svcutil-guide?tabs=dotnetsvcutil2x).

## Feedback & Contributing

To explore the project or contribute, visit our [GitHub repository](https://github.com/dotnet/wcf/tree/main/src/dotnet-svcutil).
For reporting issues or providing feedback, please [open an issue on GitHub](https://github.com/dotnet/wcf).
