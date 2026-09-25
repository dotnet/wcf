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
public static class MsmqOutputSessionChannelFactoryTest
{
    private static IChannelFactory<IOutputSessionChannel> BuildFactory()
    {
        var binding = new NetMsmqBinding(NetMsmqSecurityMode.None) { ExactlyOnce = false, Durable = false };
        return binding.BuildChannelFactory<IOutputSessionChannel>();
    }

    [WcfFact]
    public static void NetMsmqBinding_BuildSessionChannelFactory_Succeeds()
    {
        IChannelFactory<IOutputSessionChannel> factory = BuildFactory();
        Assert.NotNull(factory);
        factory.Close();
    }

    [WcfFact]
    public static void Factory_CreateChannel_HasUniqueSessionId()
    {
        IChannelFactory<IOutputSessionChannel> factory = BuildFactory();
        factory.Open();
        var ea = new EndpointAddress("net.msmq://localhost/private/scratch");
        IOutputSessionChannel c1 = factory.CreateChannel(ea);
        IOutputSessionChannel c2 = factory.CreateChannel(ea);
        try
        {
            Assert.NotNull(c1.Session);
            Assert.NotNull(c1.Session.Id);
            Assert.NotEmpty(c1.Session.Id);
            Assert.NotEqual(c1.Session.Id, c2.Session.Id);
            Assert.StartsWith("uuid:", c1.Session.Id);
        }
        finally
        {
            c1.Abort();
            c2.Abort();
        }
    }

    [WcfFact]
    public static void Factory_CreateChannel_PreservesAddresses()
    {
        IChannelFactory<IOutputSessionChannel> factory = BuildFactory();
        factory.Open();
        var ea = new EndpointAddress("net.msmq://localhost/private/scratch");
        IOutputSessionChannel channel = factory.CreateChannel(ea);
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
    public static void Factory_GetProperty_MessageVersion()
    {
        IChannelFactory<IOutputSessionChannel> factory = BuildFactory();
        Assert.Equal(MessageVersion.Soap12WSAddressing10, factory.GetProperty<MessageVersion>());
    }
}
