## About

`System.ServiceModel.Msmq` provides the types that permit SOAP messages to be exchanged using MSMQ (Microsoft Message Queuing) as the transport. The package implements the client-side surface of the .NET Framework `NetMsmqBinding` and `MsmqIntegrationBinding`.

This package is **Windows-only**. The Windows MSMQ feature (`MSMQ-Server` or the client subset) must be installed on the machine running the client.

## Runtime dependency

The package is self-contained on the managed side: it depends only on `System.ServiceModel.Primitives`. The send path P/Invokes directly into `mqrt.dll` from the Windows MSMQ feature — no extra NuGet packages required.

## Installing

```
dotnet add package System.ServiceModel.Msmq
```

## Usage

Send a SOAP message over `NetMsmqBinding` to a local private queue:

```csharp
using System.ServiceModel;
using System.ServiceModel.Channels;

var binding = new NetMsmqBinding(NetMsmqSecurityMode.None)
{
    Durable = true,
    ExactlyOnce = false,
};

IChannelFactory<IOutputChannel> factory = binding.BuildChannelFactory<IOutputChannel>();
factory.Open();

IOutputChannel channel = factory.CreateChannel(
    new EndpointAddress("net.msmq://localhost/private/orders"));
channel.Open();

Message message = Message.CreateMessage(
    MessageVersion.Soap12WSAddressing10,
    "urn:contoso/orders/submit",
    new { OrderId = 42, Sku = "ABC" });
channel.Send(message);

channel.Close();
factory.Close();
```

Integrate with a legacy MSMQ application via `MsmqIntegrationBinding` and `MsmqMessage<T>`:

```csharp
using System.ServiceModel.MsmqIntegration;

var msg = new MsmqMessage<MyPayload>(myPayload)
{
    Label = "order-42",
    Priority = MessagePriority.High,
    CorrelationId = "11111111-2222-3333-4444-555555555555\\1",
};
// channel.Send(...) carrying msg as the WCF message body.
```

## Differences from the .NET Framework

This client-only port deliberately omits server-side concerns. See the [`dotnet/wcf` README](https://github.com/dotnet/wcf/) for the broader WCF client roadmap. Highlights:

- Server hosting (channel listeners, poison-message handlers, queue receive loops) is not in scope — use [CoreWCF](https://github.com/CoreWCF/CoreWCF) for service hosting.
- `System.Configuration`-based XML binding configuration is not supported; configure bindings in code.
- `IOutputSessionChannel` is supported but emits one MSMQ message per `Send` rather than the .NET Framework "session gram" framing — wire-level interop with netfx `SessionMode.Required` services is tracked as a follow-up.

## Feedback & Contributing

To explore the project or contribute, visit our [GitHub repository](https://github.com/dotnet/wcf/).
For reporting issues or providing feedback, please [open an issue on GitHub](https://github.com/dotnet/wcf).
