// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Runtime.Versioning;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.MsmqIntegration;
using Infrastructure.Common;
using Xunit;

[SupportedOSPlatform("windows")]
public static class MsmqIntegrationOutputChannelFactoryTest
{
    private static IChannelFactory<IOutputChannel> BuildFactory()
    {
        var binding = new MsmqIntegrationBinding(MsmqIntegrationSecurityMode.None)
        {
            ExactlyOnce = false,
            Durable = false,
        };
        return binding.BuildChannelFactory<IOutputChannel>();
    }

    [WcfFact]
    public static void MsmqIntegrationBinding_BuildChannelFactory_Succeeds()
    {
        IChannelFactory<IOutputChannel> factory = BuildFactory();
        Assert.NotNull(factory);
        factory.Close();
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
    public static void Factory_CreateChannel_PreservesAddresses()
    {
        IChannelFactory<IOutputChannel> factory = BuildFactory();
        factory.Open();
        var ea = new EndpointAddress("msmq.formatname:DIRECT=OS:.\\private$\\q1");
        IOutputChannel channel = factory.CreateChannel(ea);
        try
        {
            Assert.Same(ea, channel.RemoteAddress);
            Assert.Equal(ea.Uri, channel.Via);
        }
        finally
        {
            channel.Abort();
        }
    }

    [WcfFact]
    public static void Factory_GetProperty_MessageVersion_ReturnsNone()
    {
        IChannelFactory<IOutputChannel> factory = BuildFactory();
        Assert.Same(MessageVersion.None, factory.GetProperty<MessageVersion>());
    }

    [WcfFact]
    public static void Factory_RejectsUnsupportedChannelType()
    {
        var binding = new MsmqIntegrationBinding(MsmqIntegrationSecurityMode.None);
        Assert.Throws<ArgumentException>(() => binding.BuildChannelFactory<IOutputSessionChannel>());
    }
}
