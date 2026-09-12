## About

System.ServiceModel.Web provides WCF client types for REST-style HTTP services, including `WebHttpBinding`, `WebChannelFactory<TChannel>`, URI templates, and XML, JSON, and raw message encoding.

### Installing the package

Install System.ServiceModel.Web from [NuGet.org](https://www.nuget.org/packages/System.ServiceModel.Web):

`dotnet add package System.ServiceModel.Web`

## Compatibility notes

The client APIs preserve .NET Framework behavior. In particular, `/` in a `UriTemplate` path-variable value creates additional path segments, matching uses the base URI as a case-insensitive prefix, compound-segment matching does not backtrack and compares literals case-sensitively, `UriTemplateTable` query-variable disambiguation uses case-sensitive key lookup, and `QueryStringConverter` reports all enums as convertible even though only `int`-backed enums deserialize as the enum type.

## Feedback & Contributing

To explore the project or contribute, visit our [GitHub repository](https://github.com/dotnet/wcf/).
For reporting issues or providing feedback, please [open an issue on GitHub](https://github.com/dotnet/wcf).
