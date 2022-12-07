// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

[assembly: FailFastAfter("00:01:00")]

public static partial class ChannelBaseTests_4_0_0
{
    [WcfFact]
    [OuterLoop]
    public static void ChannelBaseOfT_Asynchronous_RoundTrip()
    {
        // This test verifies ClientBase<T> can be used to create a custom ChannelBase<T> proxy and invoke an operation over Http

        MyClientBaseWithChannelBase client = null;
        IWcfServiceBeginEndGenerated serviceProxy = null;
        string echoText = "Hello";

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            client = new MyClientBaseWithChannelBase(customBinding, new EndpointAddress(Endpoints.DefaultCustomHttp_Address));
            serviceProxy = client.Proxy;

            // *** EXECUTE *** \\
            // Note: public contract supports only async use on ChannelBase<T>
            IAsyncResult ar = serviceProxy.BeginEcho(echoText, callback: null, asyncState: null);
            string result = serviceProxy.EndEcho(ar);

            // *** VALIDATE *** \\
            Assert.True(String.Equals(echoText, result),
                        String.Format("Expected response was '{0}' but actual was '{1}'", echoText, result));

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
    public static void ChannelBaseOfT_Sync_Open_Close_Events_Fire()
    {
        MyClientBaseWithChannelBase client = null;
        List<string> eventsCalled = new List<string>(4);
        List<string> proxyEventsCalled = new List<string>(4);
        IWcfServiceBeginEndGenerated serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            client = new MyClientBaseWithChannelBase(customBinding, new EndpointAddress(Endpoints.HttpSoap12_Address));

            // Listen to all events on the ClientBase and on the generated proxy
            ClientBaseTestHelpers.RegisterForEvents(client, eventsCalled);

            serviceProxy = client.Proxy;
            ClientBaseTestHelpers.RegisterForEvents((ICommunicationObject)serviceProxy, proxyEventsCalled);

            // *** EXECUTE *** \\
            IAsyncResult ar = serviceProxy.BeginEcho("Hello", callback: null, asyncState: null);
            string result = serviceProxy.EndEcho(ar);
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
            // No further cleanup is needed because the EXECUTE portion accomplished that.
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, (ICommunicationObject)client);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void ChannelBaseOfT_Async_Open_Close_Events_Fire()
    {
        MyClientBaseWithChannelBase client = null;
        List<string> eventsCalled = new List<string>(4);
        List<string> proxyEventsCalled = new List<string>(4);
        IWcfServiceBeginEndGenerated serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            client = new MyClientBaseWithChannelBase(customBinding, new EndpointAddress(Endpoints.HttpSoap12_Address));

            // Listen to all events on the ClientBase and on the generated proxy
            ClientBaseTestHelpers.RegisterForEvents(client, eventsCalled);

            serviceProxy = client.Proxy;
            ClientBaseTestHelpers.RegisterForEvents((ICommunicationObject)serviceProxy, proxyEventsCalled);

            // *** EXECUTE *** \\
            IAsyncResult ar = serviceProxy.BeginEcho("Hello", callback: null, asyncState: null);
            string result = serviceProxy.EndEcho(ar);

            ar = ((ICommunicationObject)client).BeginClose(null, null);
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
            // No further cleanup is needed because the EXECUTE portion accomplished that.
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, (ICommunicationObject)client);
        }
    }


    [WcfFact]
    [OuterLoop]
    public static void ChannelBaseOfT_Sync_Removed_Open_Close_Events_Do_Not_Fire()
    {
        MyClientBaseWithChannelBase client = null;
        List<string> eventsCalled = new List<string>(4);
        List<string> proxyEventsCalled = new List<string>(4);
        IWcfServiceBeginEndGenerated serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            client = new MyClientBaseWithChannelBase(customBinding, new EndpointAddress(Endpoints.HttpSoap12_Address));

            // Listen to all events on the ClientBase and on the generated proxy
            ClientBaseTestHelpers.RegisterForEvents(client, eventsCalled, deregister: true);

            serviceProxy = client.Proxy;
            ClientBaseTestHelpers.RegisterForEvents((ICommunicationObject)serviceProxy, proxyEventsCalled, deregister: true);

            // *** EXECUTE *** \\
            IAsyncResult ar = serviceProxy.BeginEcho("Hello", callback: null, asyncState: null);
            string result = serviceProxy.EndEcho(ar);
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
            // No further cleanup is needed because the EXECUTE portion accomplished that.
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, (ICommunicationObject)client);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void ClientBaseOfT_Dispose_Closes_Factory_And_Proxy()
    {
        MyClientBaseWithChannelBase client = null;
        IWcfServiceBeginEndGenerated serviceProxy = null;
        ChannelFactory<IWcfServiceBeginEndGenerated> factory = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            string endpoint = Endpoints.HttpSoap12_Address;
            client = new MyClientBaseWithChannelBase(customBinding, new EndpointAddress(endpoint));
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

            // *** CLEANUP *** \\
            // No further cleanup is needed because the EXECUTE portion accomplished that.
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, client, factory);
        }
    }
}

