// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;
using System.Threading.Tasks;

public static partial class ClientBaseTest
{
    [WcfFact]
    public static void ClientBaseCloseMethodClosesCorrectly()
    {
        // *** SETUP *** \\
        BasicHttpBinding binding = new BasicHttpBinding();
        MyClientBase client = new MyClientBase(binding, new EndpointAddress("http://myendpoint"));

        // *** VALIDATE *** \\
        Assert.Equal(CommunicationState.Created, client.State);
        client.Open();
        Assert.Equal(CommunicationState.Opened, client.State);
        client.Close();
        Assert.Equal(CommunicationState.Closed, client.State);
    }

    [WcfFact]
    public static void ClientBaseChannelFactoryDefaultCachingBindingEndpointAddress()
    {
        Assert.Equal(CacheSetting.Default, MyClientBase.CacheSetting);
        BasicHttpBinding binding = new BasicHttpBinding();
        EndpointAddress endpointAddress = new EndpointAddress("http://myendpoint");
        MyClientBase client1 = new MyClientBase(binding, endpointAddress);
        client1.Open();
        MyClientBase client2 = new MyClientBase(binding, endpointAddress);
        client2.Open();
        Assert.NotEqual(client1.ChannelFactory, client2.ChannelFactory);
        client1.Close();
        client2.Close();
        // Validate setting to same value doesn't throw
        MyClientBase.CacheSetting = CacheSetting.Default;
        // Validate instantiated caching throws when changing setting
        Assert.Throws<InvalidOperationException>(() => MyClientBase.CacheSetting = CacheSetting.AlwaysOn);
    }

    [WcfFact]
    public static void ClientBaseChannelFactoryDefaultCachingServiceEndpoint()
    {
        Assert.Equal(CacheSetting.Default, MyClientBase.CacheSetting);
        BasicHttpBinding binding = new BasicHttpBinding();
        EndpointAddress endpointAddress = new EndpointAddress("http://myendpoint");
        ChannelFactory<ITestService> tempFactory = new ChannelFactory<ITestService>(binding, endpointAddress);
        ServiceEndpoint serviceEndpoint = tempFactory.Endpoint;
        MyClientBase client1 = new MyClientBase(serviceEndpoint);
        client1.Open();
        MyClientBase client2 = new MyClientBase(serviceEndpoint);
        client2.Open();
        Assert.NotEqual(client1.ChannelFactory, client2.ChannelFactory);
        client1.Close();
        client2.Close();
        // Validate setting to same value doesn't throw
        MyClientBase.CacheSetting = CacheSetting.Default;
        // Validate instantiated caching throws when changing setting
        Assert.Throws<InvalidOperationException>(() => MyClientBase.CacheSetting = CacheSetting.AlwaysOn);
    }

    [WcfFact]
    public static void ClientBaseChannelFactoryAlwaysOnCachingBindingEndpointAddress()
    {
        MyClientBase2.CacheSetting = CacheSetting.AlwaysOn;
        BasicHttpBinding binding = new BasicHttpBinding();
        EndpointAddress endpointAddress = new EndpointAddress("http://myendpoint");
        MyClientBase2 client1 = new MyClientBase2(binding, endpointAddress);
        client1.Open();
        MyClientBase2 client2 = new MyClientBase2(binding, endpointAddress);
        client2.Open();
        // Validate using the same ChannelFactory when constructor args are the same
        Assert.Equal(client1.ChannelFactory, client2.ChannelFactory);
        MyClientBase2 client3 = new MyClientBase2(binding, new EndpointAddress("http://myendpoint"));
        client3.Open();
        // Validate using the same ChannelFactory when constructor args are equivalent
        Assert.Equal(client1.ChannelFactory, client3.ChannelFactory);
        MyClientBase2 client4 = new MyClientBase2(binding, new EndpointAddress("http://myotherendpoint"));
        // Validate using different ChannelFactory when constructor args are not equivalent
        Assert.NotEqual(client1.ChannelFactory, client4.ChannelFactory);
        client1.Close();
        client2.Close();
        client3.Close();
        client4.Close();
        // Validate setting to same value doesn't throw
        MyClientBase2.CacheSetting = CacheSetting.AlwaysOn;
        // Validate instantiated caching throws when changing setting
        Assert.Throws<InvalidOperationException>(() => MyClientBase2.CacheSetting = CacheSetting.Default);
    }

