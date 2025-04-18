// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public partial class ClientBaseTests
{
    [WcfFact]
    [OuterLoop]
    public static void MessageProperty_HttpRequestMessageProperty_RoundTrip_Verify()
    {
        MyClientBase<IWcfService> client = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            client = new MyClientBase<IWcfService>(customBinding, new EndpointAddress(Endpoints.DefaultCustomHttp_Address));
            client.Endpoint.EndpointBehaviors.Add(new ClientMessagePropertyBehavior());
            serviceProxy = client.ChannelFactory.CreateChannel();

            // *** EXECUTE *** \\
            TestHttpRequestMessageProperty property = serviceProxy.EchoHttpRequestMessageProperty();

            // *** VALIDATE *** \\
            Assert.NotNull(property);
            Assert.True(property.SuppressEntityBody == false, "Expected SuppressEntityBody to be 'false'");
            Assert.Equal("POST", property.Method);
            Assert.Equal("My%20address", property.QueryString);
            Assert.True(property.Headers.Count > 0, "TestHttpRequestMessageProperty.Headers should not have empty headers");
            Assert.Equal("my value", property.Headers["customer"]);

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, (ICommunicationObject)client);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void ClientBaseOfT_ServiceEndpointCtor_ExtractedCntrctDscrip()
    {
        MyClientBase<IWcfService> client = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            string endpoint = Endpoints.DefaultCustomHttp_Address;
            client = new MyClientBase<IWcfService>(customBinding, new EndpointAddress(endpoint));
            // Extract the ContractDescription from the channel factory.
            ContractDescription cd = client.ChannelFactory.Endpoint.Contract;
            // Use the ContractDescription to create a new ServiceEndpoint
            ServiceEndpoint serviceEndpoint = new ServiceEndpoint(cd, customBinding, new EndpointAddress(endpoint));

            client = new MyClientBase<IWcfService>(serviceEndpoint);
            client.Endpoint.EndpointBehaviors.Add(new ClientMessagePropertyBehavior());
            serviceProxy = client.ChannelFactory.CreateChannel();

            // *** EXECUTE *** \\
            TestHttpRequestMessageProperty property = serviceProxy.EchoHttpRequestMessageProperty();

            // *** VALIDATE *** \\
            Assert.NotNull(property);
            Assert.True(property.SuppressEntityBody == false, "Expected SuppressEntityBody to be 'false'");
            Assert.Equal("POST", property.Method);
            Assert.Equal("My%20address", property.QueryString);
            Assert.True(property.Headers.Count > 0, "TestHttpRequestMessageProperty.Headers should not have empty headers");
            Assert.Equal("my value", property.Headers["customer"]);

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, (ICommunicationObject)client);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void ClientMessageInspector_Verify_Invoke()
    {
        // This test verifies ClientMessageInspector can be added to the client endpoint behaviors
        // and this is it called properly when a message is sent.

        MyClientBase client = null;
        IWcfServiceGenerated serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            client = new MyClientBase(customBinding, new EndpointAddress(Endpoints.DefaultCustomHttp_Address));

            // Add the ClientMessageInspector and give it an instance where it can record what happens when it is called.
            ClientMessageInspectorData data = new ClientMessageInspectorData();
            client.Endpoint.EndpointBehaviors.Add(new ClientMessageInspectorBehavior(data));

            serviceProxy = client.ChannelFactory.CreateChannel();

            // *** EXECUTE *** \\
            // This proxy call should invoke the client message inspector
            string result = serviceProxy.Echo("Hello");

            // *** VALIDATE *** \\
            Assert.Equal("Hello", result);
            Assert.True(data.BeforeSendRequestCalled, "BeforeSendRequest should have been called");
            Assert.True(data.Request != null, "Did not call pass Request to BeforeSendRequest");
            Assert.True(data.Channel != null, "Did not call pass Channel to BeforeSendRequest");
            Assert.True(data.AfterReceiveReplyCalled, "AfterReceiveReplyCalled should have been called");
            Assert.True(data.Reply != null, "Did not call pass Reply to AfterReceiveReplyCalled");

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, (ICommunicationObject)client);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void ClientBaseOfT_Sync_RoundTrip_Check_CommunicationState()
    {
        MyClientBase client = null;
        IWcfServiceGenerated serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            client = new MyClientBase(customBinding, new EndpointAddress(Endpoints.HttpSoap12_Address));
            // *** VALIDATE *** \\
            Assert.Equal(CommunicationState.Created, client.State);

            serviceProxy = client.ChannelFactory.CreateChannel();
            // *** VALIDATE *** \\
            Assert.Equal(CommunicationState.Opened, client.State);

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo("Hello");
            // *** VALIDATE *** \\
            Assert.Equal(CommunicationState.Opened, client.State);

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            ((ICommunicationObject)serviceProxy).Close();
            // *** VALIDATE *** \\
            Assert.Equal(CommunicationState.Closed, client.State);
        }
        finally
        {
            // normally we'd also check for if (client != null && client.State != CommunicationState.Closed), 
            // but this is a test and it'd be good to have the Abort happen and the channel is still Closed
            if (client != null)
            {
                client.Abort();
                Assert.Equal(CommunicationState.Closed, client.State);
            }

            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, (ICommunicationObject)client);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void ClientBaseOfT_Sync_Open_Close_Factory_And_Proxy_CommunicationState()
    {
        MyClientBase client = null;
        IWcfServiceGenerated serviceProxy = null;
        ChannelFactory<IWcfServiceGenerated> factory = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            string endpoint = Endpoints.HttpSoap12_Address;
            client = new MyClientBase(customBinding, new EndpointAddress(endpoint));
            factory = client.ChannelFactory;

            // *** VALIDATE *** \\
            Assert.True(CommunicationState.Created == client.State,
                        String.Format("Expected client state to be Created but actual was '{0}'", client.State));

            Assert.True(CommunicationState.Created == factory.State,
                        String.Format("Expected channel factory state to be Created but actual was '{0}'", factory.State));

            // Note: both the full framework and this NET Core version open the channel factory
            // when asking for the internal channel.  Attempting to Open the ClientBase
            // after obtaining the internal channel with throw InvalidOperationException attempting
            // to reopen the channel factory.  Customers don't encounter this situation in normal use
            // because access to the internal channel is protected and cannot be acquired.  So we
            // defer asking for the internal channel in this test to allow the Open() to follow the
            // same code path as customer code.

            // *** EXECUTE *** \\
            // Explicitly open the ClientBase to follow general WCF guidelines
            ((ICommunicationObject)client).Open();

            // Use the internal proxy generated by ClientBase to most resemble how svcutil-generated code works.
            // This test defers asking for it until the ClientBase is open to avoid the issue described above.
            serviceProxy = client.Proxy;

            Assert.True(CommunicationState.Opened == client.State,
                        String.Format("Expected client state to be Opened but actual was '{0}'", client.State));

            Assert.True(CommunicationState.Opened == factory.State,
                        String.Format("Expected channel factory state to be Opened but actual was '{0}'", factory.State));

            Assert.True(CommunicationState.Opened == ((ICommunicationObject)serviceProxy).State,
                        String.Format("Expected proxy state to be Opened but actual was '{0}'", ((ICommunicationObject)serviceProxy).State));

            // *** EXECUTE *** \\
            // Explicitly close the ClientBase to follow general WCF guidelines
            ((ICommunicationObject)client).Close();

            // *** VALIDATE *** \\
            // Closing the ClientBase closes the internal channel and factory
            Assert.True(CommunicationState.Closed == client.State,
                        String.Format("Expected client state to be Closed but actual was '{0}'", client.State));

            // Closing the ClientBase also closes the internal channel
            Assert.True(CommunicationState.Closed == ((ICommunicationObject)serviceProxy).State,
                        String.Format("Expected proxy state to be Closed but actual was '{0}'", ((ICommunicationObject)serviceProxy).State));

            // Closing the ClientBase also closes the channel factory
            Assert.True(CommunicationState.Closed == factory.State,
                        String.Format("Expected channel factory state to be Closed but actual was '{0}'", factory.State));
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, client, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void ClientBaseOfT_Async_Open_Close_Factory_And_Proxy_CommunicationState()
    {
        MyClientBase client = null;
        IWcfServiceGenerated serviceProxy = null;
        ChannelFactory<IWcfServiceGenerated> factory = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            string endpoint = Endpoints.HttpSoap12_Address;
            client = new MyClientBase(customBinding, new EndpointAddress(endpoint));
            factory = client.ChannelFactory;

            // *** VALIDATE *** \\
            Assert.True(CommunicationState.Created == client.State,
                        String.Format("Expected client state to be Created but actual was '{0}'", client.State));

            Assert.True(CommunicationState.Created == factory.State,
                        String.Format("Expected channel factory state to be Created but actual was '{0}'", factory.State));

            // Note: both the full framework and this NET Core version open the channel factory
            // when asking for the internal channel.  Attempting to Open the ClientBase
            // after obtaining the internal channel with throw InvalidOperationException attempting
            // to reopen the channel factory.  Customers don't encounter this situation in normal use
            // because access to the internal channel is protected and cannot be acquired.  So we
            // defer asking for the internal channel in this test to allow the Open() to follow the
            // same code path as customer code.

            // *** EXECUTE *** \\
            // Explicitly async open the ClientBase to follow general WCF guidelines
            IAsyncResult ar = ((ICommunicationObject)client).BeginOpen(null, null);
            ((ICommunicationObject)client).EndOpen(ar);

            // Use the internal proxy generated by ClientBase to most resemble how svcutil-generated code works.
            // This test defers asking for it until the ClientBase is open to avoid the issue described above.
            serviceProxy = client.Proxy;

            Assert.True(CommunicationState.Opened == client.State,
                        String.Format("Expected client state to be Opened but actual was '{0}'", client.State));

            Assert.True(CommunicationState.Opened == factory.State,
                        String.Format("Expected channel factory state to be Opened but actual was '{0}'", factory.State));

            Assert.True(CommunicationState.Opened == ((ICommunicationObject)serviceProxy).State,
                        String.Format("Expected proxy state to be Opened but actual was '{0}'", ((ICommunicationObject)serviceProxy).State));

            // *** EXECUTE *** \\
            // Explicitly close the ClientBase to follow general WCF guidelines
            ar = ((ICommunicationObject)client).BeginClose(null, null);
            ((ICommunicationObject)client).EndClose(ar);

            // *** VALIDATE *** \\
            // Closing the ClientBase closes the internal channel and factory
            Assert.True(CommunicationState.Closed == client.State,
                        String.Format("Expected client state to be Closed but actual was '{0}'", client.State));

            // Closing the ClientBase also closes the internal channel
            Assert.True(CommunicationState.Closed == ((ICommunicationObject)serviceProxy).State,
                        String.Format("Expected proxy state to be Closed but actual was '{0}'", ((ICommunicationObject)serviceProxy).State));

            // Closing the ClientBase also closes the channel factory
            Assert.True(CommunicationState.Closed == factory.State,
                        String.Format("Expected channel factory state to be Closed but actual was '{0}'", factory.State));
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, client, factory);
        }
    }


    [WcfFact]
    [OuterLoop]
    public static void ClientBaseOfT_Async_Open_Close_TimeSpan_Factory_And_Proxy_CommunicationState()
    {
        MyClientBase client = null;
        IWcfServiceGenerated serviceProxy = null;
        ChannelFactory<IWcfServiceGenerated> factory = null;
        TimeSpan timeout = ScenarioTestHelpers.TestTimeout;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            string endpoint = Endpoints.HttpSoap12_Address;
            client = new MyClientBase(customBinding, new EndpointAddress(endpoint));
            factory = client.ChannelFactory;

            // *** VALIDATE *** \\
            Assert.True(CommunicationState.Created == client.State,
                        String.Format("Expected client state to be Created but actual was '{0}'", client.State));

            Assert.True(CommunicationState.Created == factory.State,
                        String.Format("Expected channel factory state to be Created but actual was '{0}'", factory.State));

            // Note: both the full framework and this NET Core version open the channel factory
            // when asking for the internal channel.  Attempting to Open the ClientBase
            // after obtaining the internal channel with throw InvalidOperationException attempting
            // to reopen the channel factory.  Customers don't encounter this situation in normal use
            // because access to the internal channel is protected and cannot be acquired.  So we
            // defer asking for the internal channel in this test to allow the Open() to follow the
            // same code path as customer code.

            // *** EXECUTE *** \\
            // Explicitly async open the ClientBase to follow general WCF guidelines
            IAsyncResult ar = ((ICommunicationObject)client).BeginOpen(timeout, null, null);
            ((ICommunicationObject)client).EndOpen(ar);

            // Use the internal proxy generated by ClientBase to most resemble how svcutil-generated code works.
            // This test defers asking for it until the ClientBase is open to avoid the issue described above.
            serviceProxy = client.Proxy;

            Assert.True(CommunicationState.Opened == client.State,
                        String.Format("Expected client state to be Opened but actual was '{0}'", client.State));

            Assert.True(CommunicationState.Opened == factory.State,
                        String.Format("Expected channel factory state to be Opened but actual was '{0}'", factory.State));

            Assert.True(CommunicationState.Opened == ((ICommunicationObject)serviceProxy).State,
                        String.Format("Expected proxy state to be Opened but actual was '{0}'", ((ICommunicationObject)serviceProxy).State));

            // *** EXECUTE *** \\
            // Explicitly close the ClientBase to follow general WCF guidelines
            ar = ((ICommunicationObject)client).BeginClose(timeout, null, null);
            ((ICommunicationObject)client).EndClose(ar);

            // *** VALIDATE *** \\
            // Closing the ClientBase closes the internal channel and factory
            Assert.True(CommunicationState.Closed == client.State,
                        String.Format("Expected client state to be Closed but actual was '{0}'", client.State));

            // Closing the ClientBase also closes the internal channel
            Assert.True(CommunicationState.Closed == ((ICommunicationObject)serviceProxy).State,
                        String.Format("Expected proxy state to be Closed but actual was '{0}'", ((ICommunicationObject)serviceProxy).State));

            // Closing the ClientBase also closes the channel factory
            Assert.True(CommunicationState.Closed == factory.State,
                        String.Format("Expected channel factory state to be Closed but actual was '{0}'", factory.State));
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, client, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void ClientBaseOfT_Abort_Aborts_Factory_And_Proxy()
    {
        MyClientBase client = null;
        IWcfServiceGenerated serviceProxy = null;
        ChannelFactory<IWcfServiceGenerated> factory = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            string endpoint = Endpoints.HttpSoap12_Address;
            client = new MyClientBase(customBinding, new EndpointAddress(endpoint));
            factory = client.ChannelFactory;

            // *** EXECUTE *** \\
            // Explicitly open the ClientBase to follow general WCF guidelines
            ((ICommunicationObject)client).Open();

            // Use the internal proxy generated by ClientBase to most resemble how svcutil-generated code works.
            // Customers cannot normally access this protected member explicitly, but the generated code uses it.
            serviceProxy = client.Proxy;

            // *** EXECUTE *** \\
            client.Abort();

            // *** VALIDATE *** \\
            // Closing the ClientBase closes the internal channel and factory
            Assert.True(CommunicationState.Closed == client.State,
                        String.Format("Expected client state to be Closed but actual was '{0}'", client.State));

            // Closing the ClientBase also closes the internal channel
            Assert.True(CommunicationState.Closed == ((ICommunicationObject)serviceProxy).State,
                        String.Format("Expected proxy state to be Closed but actual was '{0}'", ((ICommunicationObject)serviceProxy).State));

            // Closing the ClientBase also closes the channel factory
            Assert.True(CommunicationState.Closed == factory.State,
                        String.Format("Expected channel factory state to be Closed but actual was '{0}'", factory.State));
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, client, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void ClientBaseOfT_Dispose_Closes_Factory_And_Proxy()
    {
        MyClientBase client = null;
        IWcfServiceGenerated serviceProxy = null;
        ChannelFactory<IWcfServiceGenerated> factory = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            string endpoint = Endpoints.HttpSoap12_Address;
            client = new MyClientBase(customBinding, new EndpointAddress(endpoint));
            factory = client.ChannelFactory;

            // *** EXECUTE *** \\
            // Explicitly open the ClientBase to follow general WCF guidelines
            ((ICommunicationObject)client).Open();

            // Use the internal proxy generated by ClientBase to most resemble how svcutil-generated code works.
            // Customers cannot normally access this protected member explicitly, but the generated code uses it.
            serviceProxy = client.Proxy;

            // *** EXECUTE *** \\
            // ClientBase is IDisposable, which should close the client, factory and proxy
            ((IDisposable)client).Dispose();

            // *** VALIDATE *** \\
            // Closing the ClientBase closes the internal channel and factory
            Assert.True(CommunicationState.Closed == client.State,
                        String.Format("Expected client state to be Closed but actual was '{0}'", client.State));

            // Closing the ClientBase also closes the internal channel
            Assert.True(CommunicationState.Closed == ((ICommunicationObject)serviceProxy).State,
                        String.Format("Expected proxy state to be Closed but actual was '{0}'", ((ICommunicationObject)serviceProxy).State));

            // Closing the ClientBase also closes the channel factory
            Assert.True(CommunicationState.Closed == factory.State,
                        String.Format("Expected channel factory state to be Closed but actual was '{0}'", factory.State));
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, client, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void ClientBaseOfT_Sync_Open_Close_Events_Fire()
    {
        MyClientBase client = null;
        List<string> eventsCalled = new List<string>(4);
        List<string> proxyEventsCalled = new List<string>(4);
        IWcfServiceGenerated proxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            client = new MyClientBase(customBinding, new EndpointAddress(Endpoints.HttpSoap12_Address));

            // Listen to all events on the ClientBase and on the generated proxy
            ClientBaseTestHelpers.RegisterForEvents(client, eventsCalled);

            proxy = client.Proxy;
            ClientBaseTestHelpers.RegisterForEvents((ICommunicationObject)proxy, proxyEventsCalled);

            // *** EXECUTE *** \\
            proxy.Echo("Hello");
            ((ICommunicationObject)client).Close();

            // *** VALIDATE *** \\

            // We expect both the ClientBase and the generated proxy to have fired all the open/close events
            string expected = "Opening,Opened,Closing,Closed";
            string actual = String.Join(",", eventsCalled);

            Assert.True(String.Equals(expected, actual),
                        String.Format("Expected client to receive events '{0}' but actual was '{1}'", expected, actual));

            actual = String.Join(",", proxyEventsCalled);
            Assert.True(String.Equals(expected, actual),
                        String.Format("Expected proxy to receive events '{0}' but actual was '{1}'", expected, actual));

            // *** CLEANUP *** \\
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)proxy, (ICommunicationObject)client);
        }
    }


    [WcfFact]
    [OuterLoop]
    public static void ClientBaseOfT_Async_Open_Close_Events_Fire()
    {
        MyClientBase client = null;
        List<string> eventsCalled = new List<string>(4);
        List<string> proxyEventsCalled = new List<string>(4);
        IWcfServiceGenerated proxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            client = new MyClientBase(customBinding, new EndpointAddress(Endpoints.HttpSoap12_Address));

            // Listen to all events on the ClientBase and on the generated proxy
            ClientBaseTestHelpers.RegisterForEvents(client, eventsCalled);

            proxy = client.Proxy;
            ClientBaseTestHelpers.RegisterForEvents((ICommunicationObject)proxy, proxyEventsCalled);

            // *** EXECUTE *** \\
            Task<string> task = proxy.EchoAsync("Hello");
            task.GetAwaiter().GetResult();

            IAsyncResult ar = ((ICommunicationObject)client).BeginClose(null, null);
            ((ICommunicationObject)client).EndClose(ar);

            // *** VALIDATE *** \\

            // We expect both the ClientBase and the generated proxy to have fired all the open/close events
            string expected = "Opening,Opened,Closing,Closed";
            string actual = String.Join(",", eventsCalled);

            Assert.True(String.Equals(expected, actual),
                        String.Format("Expected client to receive events '{0}' but actual was '{1}'", expected, actual));

            actual = String.Join(",", proxyEventsCalled);
            Assert.True(String.Equals(expected, actual),
                        String.Format("Expected proxy to receive events '{0}' but actual was '{1}'", expected, actual));

            // *** CLEANUP *** \\
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)proxy, (ICommunicationObject)client);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void ClientBaseOfT_Sync_Removed_Open_Close_Events_Do_Not_Fire()
    {
        MyClientBase client = null;
        List<string> eventsCalled = new List<string>(4);
        List<string> proxyEventsCalled = new List<string>(4);
        IWcfServiceGenerated proxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            client = new MyClientBase(customBinding, new EndpointAddress(Endpoints.HttpSoap12_Address));

            // Both Add and Remove event handlers
            ClientBaseTestHelpers.RegisterForEvents(client, eventsCalled, deregister:true);

            proxy = client.Proxy;
            ClientBaseTestHelpers.RegisterForEvents((ICommunicationObject)proxy, proxyEventsCalled, deregister:true);

            // *** EXECUTE *** \\
            proxy.Echo("Hello");
            ((ICommunicationObject)client).Close();

            // *** VALIDATE *** \\

            // We expect both the ClientBase and the generated proxy to have NOT fired all the open/close events
            string actual = String.Join(",", eventsCalled);

            Assert.True(eventsCalled.Count == 0,
                        String.Format("Expected client NOT to receive events but actual was '{0}'", actual));

            actual = String.Join(",", proxyEventsCalled);
            Assert.True(proxyEventsCalled.Count == 0,
                        String.Format("Expected proxy NOT to receive events but actual was '{0}'", actual));

            // *** CLEANUP *** \\
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)proxy, (ICommunicationObject)client);
        }
    }


    [WcfFact]
    [OuterLoop]
    public static void ClientBaseOfT_Sync_RoundTrip_Call_Using_HttpTransport()
    {
        // This test verifies ClientBase<T> can be used to create a proxy and invoke an operation over Http

        MyClientBase client = null;
        IWcfServiceGenerated serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            client = new MyClientBase(customBinding, new EndpointAddress(Endpoints.DefaultCustomHttp_Address));
            serviceProxy = client.ChannelFactory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo("Hello");

            // *** VALIDATE *** \\
            Assert.Equal("Hello", result);

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, (ICommunicationObject)client);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void ClientBaseOfT_Sync_RoundTrip_Call_Using_NetTcpTransport()
    {
        // This test verifies ClientBase<T> can be used to create a proxy and invoke an operation over Tcp
        // (request reply over Tcp) 

        MyClientBase client = null;
        IWcfServiceGenerated serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding binding = new CustomBinding(new TextMessageEncodingBindingElement(), new TcpTransportBindingElement());

            client = new MyClientBase(binding, new EndpointAddress(Endpoints.Tcp_CustomBinding_NoSecurity_Text_Address));
            serviceProxy = client.ChannelFactory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo("Hello");

            // *** VALIDATE *** \\
            Assert.Equal("Hello", result);

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, (ICommunicationObject)client);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void OperationContextScope_HttpRequestCustomMessageHeader_RoundTrip_Verify()
    {
        string customHeaderName = "OperationContextScopeCustomHeader";
        string customHeaderNS = "http://tempuri.org/OperationContextScope_HttpRequestCustomMessageHeader_RoundTrip_Verify";
        string customHeaderValue = "CustomHappyValue";

        MyClientBase<IWcfService> client = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpBinding binding = new BasicHttpBinding();

            client = new MyClientBase<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            serviceProxy = client.ChannelFactory.CreateChannel();

            using (OperationContextScope scope = new OperationContextScope((IContextChannel)serviceProxy))
            {
                MessageHeader header
                  = MessageHeader.CreateHeader(
                  customHeaderName,
                  customHeaderNS,
                  customHeaderValue
                  );
                OperationContext.Current.OutgoingMessageHeaders.Add(header);

                // *** EXECUTE *** \\
                Dictionary<string, string> incomingMessageHeaders = serviceProxy.GetIncomingMessageHeaders();
                string result = ClientBaseTestHelpers.GetHeader(customHeaderName, customHeaderNS, incomingMessageHeaders);

                // *** VALIDATE *** \\
                Assert.Equal(customHeaderValue, result);
            }

            // *** EXECUTE *** \\
            //Call outside of scope should not have the custom header
            Dictionary<string, string> outofScopeIncomingMessageHeaders = serviceProxy.GetIncomingMessageHeaders();
            string outofScopeResult = ClientBaseTestHelpers.GetHeader(customHeaderName, customHeaderNS, outofScopeIncomingMessageHeaders);

            // *** VALIDATE *** \\
            Assert.True(string.Empty == outofScopeResult, string.Format("Expect call out of the OperationContextScope does not have the custom header {0}", customHeaderName));

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, (ICommunicationObject)client);
        }
    }
}

