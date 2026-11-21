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

`MsmqIntegrationBinding` carries raw MSMQ payloads rather than SOAP envelopes, so
it contributes no message encoder. The body is produced from
`MsmqIntegrationMessageProperty.Body` according to `SerializationFormat`, and a
message sent over this binding must carry an `MsmqIntegrationMessageProperty`.

## Differences from the .NET Framework

This client-only port deliberately omits server-side concerns. See the [`dotnet/wcf` README](https://github.com/dotnet/wcf/) for the broader WCF client roadmap. Highlights:

- Server hosting (channel listeners, poison-message handlers, queue receive loops) is not in scope — use [CoreWCF](https://github.com/CoreWCF/CoreWCF) for service hosting.
- `System.Configuration`-based XML binding configuration is not supported; configure bindings in code.
- `IOutputSessionChannel` is supported but emits one MSMQ message per `Send` rather than the .NET Framework "session gram" framing — wire-level interop with netfx `SessionMode.Required` services is tracked as a follow-up.

### Configurations that are rejected rather than silently ignored

Where a .NET Framework capability has not been ported, the corresponding
configuration throws instead of quietly falling back to different behaviour on
the wire:

| Configuration | Behaviour | Reason |
| --- | --- | --- |
| `NetMsmqSecurityMode.Message` / `.Both` | `NotSupportedException` from `CreateBindingElements()` | Needs the WS-Security message-protection stack, which this package does not carry. Falling back would put plaintext on the wire under a binding that claims message security. |
| `MsmqTransportBindingElement.UseActiveDirectory = true` | `NotSupportedException` when the channel factory resolves the address | Active Directory queue path lookup is not ported; the DIRECT= fallback would address a different queue than the caller asked for. |
| `MsmqMessageSerializationFormat.Binary` / `.ActiveX` | `PlatformNotSupportedException` | Relied on `BinaryFormatter` and the ActiveX type serializer, neither of which exists in modern .NET. Use `Xml`, `ByteArray` or `Stream`. |

### Receive-side properties

`MaxRetryCycles`, `ReceiveRetryCount`, `RetryCycleDelay`, `ReceiveErrorHandling`,
`ReceiveContextEnabled` and `ValidityDuration` are present for .NET Framework
surface parity. They configure the receive side and have no effect on a
client-side send; host the service with CoreWCF to use them.

`MaxPoolSize` is accepted but not yet honoured — queue handles are opened per
send rather than pooled.

## Feedback & Contributing

To explore the project or contribute, visit our [GitHub repository](https://github.com/dotnet/wcf/).
For reporting issues or providing feedback, please [open an issue on GitHub](https://github.com/dotnet/wcf).
