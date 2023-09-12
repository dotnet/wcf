## About

Provides the types that permit SOAP messages to be exchanged using TCP (example: NetTcpBinding).

## Key Features

* A secure, reliable binding suitable for cross-machine communication
* Specifies the types of transport-level and message-level security used by an endpoint configured with a NetTcpBinding.
* Provides properties that control authentication parameters and protection level for the TCP transport.

## How to Use

```csharp
using System;
using System.ServiceModel;
using System.ServiceModel.NetTcp;

NetTcpBinding binding = new NetTcpBinding();
binding.Security.Mode = SecurityMode.Transport;
binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
```

## Main Types

* `System.ServiceModel.NetTcpBinding`
* `System.ServiceModel.NetTcpSecurity`
* `System.ServiceModel.TcpTransportSecurity`

## Addtional Documentation

* [WCF Repo documentation](https://github.com/dotnet/wcf/tree/main#readme)
* [NetTcpBinding API documentation](https://learn.microsoft.com/en-us/dotnet/api/system.servicemodel.nettcpbinding)
* [NetTcpSecurity API documentation](https://learn.microsoft.com/en-us/dotnet/api/system.servicemodel.nettcpsecurity)
* [TcpTransportSecurity API documentation](https://learn.microsoft.com/en-us/dotnet/api/system.servicemodel.tcptransportsecurity)

## Related Packages

* [System.ServiceModel.Primitives](https://www.nuget.org/packages/System.ServiceModel.Primitives)

## Feedback & Contributing

System.ServiceModel.NetTcp is released as open source under the [MIT license](https://licenses.nuget.org/MIT). Bug reports and contributions are welcome at [the GitHub repository](https://github.com/dotnet/wcf).