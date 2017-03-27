// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Infrastructure.Common;
using Xunit;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

public static class MessageContractTests
{
    public static MessageHeader customHeaderMustUnderstand_True = MessageHeader.CreateHeader("MustUnderstand_True", "http://tempuri.org/MustUnderstand_True_Namespace", "EmptyObject", true);
    public static MessageHeader customHeaderMustUnderstand_False = MessageHeader.CreateHeader("MustUnderstand_False", "http://tempuri.org/MustUnderstand_False_Namespace", "EmptyObject", false);

    [WcfFact]
    [OuterLoop]
    public static void MustUnderstand_False_TrueNegative_TruePositive()
    {
        // When a Message arrives it travels through the WCF stack at the end of which it is passed to the ServiceModel layer.
        // The MustUnderstand property for every header in the Message is either true or false.
        // At the end of the stack a check is done, every header with MustUnderstand set to "true" must be found in the...
        // UnderstoodHeaders collection.

        // This test makes three calls to a service opration.
        // The service operation creates a MessageHeader using data passed to it in the call.
        // Each call validates a scenario variation.

        UnderstoodHeadersInspector inspector = null;
        ChannelFactory<IUnderstoodHeaders> factory = null;
        BasicHttpBinding binding = null;
        IUnderstoodHeaders proxy = null;

        // *** SETUP *** \\
        binding = new BasicHttpBinding();
        factory = new ChannelFactory<IUnderstoodHeaders>(binding, new EndpointAddress(Endpoints.UnderstoodHeaders));
        inspector = new UnderstoodHeadersInspector();
        factory.Endpoint.EndpointBehaviors.Add(inspector);
        proxy = factory.CreateChannel();

        // ***  EXECUTE 1st Variation *** \\
        // Custom header with MustUnderstand set to "false", this should just work.
        try
        {
            proxy.CreateMessageHeader(customHeaderMustUnderstand_False.Name, customHeaderMustUnderstand_False.Namespace, customHeaderMustUnderstand_False.MustUnderstand);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)proxy);
        }

        // ***  EXECUTE 2nd Variation *** \\
        // Custom header with MustUnderstand set to "true" this should result in an exception since nothing...
        // on the client side stack is adding this header to UnderstoodHeaders.
        Assert.Throws<ProtocolException>(() =>
        {
            try
            {
                proxy = factory.CreateChannel();
                proxy.CreateMessageHeader(customHeaderMustUnderstand_True.Name, customHeaderMustUnderstand_True.Namespace, customHeaderMustUnderstand_True.MustUnderstand);
            }
            finally
            {
                // *** ENSURE CLEANUP *** \\
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)proxy);
            }
        });

        // ***  EXECUTE 3d Variation *** \\
        // Custom header with MustUnderstand set to "True", this should pass because...
        // we are using a Message Inspector to add the header to UnderstoodHeaders.
        // This mimics the scenario where the client was expecting this header, did what it needed to do...
        // and then added it to the UnderstoodHeaders collection as confirmation that it was handled.
        try
        {
            proxy = factory.CreateChannel();
            inspector.DoNothing = false;
            proxy.CreateMessageHeader(customHeaderMustUnderstand_True.Name, customHeaderMustUnderstand_True.Namespace, customHeaderMustUnderstand_True.MustUnderstand);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)proxy, factory);
        }
    }
}

[ServiceContract]
public interface IUnderstoodHeaders
{
    [OperationContract]
    void CreateMessageHeader(string headerName, string headerNameSpace, bool mustUnderstand);
}

public class UnderstoodHeadersInspector : IClientMessageInspector, IEndpointBehavior
{
    private bool _doNothing = true;
    public bool DoNothing
    {
        get
        {
            return _doNothing;
        }
        set
        {
            _doNothing = value;
        }
    }

    public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
    {
        if (_doNothing == false)
        {
            int pos = reply.Headers.FindHeader("MustUnderstand_True", "http://tempuri.org/MustUnderstand_True_Namespace");
            reply.Headers.UnderstoodHeaders.Add(reply.Headers[pos]);
        }
    }

    public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel)
    {
        return null;
    }

    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    { }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        clientRuntime.ClientMessageInspectors.Add(this);
    }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    { }

    public void Validate(ServiceEndpoint endpoint)
    { }
}
