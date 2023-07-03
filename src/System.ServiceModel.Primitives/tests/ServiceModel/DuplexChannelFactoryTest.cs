// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Infrastructure.Common;
using Xunit;

public class DuplexChannelFactoryTest
{
    [WcfFact]
    public static void CreateChannel_EndpointAddress_Null_Throws()
    {
        WcfDuplexServiceCallback callback = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callback);
        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.None;
        EndpointAddress remoteAddress = null;

        DuplexChannelFactory<IWcfDuplexService> factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, remoteAddress);
        try
        {
            Assert.Throws<InvalidOperationException>(() =>
               {
                   factory.Open();
                   factory.CreateChannel();
               });
        }
        finally
        {
            factory.Abort();
        }
    }

    [WcfFact]
    public static void CreateChannel_InvalidEndpointAddress_AsString_ThrowsUriFormat()
    {
        WcfDuplexServiceCallback callback = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callback);
        Binding binding = new NetTcpBinding();
        Assert.Throws<UriFormatException>(() =>
        {
            DuplexChannelFactory<IWcfDuplexService> factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, "invalid");
        });
    }

    [WcfFact]
    public static void CreateChannel_EmptyEndpointAddress_AsString_ThrowsUriFormat()
    {
        WcfDuplexServiceCallback callback = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callback);
        Binding binding = new NetTcpBinding();
        Assert.Throws<UriFormatException>(() =>
        {
            DuplexChannelFactory<IWcfDuplexService> factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, string.Empty);
        });
    }

    [WcfFact]
    // valid address, but the scheme is incorrect
    public static void CreateChannel_ExpectedNetTcpScheme_HttpScheme_ThrowsUriFormat()
    {
        WcfDuplexServiceCallback callback = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callback);
        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.None;
        Assert.Throws<ArgumentException>("via", () =>
        {
            DuplexChannelFactory<IWcfDuplexService> factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, "http://not-the-right-scheme");
            factory.CreateChannel();
        });
    }

    [WcfFact]
    // valid address, but the scheme is incorrect
    public static void CreateChannel_ExpectedNetTcpScheme_InvalidScheme_ThrowsUriFormat()
    {
        WcfDuplexServiceCallback callback = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callback);
        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.None;
        Assert.Throws<ArgumentException>("via", () =>
        {
            DuplexChannelFactory<IWcfDuplexService> factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, "qwerty://not-the-right-scheme");
            factory.CreateChannel();
        });
    }

    [WcfFact]
    // valid address, but the scheme is incorrect
    public static void CreateChannel_BasicHttpBinding_Throws_InvalidOperation()
    {
        WcfDuplexServiceCallback callback = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callback);
        Binding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);

        // Contract requires Duplex, but Binding 'BasicHttpBinding' doesn't support it or isn't configured properly to support it.
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            DuplexChannelFactory<IWcfDuplexService> factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, "http://basichttp-not-duplex");
            factory.CreateChannel();
        });

        Assert.True(exception.Message.Contains("BasicHttpBinding"),
            string.Format("InvalidOperationException exception string should contain 'BasicHttpBinding'. Actual message:\r\n" + exception.ToString()));
    }

    [WcfFact]
    public static void CreateChannel_Address_NullString_ThrowsArgumentNull()
    {
        WcfDuplexServiceCallback callback = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callback);
        Binding binding = new NetTcpBinding();
        Assert.Throws<ArgumentNullException>("uri", () =>
        {
            DuplexChannelFactory<IWcfDuplexService> factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, (string)null);
        });
    }

    [WcfFact]
    public static void CreateChannel_Address_NullEndpointAddress_ThrowsArgumentNull()
    {
        WcfDuplexServiceCallback callback = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callback);
        Binding binding = new NetTcpBinding();
        DuplexChannelFactory<IWcfDuplexService> factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, (EndpointAddress)null);

        Assert.Throws<InvalidOperationException>(() =>
        {
            factory.CreateChannel();
        });
    }

    [WcfFact]
    public static void CreateChannel_Using_NetTcpBinding_Defaults()
    {
        WcfDuplexServiceCallback callback = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callback);
        Binding binding = new NetTcpBinding();
        EndpointAddress endpoint = new EndpointAddress("net.tcp://not-an-endpoint");

        DuplexChannelFactory<IWcfDuplexService> factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, endpoint);
        IWcfDuplexService proxy = factory.CreateChannel();
        Assert.NotNull(proxy);
    }

    [WcfFact]
    public static void Ctor_Type_Overloads_Can_CreateChannel()
    {        
        Binding binding = new NetTcpBinding();
        string remoteAddress = "net.tcp://not-an-endpoint";
        EndpointAddress endpoint = new EndpointAddress(remoteAddress);
        ServiceEndpoint serviceEndpoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(IWcfDuplexService)), binding, endpoint);
        WcfDuplexServiceCallback callback = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callback);        

        DuplexChannelFactory<IWcfDuplexService> factory = new DuplexChannelFactory<IWcfDuplexService>(typeof(WcfDuplexServiceCallback), binding, endpoint);
        IWcfDuplexService proxy = factory.CreateChannel(context);
        Assert.NotNull(proxy);

        factory = new DuplexChannelFactory<IWcfDuplexService>(typeof(WcfDuplexServiceCallback), binding, remoteAddress);
        proxy = factory.CreateChannel(context);
        Assert.NotNull(proxy);

        factory = new DuplexChannelFactory<IWcfDuplexService>(typeof(WcfDuplexServiceCallback), binding);
        proxy = factory.CreateChannel(context, endpoint);
        Assert.NotNull(proxy);

        factory = new DuplexChannelFactory<IWcfDuplexService>(typeof(WcfDuplexServiceCallback));
        factory.Endpoint.Binding = binding;
        factory.Endpoint.Address = endpoint;
        proxy = factory.CreateChannel(context);
        Assert.NotNull(proxy);

        factory = new DuplexChannelFactory<IWcfDuplexService>(typeof(WcfDuplexServiceCallback), serviceEndpoint);
        proxy = factory.CreateChannel(context);
        Assert.NotNull(proxy);
    }

    [WcfFact]
    public static void CreateChannel_Using_NetTcp_NoSecurity()
    {
        WcfDuplexServiceCallback callback = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callback);
        Binding binding = new NetTcpBinding(SecurityMode.None);
        EndpointAddress endpoint = new EndpointAddress("net.tcp://not-an-endpoint");
        DuplexChannelFactory<IWcfDuplexService> factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, endpoint);
        factory.CreateChannel();
    }

    [WcfFact]
    public static void CreateChannel_Using_Http_NoSecurity()
    {
        WcfDuplexServiceCallback callback = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callback);
        Binding binding = new NetHttpBinding(BasicHttpSecurityMode.None);
        EndpointAddress endpoint = new EndpointAddress(FakeAddress.HttpAddress);
        DuplexChannelFactory<IWcfDuplexService> factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, endpoint);
        factory.CreateChannel();
    }

    [WcfFact]
    public static void CreateChannel_Of_IDuplexChannel_Using_NetTcpBinding_Creates_Unique_Instances()
    {
        DuplexChannelFactory<IWcfDuplexService> factory = null;
        DuplexChannelFactory<IWcfDuplexService> factory2 = null;
        IWcfDuplexService channel = null;
        IWcfDuplexService channel2 = null;

        WcfDuplexServiceCallback callback = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callback);

        try
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            EndpointAddress endpointAddress = new EndpointAddress(FakeAddress.TcpAddress);

            // Create the channel factory for the request-reply message exchange pattern.
            factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, endpointAddress);
            factory2 = new DuplexChannelFactory<IWcfDuplexService>(context, binding, endpointAddress);

            // Create the channel.
            channel = factory.CreateChannel();
            Assert.True(typeof(IWcfDuplexService).GetTypeInfo().IsAssignableFrom(channel.GetType().GetTypeInfo()),
                String.Format("Channel type '{0}' was not assignable to '{1}'", channel.GetType(), typeof(IDuplexChannel)));

            channel2 = factory2.CreateChannel();
            Assert.True(typeof(IWcfDuplexService).GetTypeInfo().IsAssignableFrom(channel2.GetType().GetTypeInfo()),
                String.Format("Channel type '{0}' was not assignable to '{1}'", channel2.GetType(), typeof(IDuplexChannel)));

            // Validate ToString()
            string toStringResult = channel.ToString();
            string toStringExpected = "IWcfDuplexService";
            Assert.Equal(toStringExpected, toStringResult);

            // Validate Equals()
            Assert.StrictEqual<IWcfDuplexService>(channel, channel);

            // Validate Equals(other channel) negative
            Assert.NotStrictEqual<IWcfDuplexService>(channel, channel2);

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
}
