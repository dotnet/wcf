// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public class ChannelFactoryTest
{
    [WcfFact]
    public static void CreateChannel_Of_IRequestChannel_Using_CustomBinding()
    {
        ChannelFactory<IRequestChannel> factory = null;
        ChannelFactory<IRequestChannel> factory2 = null;
        IRequestChannel channel = null;
        IRequestChannel channel2 = null;

        try
        {
            CustomBinding binding = new CustomBinding(new BindingElement[] {
                new TextMessageEncodingBindingElement(MessageVersion.Default, Encoding.UTF8),
                new HttpTransportBindingElement() });

            EndpointAddress endpointAddress = new EndpointAddress(FakeAddress.HttpAddress);

            // Create the channel factory for the request-reply message exchange pattern.
            factory = new ChannelFactory<IRequestChannel>(binding, endpointAddress);
            factory2 = new ChannelFactory<IRequestChannel>(binding, endpointAddress);

            // Create the channel.
            channel = factory.CreateChannel();
            Assert.True(typeof(IRequestChannel).GetTypeInfo().IsAssignableFrom(channel.GetType().GetTypeInfo()),
                String.Format("Channel type '{0}' was not assignable to '{1}'", channel.GetType(), typeof(IRequestChannel)));

            channel2 = factory2.CreateChannel();
            Assert.True(typeof(IRequestChannel).GetTypeInfo().IsAssignableFrom(channel2.GetType().GetTypeInfo()),
                String.Format("Channel type '{0}' was not assignable to '{1}'", channel2.GetType(), typeof(IRequestChannel)));

            // Validate ToString()
            string toStringResult = channel.ToString();
            string toStringExpected = "System.ServiceModel.Channels.IRequestChannel";
            Assert.Equal(toStringExpected, toStringResult);

            // Validate Equals()
            Assert.StrictEqual<IRequestChannel>(channel, channel);

            // Validate Equals(other channel) negative
            Assert.NotStrictEqual<IRequestChannel>(channel, channel2);

            // Validate Equals("other") negative
            Assert.NotStrictEqual<object>("other", channel);

            // Validate Equals(null) negative
            Assert.NotNull(channel);
        }
        finally
        {
            if (factory != null)
            {
                factory.Close();
            }
            if (factory2 != null)
            {
                factory2.Close();
            }
        }
    }

    [WcfFact]
    public static void CreateChannel_Of_IRequestChannel_Using_BasicHttpBinding_Creates_Unique_Instances()
    {
        ChannelFactory<IRequestChannel> factory = null;
        ChannelFactory<IRequestChannel> factory2 = null;
        IRequestChannel channel = null;
        IRequestChannel channel2 = null;

        try
        {
            BasicHttpBinding binding = new BasicHttpBinding();

            EndpointAddress endpointAddress = new EndpointAddress(FakeAddress.HttpAddress);

            // Create the channel factory for the request-reply message exchange pattern.
            factory = new ChannelFactory<IRequestChannel>(binding, endpointAddress);
            factory2 = new ChannelFactory<IRequestChannel>(binding, endpointAddress);

            // Create the channel.
            channel = factory.CreateChannel();
            Assert.True(typeof(IRequestChannel).GetTypeInfo().IsAssignableFrom(channel.GetType().GetTypeInfo()),
                String.Format("Channel type '{0}' was not assignable to '{1}'", channel.GetType(), typeof(IRequestChannel)));

            channel2 = factory2.CreateChannel();
            Assert.True(typeof(IRequestChannel).GetTypeInfo().IsAssignableFrom(channel2.GetType().GetTypeInfo()),
                String.Format("Channel type '{0}' was not assignable to '{1}'", channel2.GetType(), typeof(IRequestChannel)));

            // Validate ToString()
            string toStringResult = channel.ToString();
            string toStringExpected = "System.ServiceModel.Channels.IRequestChannel";
            Assert.Equal(toStringExpected, toStringResult);

            // Validate Equals()
            Assert.StrictEqual<IRequestChannel>(channel, channel);

            // Validate Equals(other channel) negative
            Assert.NotStrictEqual<IRequestChannel>(channel, channel2);

            // Validate Equals("other") negative
            Assert.NotStrictEqual<object>("other", channel);

            // Validate Equals(null) negative
            Assert.NotNull(channel);
        }
        finally
        {
            if (factory != null)
            {
                factory.Close();
            }
            if (factory2 != null)
            {
                factory2.Close();
            }
        }
    }

    [WcfFact]
    public static void ChannelFactory_Verify_CommunicationStates()
    {
        ChannelFactory<IRequestChannel> factory = null;
        IRequestChannel channel = null;

        try
        {
            BasicHttpBinding binding = new BasicHttpBinding();

            EndpointAddress endpointAddress = new EndpointAddress(FakeAddress.HttpAddress);

            // Create the channel factory for the request-reply message exchange pattern.
            factory = new ChannelFactory<IRequestChannel>(binding, endpointAddress);

            Assert.Equal(CommunicationState.Created, factory.State);

            // Create the channel.
            channel = factory.CreateChannel();
            Assert.True(typeof(IRequestChannel).GetTypeInfo().IsAssignableFrom(channel.GetType().GetTypeInfo()),
                String.Format("Channel type '{0}' was not assignable to '{1}'", channel.GetType(), typeof(IRequestChannel)));
            Assert.Equal(CommunicationState.Opened, factory.State);

            // Validate ToString()
            string toStringResult = channel.ToString();
            string toStringExpected = "System.ServiceModel.Channels.IRequestChannel";
            Assert.Equal(toStringExpected, toStringResult);

            factory.Close();
            Assert.Equal(CommunicationState.Closed, factory.State);
        }
        finally
        {
            if (factory != null)
            {
                // check that there are no effects after calling Abort after Close
                factory.Abort();
                Assert.Equal(CommunicationState.Closed, factory.State);

                // check that there are no effects after calling Close again
                factory.Close();
                Assert.Equal(CommunicationState.Closed, factory.State);
            }
        }
    }

    [WcfFact]
    // Create the channel factory using BasicHttpBinding and open the channel using a user generated interface
    public static void CreateChannel_Of_Typed_Proxy_Using_BasicHttpBinding()
    {
        ChannelFactory<IWcfServiceGenerated> factory = null;
        try
        {
            BasicHttpBinding binding = new BasicHttpBinding();

            // Create the channel factory
            factory = new ChannelFactory<IWcfServiceGenerated>(binding, new EndpointAddress(FakeAddress.HttpAddress));

            factory.Open();

            // Create the channel.
            IWcfServiceGenerated channel = factory.CreateChannel();

            Assert.True(typeof(IWcfServiceGenerated).GetTypeInfo().IsAssignableFrom(channel.GetType().GetTypeInfo()),
                String.Format("Channel type '{0}' was not assignable to '{1}'", channel.GetType(), typeof(IRequestChannel)));
        }
        finally
        {
            if (factory != null)
            {
                factory.Close();
            }
        }
    }

    [WcfFact]
    public static void ChannelFactory_Async_Open_Close()
    {
        ChannelFactory<IRequestChannel> factory = null;
        try
        {
            BasicHttpBinding binding = new BasicHttpBinding();

            // Create the channel factory
            factory = new ChannelFactory<IRequestChannel>(binding, new EndpointAddress(FakeAddress.HttpAddress));
            Assert.True(CommunicationState.Created == factory.State,
                string.Format("factory.State - Expected: {0}, Actual: {1}.", CommunicationState.Created, factory.State));

            Task.Factory.FromAsync(factory.BeginOpen(null, null), factory.EndOpen).GetAwaiter().GetResult();
            Assert.True(CommunicationState.Opened == factory.State,
                string.Format("factory.State - Expected: {0}, Actual: {1}.", CommunicationState.Opened, factory.State));

            Task.Factory.FromAsync(factory.BeginClose(null, null), factory.EndClose).GetAwaiter().GetResult();
            Assert.True(CommunicationState.Closed == factory.State,
                string.Format("factory.State - Expected: {0}, Actual: {1}.", CommunicationState.Closed, factory.State));
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }
    }

    [WcfTheory]
    [InlineData(true)]
    [InlineData(false)]
    public static void ChannelFactory_AllowCookies(bool allowCookies)
    {
        ChannelFactory<IWcfService> factory = null;

        try
        {
            factory = new ChannelFactory<IWcfService>(
                     new BasicHttpBinding()
                     {
                         AllowCookies = allowCookies
                     },
                     new EndpointAddress(FakeAddress.HttpAddress));

            IWcfService serviceProxy = factory.CreateChannel();

            IHttpCookieContainerManager cookieManager = ((IChannel)serviceProxy).GetProperty<IHttpCookieContainerManager>();
            Assert.True(allowCookies == (cookieManager != null),
                string.Format("AllowCookies was '{0}', 'cookieManager != null' was expected to be '{0}', but it was '{1}'.", allowCookies, cookieManager != null));

            if (allowCookies)
            {
                Assert.True(allowCookies == (cookieManager.CookieContainer != null),
                    string.Format("AllowCookies was '{0}', 'cookieManager.CookieContainer != null' was expected to be '{0}', but it was '{1}'.", allowCookies, cookieManager != null));
            }
        }
        finally
        {
            if (factory != null)
            {
                factory.Close();
            }
        }
    }

    [Fact]
    public static async Task ChannelFactory_AsyncDisposable()
    {
        ChannelFactory<IWcfService> factory = null;

        try
        {
            factory = new ChannelFactory<IWcfService>(
                     new BasicHttpBinding(),
                     new EndpointAddress(FakeAddress.HttpAddress));
            factory.Open();

            await ((IAsyncDisposable)factory).DisposeAsync();
            Assert.Equal(CommunicationState.Closed, factory.State);
            factory = null;
        }
        finally
        {
            if (factory != null)
            {
                factory.Close();
            }
        }
    }

    [Fact]
    public static async Task ChannelFactory_AsyncDisposable_NoThrow()
    {
        ChannelFactory<IWcfService> factory = null;

        try
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            var customBinding = new CustomBinding(binding);
            customBinding.Elements.Insert(0, new ThrowingOnCloseBindingElement(new CommunicationException(nameof(ChannelFactory_AsyncDisposable_NoThrow)), false));

            factory = new ChannelFactory<IWcfService>(
                     customBinding,
                     new EndpointAddress(FakeAddress.HttpAddress));
            factory.Open();

            await ((IAsyncDisposable)factory).DisposeAsync();
            Assert.Equal(CommunicationState.Closed, factory.State);
            factory = null;
        }
        finally
        {
            if (factory != null)
            {
                factory.Close();
            }
        }
    }
}