    [WcfFact]
    public static void ClientBaseChannelFactoryAlwaysOnServiceEndpoint()
    {
        MyClientBase2.CacheSetting = CacheSetting.AlwaysOn;
        BasicHttpBinding binding = new BasicHttpBinding();
        EndpointAddress endpointAddress = new EndpointAddress("http://myendpoint");
        ChannelFactory<ITestService2> tempFactory = new ChannelFactory<ITestService2>(binding, endpointAddress);
        ServiceEndpoint serviceEndpoint = tempFactory.Endpoint;
        MyClientBase2 client1 = new MyClientBase2(serviceEndpoint);
        client1.Open();
        MyClientBase2 client2 = new MyClientBase2(serviceEndpoint);
        client2.Open();
        // Validate using the same ChannelFactory when constructor args are the same
        Assert.Equal(client1.ChannelFactory, client2.ChannelFactory);
        tempFactory = new ChannelFactory<ITestService2>(binding, endpointAddress);
        ServiceEndpoint serviceEndpoint2 = tempFactory.Endpoint;
        MyClientBase2 client3 = new MyClientBase2(serviceEndpoint2);
        client3.Open();
        // Validate using different ChannelFactory when constructor args are not the same
        Assert.NotEqual(client1.ChannelFactory, client3.ChannelFactory);
        var existingChannelFactory = client1.ChannelFactory;
        client1.Close();
        client2.Close();
        client3.Close();
        // Validate that ChannelFactory is replaced if existing one is closed
        existingChannelFactory.Close();
        Assert.Equal(CommunicationState.Closed, existingChannelFactory.State);
        MyClientBase2 client4 = new MyClientBase2(serviceEndpoint);
        client4.Open();
        Assert.NotEqual(existingChannelFactory, client4.ChannelFactory);
        // Validate setting to same value doesn't throw
        MyClientBase2.CacheSetting = CacheSetting.AlwaysOn;
        // Validate instantiated caching throws when changing setting
        Assert.Throws<InvalidOperationException>(() => MyClientBase2.CacheSetting = CacheSetting.Default);
    }


    [WcfFact]
    public static void ClientBaseChannelFactoryAlwaysOffCachingBindingEndpointAddress()
    {
        MyClientBase3.CacheSetting = CacheSetting.AlwaysOff;
        BasicHttpBinding binding = new BasicHttpBinding();
        EndpointAddress endpointAddress = new EndpointAddress("http://myendpoint");
        MyClientBase3 client1 = new MyClientBase3(binding, endpointAddress);
        client1.Open();
        MyClientBase3 client2 = new MyClientBase3(binding, endpointAddress);
        client2.Open();
        Assert.NotEqual(client1.ChannelFactory, client2.ChannelFactory);
        client1.Close();
        client2.Close();
        // Validate setting to same value doesn't throw
        MyClientBase3.CacheSetting = CacheSetting.AlwaysOff;
        // Validate instantiated caching throws when changing setting
        Assert.Throws<InvalidOperationException>(() => MyClientBase.CacheSetting = CacheSetting.AlwaysOn);
    }

