// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using Infrastructure.Common;
using Xunit;

public static class WebHttpBehaviorTest
{
    private static ServiceEndpoint CreateEndpoint(Type contractType, Binding binding = null)
    {
        return new ServiceEndpoint(
            ContractDescription.GetContract(contractType),
            binding ?? new WebHttpBinding(),
            new EndpointAddress("http://localhost:8080/svc"));
    }

    // ClientRuntime has no public constructor, so the only way to obtain one is to let the real
    // ChannelFactory pipeline build it and capture the instance handed to endpoint behaviors.
    private sealed class ClientRuntimeCapturingBehavior : IEndpointBehavior
    {
        public ClientRuntime ClientRuntime { get; private set; }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            ClientRuntime = clientRuntime;
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    private static ClientRuntime ApplyClientBehavior(ServiceEndpoint endpoint, WebHttpBehavior behavior = null)
    {
        endpoint.Behaviors.Add(behavior ?? new WebHttpBehavior());

        ClientRuntimeCapturingBehavior capturing = new ClientRuntimeCapturingBehavior();
        endpoint.Behaviors.Add(capturing);

        ChannelFactory<IRequestChannel> factory = new ChannelFactory<IRequestChannel>(endpoint);
        try
        {
            factory.Open();
        }
        finally
        {
            // Abort rather than Close/Dispose: if Open threw, the factory is faulted and
            // disposing it would mask the original exception.
            factory.Abort();
        }

        Assert.NotNull(capturing.ClientRuntime);
        return capturing.ClientRuntime;
    }

    private static ClientOperation FindOperation(ClientRuntime runtime, string name)
    {
        foreach (ClientOperation operation in runtime.Operations)
        {
            if (operation.Name == name)
            {
                return operation;
            }
        }

        return null;
    }

    private static Message ReadReplyMessage(string payload, string contentType)
    {
        MessageEncoder encoder = new WebMessageEncodingBindingElement().CreateMessageEncoderFactory().Encoder;
        return encoder.ReadMessage(
            new MemoryStream(Encoding.UTF8.GetBytes(payload), false),
            int.MaxValue,
            contentType);
    }

    [WcfFact]
    public static void Defaults_Match_Documented_Values()
    {
        WebHttpBehavior behavior = new WebHttpBehavior();

        Assert.Equal(WebMessageBodyStyle.Bare, behavior.DefaultBodyStyle);
        Assert.Equal(WebMessageFormat.Xml, behavior.DefaultOutgoingRequestFormat);
        Assert.Equal(WebMessageFormat.Xml, behavior.DefaultOutgoingResponseFormat);
        Assert.False(behavior.HelpEnabled);
        Assert.False(behavior.AutomaticFormatSelectionEnabled);
        Assert.False(behavior.FaultExceptionEnabled);
    }

    [WcfFact]
    public static void Properties_RoundTrip()
    {
        WebHttpBehavior behavior = new WebHttpBehavior
        {
            DefaultBodyStyle = WebMessageBodyStyle.Wrapped,
            DefaultOutgoingRequestFormat = WebMessageFormat.Json,
            DefaultOutgoingResponseFormat = WebMessageFormat.Json,
            HelpEnabled = true,
            AutomaticFormatSelectionEnabled = true,
            FaultExceptionEnabled = true
        };

        Assert.Equal(WebMessageBodyStyle.Wrapped, behavior.DefaultBodyStyle);
        Assert.Equal(WebMessageFormat.Json, behavior.DefaultOutgoingRequestFormat);
        Assert.Equal(WebMessageFormat.Json, behavior.DefaultOutgoingResponseFormat);
        Assert.True(behavior.HelpEnabled);
        Assert.True(behavior.AutomaticFormatSelectionEnabled);
        Assert.True(behavior.FaultExceptionEnabled);
    }

    [WcfFact]
    public static void Rejects_Undefined_Enum_Values()
    {
        WebHttpBehavior behavior = new WebHttpBehavior();

        Assert.Throws<ArgumentOutOfRangeException>(() => behavior.DefaultBodyStyle = (WebMessageBodyStyle)99);
        Assert.Throws<ArgumentOutOfRangeException>(() => behavior.DefaultOutgoingRequestFormat = (WebMessageFormat)99);
        Assert.Throws<ArgumentOutOfRangeException>(() => behavior.DefaultOutgoingResponseFormat = (WebMessageFormat)99);
    }

