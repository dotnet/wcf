// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public class DuplexClientBaseTest
{
    [WcfFact]
    public static void DuplexClientBase_Ctor_Initializes_State()
    {
        InstanceContext context = new InstanceContext(new WcfDuplexServiceCallback());
        Binding binding = new NetTcpBinding();
        EndpointAddress endpoint = new EndpointAddress(FakeAddress.TcpAddress);
        MyDuplexClientBase<IWcfDuplexService> duplexClientBase = new MyDuplexClientBase<IWcfDuplexService>(context, binding, endpoint);

        Assert.Equal<EndpointAddress>(endpoint, duplexClientBase.Endpoint.Address);
        Assert.Equal<CommunicationState>(CommunicationState.Created, duplexClientBase.State);

        duplexClientBase.Abort();
    }

    [WcfFact]
    public static void DuplexClientBase_Aborts_Changes_CommunicationState()
    {
        InstanceContext context = new InstanceContext(new WcfDuplexServiceCallback());
        Binding binding = new NetTcpBinding();
        EndpointAddress endpoint = new EndpointAddress(FakeAddress.TcpAddress);
        MyDuplexClientBase<IWcfDuplexService> duplexClientBase = new MyDuplexClientBase<IWcfDuplexService>(context, binding, endpoint);

        Assert.Equal<CommunicationState>(CommunicationState.Created, duplexClientBase.State);
        duplexClientBase.Abort();
        Assert.Equal<CommunicationState>(CommunicationState.Closed, duplexClientBase.State);
    }

    [WcfFact]
    public static void CreateDuplexClientBase_NullContext_Throws()
    {
        Binding binding = new NetTcpBinding();
        EndpointAddress endpoint = new EndpointAddress(FakeAddress.TcpAddress);
        Assert.Throws<ArgumentNullException>("callbackInstance", () => { MyDuplexClientBase<IWcfDuplexService> duplexClientBase = new MyDuplexClientBase<IWcfDuplexService>(null, binding, endpoint); });
    }

    [WcfFact]
    public static void CreateDuplexClientBase_NullBinding_Throws()
    {
        InstanceContext context = new InstanceContext(new WcfDuplexServiceCallback());
        EndpointAddress endpoint = new EndpointAddress(FakeAddress.TcpAddress);
        Assert.Throws<ArgumentNullException>("binding", () => { MyDuplexClientBase<IWcfDuplexService> duplexClientBase = new MyDuplexClientBase<IWcfDuplexService>(context, null, endpoint); });
    }

    [WcfFact]
    public static void CreateDuplexClientBase_NullEndpoint_Throws()
    {
        InstanceContext context = new InstanceContext(new WcfDuplexServiceCallback());
        Binding binding = new NetTcpBinding();
        Assert.Throws<ArgumentNullException>("remoteAddress", () => { MyDuplexClientBase<IWcfDuplexService> duplexClientBase = new MyDuplexClientBase<IWcfDuplexService>(context, binding, null); });
    }

    [WcfFact]
    public static void CreateDuplexClientBase_Binding_Url_Mismatch_Throws()
    {
        InstanceContext context = new InstanceContext(new WcfDuplexServiceCallback());
        Binding binding = new NetTcpBinding();
        EndpointAddress endpoint = new EndpointAddress(FakeAddress.HttpAddress);
        Assert.Throws<ArgumentException>("via", () =>
        {
            MyDuplexClientBase<IWcfDuplexService> duplexClientBase = new MyDuplexClientBase<IWcfDuplexService>(context, binding, endpoint);
            ((ICommunicationObject)duplexClientBase).Open();
        });
    }

    // private to this class for testing purposes 
    private class WcfDuplexServiceCallback : IWcfDuplexServiceCallback
    {
        public Task<Guid> OnPingCallback(Guid guid)
        {
            return Task.FromResult<Guid>(guid);
        }

        void IWcfDuplexServiceCallback.OnPingCallback(Guid guid)
        {
        }
    }
}
