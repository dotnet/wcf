// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Xunit;

public class ChannelFactoryTest
{
    [Fact]
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
            Assert.Equal<string>(toStringExpected, toStringResult);

            // Validate Equals()
            Assert.StrictEqual<IRequestChannel>(channel, channel);

            // Validate Equals(other channel) negative
            Assert.NotStrictEqual<IRequestChannel>(channel, channel2);

            // Validate Equals("other") negative
            Assert.NotStrictEqual<object>(channel, "other");

            // Validate Equals(null) negative
            Assert.NotStrictEqual<IRequestChannel>(channel, null);
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

    [Fact]
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
            Assert.Equal<string>(toStringExpected, toStringResult);

            // Validate Equals()
            Assert.StrictEqual<IRequestChannel>(channel, channel);

            // Validate Equals(other channel) negative
            Assert.NotStrictEqual<IRequestChannel>(channel, channel2);

            // Validate Equals("other") negative
            Assert.NotStrictEqual<object>(channel, "other");

            // Validate Equals(null) negative
            Assert.NotStrictEqual<IRequestChannel>(channel, null);
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

    [Fact]
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
            Assert.Equal<string>(toStringExpected, toStringResult);

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

    [Fact]
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
}