    [WcfFact]
    public static void Validate_Accepts_A_WebHttpBinding_Endpoint()
    {
        new WebHttpBehavior().Validate(CreateEndpoint(typeof(IWebHttpBindingTestService)));
    }

    [WcfFact]
    public static void Validate_Null_Endpoint_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new WebHttpBehavior().Validate(null));
    }

    [WcfFact]
    public static void Validate_Rejects_Non_Web_Binding()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpBindingTestService), new BasicHttpBinding());

        Assert.Throws<InvalidOperationException>(() => new WebHttpBehavior().Validate(endpoint));
    }

    [WcfFact]
    public static void Validate_Rejects_Custom_Binding_Without_Web_Encoder()
    {
        CustomBinding binding = new CustomBinding(
            new TextMessageEncodingBindingElement(MessageVersion.None, System.Text.Encoding.UTF8),
            new HttpTransportBindingElement());
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpBindingTestService), binding);

        Assert.Throws<InvalidOperationException>(() => new WebHttpBehavior().Validate(endpoint));
    }

    [WcfFact]
    public static void Validate_Accepts_Custom_Binding_With_Web_Encoder()
    {
        CustomBinding binding = new CustomBinding(
            new WebMessageEncodingBindingElement(),
            new HttpTransportBindingElement { ManualAddressing = true });
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpBindingTestService), binding);

        new WebHttpBehavior().Validate(endpoint);
    }

    [WcfFact]
    public static void Validate_Requires_ManualAddressing_On_The_Transport()
    {
        CustomBinding binding = new CustomBinding(
            new WebMessageEncodingBindingElement(),
            new HttpTransportBindingElement { ManualAddressing = false });
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpBindingTestService), binding);

        Assert.Throws<InvalidOperationException>(() => new WebHttpBehavior().Validate(endpoint));
    }

    [WcfFact]
    public static void ApplyClientBehavior_Rejects_Unconvertible_UriTemplate_Parameter()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpUnconvertibleParameterService));

        Assert.ThrowsAny<InvalidOperationException>(() => ApplyClientBehavior(endpoint));
    }

    [WcfFact]
    public static void AddBindingParameters_Is_A_No_Op()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpBindingTestService));
        BindingParameterCollection parameters = new BindingParameterCollection();

        new WebHttpBehavior().AddBindingParameters(endpoint, parameters);

        Assert.Empty(parameters);
    }

    [WcfFact]
    public static void ApplyDispatchBehavior_Validates_Its_Arguments()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpBindingTestService));

        Assert.Throws<ArgumentNullException>(() => new WebHttpBehavior().ApplyDispatchBehavior(null, null));
        Assert.Throws<ArgumentNullException>(() => new WebHttpBehavior().ApplyDispatchBehavior(endpoint, null));
    }

    [WcfFact]
    public static void ApplyClientBehavior_Null_ClientRuntime_Throws()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpBindingTestService));

        Assert.Throws<ArgumentNullException>(() => new WebHttpBehavior().ApplyClientBehavior(endpoint, null));
    }

    [WcfFact]
    public static void ApplyClientBehavior_Installs_Formatters_On_Every_Operation()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpBindingTestService));

        ClientRuntime runtime = ApplyClientBehavior(endpoint);

        Assert.NotEmpty(runtime.Operations);
        foreach (ClientOperation operation in runtime.Operations)
        {
            Assert.NotNull(operation.Formatter);
            Assert.True(operation.SerializeRequest, "WebHttpBehavior must take over request serialization");
            Assert.True(operation.DeserializeReply, "WebHttpBehavior must take over reply deserialization");
        }
    }

    [WcfFact]
    public static void ApplyClientBehavior_Installs_A_Fault_Inspector()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpBindingTestService));

        ClientRuntime runtime = ApplyClientBehavior(endpoint);

        Assert.NotEmpty(runtime.ClientMessageInspectors);
    }

    [WcfFact]
    public static void ApplyClientBehavior_Handles_Operations_Without_Web_Attributes()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpUnattributedTestService));

        ClientRuntime runtime = ApplyClientBehavior(endpoint);

        ClientOperation operation = FindOperation(runtime, "Echo");
        Assert.NotNull(operation);
        Assert.NotNull(operation.Formatter);
    }

    [WcfFact]
    public static void ApplyClientBehavior_Handles_Stream_Operations()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpRawTestService));

        ClientRuntime runtime = ApplyClientBehavior(endpoint);

        ClientOperation operation = FindOperation(runtime, "EchoStream");
        Assert.NotNull(operation);
        Assert.NotNull(operation.Formatter);
    }

    [WcfFact]
    public static void Request_Formatter_Writes_Json_ContentType_And_Null_Body_Suppression_To_Ambient_Properties()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpJsonTestService));
        ClientRuntime runtime = ApplyClientBehavior(endpoint);
        ClientOperation operation = FindOperation(runtime, "EchoJson");
        ChannelFactory<IRequestChannel> contextFactory = new ChannelFactory<IRequestChannel>(
            new WebHttpBinding(), new EndpointAddress("http://localhost:8080/context"));
        IRequestChannel contextChannel = contextFactory.CreateChannel();

        try
        {
            using (new OperationContextScope((IContextChannel)contextChannel))
            {
                HttpRequestMessageProperty customProperty = new HttpRequestMessageProperty();
                customProperty.Headers["X-Custom"] = "value";
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = customProperty;

                using (Message message = operation.Formatter.SerializeRequest(MessageVersion.None, new object[] { null }))
                {
                    HttpRequestMessageProperty ambientProperty =
                        (HttpRequestMessageProperty)OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name];

                    Assert.Equal("POST", ambientProperty.Method);
                    Assert.True(ambientProperty.SuppressEntityBody);
                    Assert.Equal("application/json; charset=utf-8",
                        ambientProperty.Headers[HttpRequestHeader.ContentType]);
                }
            }
        }
        finally
        {
            ((ICommunicationObject)contextChannel).Abort();
            contextFactory.Abort();
        }
    }

    [WcfFact]
    public static void Request_Formatter_Writes_Json_ContentType_To_Message_When_Ambient_Context_Has_No_Properties()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpJsonTestService));
        ClientRuntime runtime = ApplyClientBehavior(endpoint);
        ClientOperation operation = FindOperation(runtime, "EchoWildcardJson");
        ChannelFactory<IRequestChannel> contextFactory = new ChannelFactory<IRequestChannel>(
            new WebHttpBinding(), new EndpointAddress("http://localhost:8080/context"));
        IRequestChannel contextChannel = contextFactory.CreateChannel();

        try
        {
            using (new OperationContextScope((IContextChannel)contextChannel))
            using (Message message = operation.Formatter.SerializeRequest(
                MessageVersion.None, new object[] { "value" }))
            {
                HttpRequestMessageProperty messageProperty =
                    (HttpRequestMessageProperty)message.Properties[HttpRequestMessageProperty.Name];

                Assert.Equal("application/json; charset=utf-8",
                    messageProperty.Headers[HttpRequestHeader.ContentType]);
            }
        }
        finally
        {
            ((ICommunicationObject)contextChannel).Abort();
            contextFactory.Abort();
        }
    }

    [WcfFact]
    public static void ApplyClientBehavior_Result_Is_Immutable_After_The_Factory_Opens()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpBindingTestService));
        WebHttpBehavior behavior = new WebHttpBehavior();
        ClientRuntime runtime = ApplyClientBehavior(endpoint, behavior);

        // Behaviors only get to shape the runtime while the factory is opening.
        Assert.Throws<InvalidOperationException>(() => behavior.ApplyClientBehavior(endpoint, runtime));
    }

    [WcfFact]
    public static void ApplyClientBehavior_Honors_Json_Default_Formats()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpUnattributedTestService));
        WebHttpBehavior behavior = new WebHttpBehavior
        {
            DefaultOutgoingRequestFormat = WebMessageFormat.Json,
            DefaultOutgoingResponseFormat = WebMessageFormat.Json
        };

        ClientRuntime runtime = ApplyClientBehavior(endpoint, behavior);

        Assert.NotNull(FindOperation(runtime, "Echo").Formatter);
    }

    [WcfFact]
    public static void Reply_Formatter_Uses_Json_Message_Format_When_Xml_Is_Declared()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpBindingTestService));
        ClientRuntime runtime = ApplyClientBehavior(endpoint);
        ClientOperation operation = FindOperation(runtime, "EchoWithGet");

        using (Message reply = ReadReplyMessage("\"hello\"", "application/json"))
        {
            object result = operation.Formatter.DeserializeReply(reply, Array.Empty<object>());

            Assert.Equal("hello", result);
        }
    }

    [WcfFact]
    public static void Reply_Formatter_Uses_Xml_Message_Format_When_Json_Is_Declared()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpJsonTestService));
        ClientRuntime runtime = ApplyClientBehavior(endpoint);
        ClientOperation operation = FindOperation(runtime, "EchoJson");
        const string Payload = "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">hello</string>";

        using (Message reply = ReadReplyMessage(Payload, "application/xml"))
        {
            object result = operation.Formatter.DeserializeReply(reply, Array.Empty<object>());

            Assert.Equal("hello", result);
        }
    }

    [WcfFact]
    public static void Wrapped_Reply_Formatter_Uses_Actual_Message_Format()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpWrappedReplyTestService));
        ClientRuntime runtime = ApplyClientBehavior(endpoint);
        ClientOperation operation = FindOperation(runtime, "EchoWrapped");

        using (Message reply = ReadReplyMessage("{\"EchoWrappedResult\":\"hello\"}", "application/json"))
        {
            object result = operation.Formatter.DeserializeReply(reply, Array.Empty<object>());

            Assert.Equal("hello", result);
        }
    }

    [WcfFact]
    public static void Reply_Formatter_Defaults_To_Xml_When_Format_Property_Is_Missing()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpBindingTestService));
        ClientRuntime runtime = ApplyClientBehavior(endpoint);
        ClientOperation operation = FindOperation(runtime, "EchoWithGet");
        MessageEncoder encoder = new TextMessageEncodingBindingElement(MessageVersion.None, Encoding.UTF8)
            .CreateMessageEncoderFactory().Encoder;
        const string Payload = "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">hello</string>";

        using (Message reply = encoder.ReadMessage(
            new MemoryStream(Encoding.UTF8.GetBytes(Payload), false),
            int.MaxValue,
            "application/xml"))
        {
            Assert.False(reply.Properties.ContainsKey(WebBodyFormatMessageProperty.Name));

            object result = operation.Formatter.DeserializeReply(reply, Array.Empty<object>());

            Assert.Equal("hello", result);
        }
    }

    [WcfFact]
    public static void Reply_Formatter_Rejects_Unsupported_Actual_Message_Format()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpBindingTestService));
        ClientRuntime runtime = ApplyClientBehavior(endpoint);
        ClientOperation operation = FindOperation(runtime, "EchoWithGet");

        using (Message reply = ReadReplyMessage("raw", "application/octet-stream"))
        {
            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
                () => operation.Formatter.DeserializeReply(reply, Array.Empty<object>()));

            Assert.Contains("Raw", exception.Message);
            Assert.Contains("Xml", exception.Message);
            Assert.Contains("Json", exception.Message);
        }
    }

    [WcfFact]
    public static void ApplyClientBehavior_Honors_Wrapped_Body_Style()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpUnattributedTestService));
        WebHttpBehavior behavior = new WebHttpBehavior
        {
            DefaultBodyStyle = WebMessageBodyStyle.Wrapped
        };

        ClientRuntime runtime = ApplyClientBehavior(endpoint, behavior);

        Assert.NotNull(FindOperation(runtime, "Echo").Formatter);
    }

    private sealed class CustomQueryStringConverterBehavior : WebHttpBehavior
    {
        public int GetQueryStringConverterCallCount { get; private set; }

        protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
        {
            GetQueryStringConverterCallCount++;
            return base.GetQueryStringConverter(operationDescription);
        }
    }

    [WcfFact]
    public static void Derived_Behavior_Can_Override_QueryStringConverter()
    {
        ServiceEndpoint endpoint = CreateEndpoint(typeof(IWebHttpBindingTestService));
        CustomQueryStringConverterBehavior behavior = new CustomQueryStringConverterBehavior();

        ApplyClientBehavior(endpoint, behavior);

        Assert.True(behavior.GetQueryStringConverterCallCount > 0,
            "GetQueryStringConverter should be consulted while applying client behavior");
    }

}
