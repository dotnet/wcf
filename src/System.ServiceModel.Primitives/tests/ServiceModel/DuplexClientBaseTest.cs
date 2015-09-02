// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Xunit;

public class DuplexClientBaseTest
{
    [Fact]
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

    [Fact]
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

    [Fact]
    public static void CreateDuplexClientBase_NullContext_Throws()
    {
        Binding binding = new NetTcpBinding();
        EndpointAddress endpoint = new EndpointAddress(FakeAddress.TcpAddress);
        Assert.Throws<ArgumentNullException>("callbackInstance", () => { MyDuplexClientBase<IWcfDuplexService> duplexClientBase = new MyDuplexClientBase<IWcfDuplexService>(null, binding, endpoint); }); 
    }

    [Fact]
    public static void CreateDuplexClientBase_NullBinding_Throws()
    {
        InstanceContext context = new InstanceContext(new WcfDuplexServiceCallback());
        EndpointAddress endpoint = new EndpointAddress(FakeAddress.TcpAddress);
        Assert.Throws<ArgumentNullException>("binding", () => { MyDuplexClientBase<IWcfDuplexService> duplexClientBase = new MyDuplexClientBase<IWcfDuplexService>(context, null, endpoint); });
    }

    [Fact]
    public static void CreateDuplexClientBase_NullEndpoint_Throws()
    {
        InstanceContext context = new InstanceContext(new WcfDuplexServiceCallback());
        Binding binding = new NetTcpBinding();
        Assert.Throws<ArgumentNullException>("remoteAddress", () => { MyDuplexClientBase<IWcfDuplexService> duplexClientBase = new MyDuplexClientBase<IWcfDuplexService>(context, binding, null); });
    }
    
    [Fact]
    [ActiveIssue(301)]
    public static void CreateDuplexClientBase_Binding_Url_Mismatch_Throws()
    {
        InstanceContext context = new InstanceContext(new WcfDuplexServiceCallback());
        Binding binding = new NetTcpBinding();
        EndpointAddress endpoint = new EndpointAddress(FakeAddress.HttpAddress);
        Assert.Throws<ArgumentException>("via", () => {
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
