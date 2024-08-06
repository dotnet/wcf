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

### Referencing and Sharing Types
| Option  |Description| 
|:--------|:----------|
--reference \<package\>|Package or project references to reuse types from. When generating clients, use this option to specify references in the user's project that might  contain types representing the metadata being imported. The reference can be specified as a nuget package name and version or the path to a .Net Core project. (Short Form: -r)
--excludeType \<type\>|A fully-qualified or assembly-qualified type name to exclude from referenced contract types. (Short Form: -et)

### Choosing a Serializer
| Option  |Description| 
|:--------|:----------|
--serializer Auto|Automatically select the serializer. This tries to use the Data Contract serializer and uses the XmlSerializer if that fails. (Short Form: -ser)
--serializer DataContractSerializer|Generate data types that use the Data Contract Serializer for serialization and deserialization
--serializer XmlSerializer|Generate data types that use the XmlSerializer for serialization and deserialization

### Choosing a Namespace for the Client
| Option  |Description| 
|:--------|:----------|
--namespace \<string,string\>|A mapping from a WSDL or XML Schema targetNamespace to a CLR namespace. Using the '*' for the targetNamespace maps all targetNamespaces without an explicit mapping to that CLR namespace. Default: derived from the target namespace of the schema document for Data Contracts. The default namespace is used for all other generated types. (Short Form: -n)

### Choosing a Data Binding
| Option  |Description| 
|:--------|:----------|
--enableDataBinding|Implement the System.ComponentModel.INotifyPropertyChanged interface on all Data Contract types to enable data binding. (Short Form: -edb)

## Additional documentation

For more details, visit the [WCF dotnet-svcutil tool guide](https://learn.microsoft.com/en-us/dotnet/core/additional-tools/dotnet-svcutil-guide?tabs=dotnetsvcutil2x).

## Feedback

For issues or feedback, please [file an issue on GuitHub](https://github.com/dotnet/wcf).
