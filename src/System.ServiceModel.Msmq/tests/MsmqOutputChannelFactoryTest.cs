// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Reflection;
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

    // MsmqBindingElementBase.AddressTranslator is internal; reach it through
    // reflection rather than widening the shipping surface for tests.
    private static string TranslateAddress(MsmqBindingElementBase bindingElement, string uri)
    {
        object translator = GetAddressTranslator(bindingElement);
        MethodInfo toFormatName = translator.GetType().GetMethod(
            "UriToFormatName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        return (string)toFormatName.Invoke(translator, new object[] { new Uri(uri) });
    }

    private static object GetAddressTranslator(MsmqBindingElementBase bindingElement)
    {
        PropertyInfo property = typeof(MsmqBindingElementBase).GetProperty(
            "AddressTranslator", BindingFlags.Instance | BindingFlags.NonPublic);
        try
        {
            return property.GetValue(bindingElement);
        }
        catch (TargetInvocationException ex)
        {
            throw ex.InnerException;
        }
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

    // Address translation is deferred to Open so that CreateChannel always
    // hands back a channel object, matching the CommunicationObject contract
    // that callers rely on when they wrap Open in a try/catch.
    [WcfFact]
    public static void Factory_CreateChannel_DefersSchemeValidationToOpen()
    {
        IChannelFactory<IOutputChannel> factory = BuildFactory();
        factory.Open();
        var ea = new EndpointAddress("http://localhost/svc");
        IOutputChannel channel = factory.CreateChannel(ea);
        try
        {
            Assert.NotNull(channel);
            Assert.Throws<ArgumentException>(() => channel.Open());
        }
        finally
        {
            channel.Abort();
        }
    }

    [WcfFact]
    public static void Factory_AddressTranslator_IsWiredForNativeProtocol()
    {
        var bindingElement = new MsmqTransportBindingElement();
        Assert.Equal(QueueTransferProtocol.Native, bindingElement.QueueTransferProtocol);
        Assert.Equal(
            "DIRECT=OS:.\\private$\\q1",
            TranslateAddress(bindingElement, "net.msmq://localhost/private/q1"));
    }

    [WcfTheory]
    [InlineData(QueueTransferProtocol.Srmp, "DIRECT=http://server/msmq/private$/q1")]
    [InlineData(QueueTransferProtocol.SrmpSecure, "DIRECT=https://server/msmq/private$/q1")]
    public static void Factory_AddressTranslator_HonoursQueueTransferProtocol(
        QueueTransferProtocol protocol, string expected)
    {
        var bindingElement = new MsmqTransportBindingElement { QueueTransferProtocol = protocol };
        Assert.Equal(expected, TranslateAddress(bindingElement, "net.msmq://server/private/q1"));
    }

    // UseActiveDirectory used to be silently ignored, which sent the message to
    // a DIRECT= format name the caller never asked for.
    [WcfFact]
    public static void Factory_UseActiveDirectory_IsRejectedRatherThanIgnored()
    {
        var bindingElement = new MsmqTransportBindingElement { UseActiveDirectory = true };
        Assert.Throws<NotSupportedException>(() => GetAddressTranslator(bindingElement));
    }

    [WcfFact]
    public static void Factory_GetProperty_MessageVersion_ReturnsSoap12()
    {
        IChannelFactory<IOutputChannel> factory = BuildFactory();
        MessageVersion mv = factory.GetProperty<MessageVersion>();
        Assert.Equal(MessageVersion.Soap12WSAddressing10, mv);
    }

    [WcfFact]
    public static void Factory_NullBindingContext_ThrowsArgumentNullException()
    {
        var bindingElement = new MsmqTransportBindingElement();
        Assert.Throws<ArgumentNullException>(() => bindingElement.BuildChannelFactory<IOutputChannel>(null));
    }
}