    [WcfFact]
    public static void ClientBaseChannelFactoryAlwaysOffCachingServiceEndpoint()
    {
        MyClientBase3.CacheSetting = CacheSetting.AlwaysOff;
        BasicHttpBinding binding = new BasicHttpBinding();
        EndpointAddress endpointAddress = new EndpointAddress("http://myendpoint");
        ChannelFactory<ITestService3> tempFactory = new ChannelFactory<ITestService3>(binding, endpointAddress);
        ServiceEndpoint serviceEndpoint = tempFactory.Endpoint;
        MyClientBase3 client1 = new MyClientBase3(serviceEndpoint);
        client1.Open();
        MyClientBase3 client2 = new MyClientBase3(serviceEndpoint);
        client2.Open();
        Assert.NotEqual(client1.ChannelFactory, client2.ChannelFactory);
        client1.Close();
        client2.Close();
        // Validate setting to same value doesn't throw
        MyClientBase3.CacheSetting = CacheSetting.AlwaysOff;
        // Validate instantiated caching throws when changing setting
        Assert.Throws<InvalidOperationException>(() => MyClientBase.CacheSetting = CacheSetting.AlwaysOn);
    }

    [WcfFact]
    public static async Task ClientBaseDisposeAsyncMethodClosesCorrectly()
    {
        // *** SETUP *** \\
        BasicHttpBinding binding = new BasicHttpBinding();
        MyClientBase client = new MyClientBase(binding, new EndpointAddress("http://myendpoint"));

        // *** VALIDATE *** \\
        Assert.Equal(CommunicationState.Created, client.State);
        client.Open();
        Assert.Equal(CommunicationState.Opened, client.State);
        await((IAsyncDisposable)client).DisposeAsync();
        Assert.Equal(CommunicationState.Closed, client.State);
    }

    [WcfTheory]
    [InlineData(true)]
    [InlineData(false)]
    public static async Task ClientBaseDisposeAsyncNoThrow(bool channelThrows)
    {
        // *** SETUP *** \\
        BasicHttpBinding binding = new BasicHttpBinding();
        var customBinding = new CustomBinding(binding);
        customBinding.Elements.Insert(0, new ThrowingOnCloseBindingElement(new CommunicationException(nameof(ClientBaseDisposeAsyncNoThrow)), channelThrows));
        MyClientBase client = new MyClientBase(customBinding, new EndpointAddress("http://myendpoint"));
        // *** VALIDATE *** \\
        Assert.Equal(CommunicationState.Created, client.State);
        client.Open();
        Assert.Equal(CommunicationState.Opened, client.State);
        await ((IAsyncDisposable)client).DisposeAsync();
        Assert.Equal(CommunicationState.Closed, client.State);
    }

    public class MyClientBase : ClientBase<ITestService>
    {
        public MyClientBase(Binding binding, EndpointAddress endpointAddress)
            : base(binding, endpointAddress)
        {
        }

        public MyClientBase(ServiceEndpoint serviceEndpoint)
            : base(serviceEndpoint)
        {
        }

        public ITestService Proxy
        {
            get { return base.Channel; }
        }
    }

    // 3 ClientBase types are needed as once you set the CacheSetting and instantiated a ClientBase<T>,
    // you can't modify the CacheSetting. We have tests which require a CacheSetting different from default
    // so need a 3 different types to test the 3 CacheSettings modes.
    public class MyClientBase2 : ClientBase<ITestService2>
    {
        public MyClientBase2(Binding binding, EndpointAddress endpointAddress)
            : base(binding, endpointAddress)
        {
        }

        public MyClientBase2(ServiceEndpoint serviceEndpoint)
            : base(serviceEndpoint)
        {
        }
    }

    public class MyClientBase3 : ClientBase<ITestService3>
    {
        public MyClientBase3(Binding binding, EndpointAddress endpointAddress)
            : base(binding, endpointAddress)
        {
        }

        public MyClientBase3(ServiceEndpoint serviceEndpoint)
            : base(serviceEndpoint)
        {
        }
    }

    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        string Echo(string message);
    }

    [ServiceContract]
    public interface ITestService2 : ITestService
    {
    }

    [ServiceContract]
    public interface ITestService3 : ITestService
    {
    }
}
