// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Runtime.Versioning;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

[SupportedOSPlatform("windows")]
public static class MsmqOutputChannelFactoryTest
{
    private static IChannelFactory<IOutputChannel> BuildFactory()
    {
        var binding = new NetMsmqBinding(NetMsmqSecurityMode.None) { ExactlyOnce = false, Durable = false };
        return binding.BuildChannelFactory<IOutputChannel>();
    }

    [WcfFact]
    public static void NetMsmqBinding_BuildChannelFactory_Succeeds()
    {
        IChannelFactory<IOutputChannel> factory = BuildFactory();
        Assert.NotNull(factory);
    }

    [WcfFact]
    public static void Factory_OpenClose_Lifecycle()
    {
        IChannelFactory<IOutputChannel> factory = BuildFactory();
        factory.Open();
        Assert.Equal(CommunicationState.Opened, factory.State);
        factory.Close();
        Assert.Equal(CommunicationState.Closed, factory.State);
    }

    [WcfFact]
    public static void Factory_CreateChannel_ValidatesAddress()
    {
        IChannelFactory<IOutputChannel> factory = BuildFactory();
        factory.Open();
        Assert.Throws<ArgumentNullException>(() => factory.CreateChannel(null));
    }

    [WcfFact]
    public static void Factory_CreateChannel_ReturnsChannelWithExpectedAddresses()
    {
        IChannelFactory<IOutputChannel> factory = BuildFactory();
        factory.Open();
        var ea = new EndpointAddress("net.msmq://localhost/private/scratch");
        IOutputChannel channel = factory.CreateChannel(ea);
        try
        {
            Assert.Same(ea, channel.RemoteAddress);
            Assert.Equal(new Uri("net.msmq://localhost/private/scratch"), channel.Via);
        }
        finally
        {
            channel.Abort();
        }
    }

    [WcfFact]
    public static void Factory_CreateChannel_RejectsUnsupportedScheme()
    {
        IChannelFactory<IOutputChannel> factory = BuildFactory();
        factory.Open();
        var ea = new EndpointAddress("http://localhost/svc");
        Assert.Throws<ArgumentException>(() => factory.CreateChannel(ea));
    }

    [WcfFact]
    public static void Factory_GetProperty_MessageVersion_ReturnsSoap12()
    {
        IChannelFactory<IOutputChannel> factory = BuildFactory();
        MessageVersion mv = factory.GetProperty<MessageVersion>();
        Assert.Equal(MessageVersion.Soap12WSAddressing10, mv);
    }
}
