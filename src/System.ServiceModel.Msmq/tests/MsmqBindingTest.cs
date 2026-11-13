// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ComponentModel;
using System.Net.Security;
using System.Runtime.Versioning;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

[SupportedOSPlatform("windows")]
public static class MsmqBindingTest
{
    [WcfFact]
    public static void MsmqTransportBindingElement_Defaults()
    {
        var bel = new MsmqTransportBindingElement();
        Assert.Equal("net.msmq", bel.Scheme);
        Assert.Equal(MsmqAuthenticationMode.WindowsDomain, bel.MsmqTransportSecurity.MsmqAuthenticationMode);
        Assert.True(bel.Durable);
        Assert.True(bel.ExactlyOnce);
        Assert.Equal(DeadLetterQueue.System, bel.DeadLetterQueue);
        Assert.Equal(QueueTransferProtocol.Native, bel.QueueTransferProtocol);
        Assert.False(bel.UseActiveDirectory);
        Assert.False(bel.UseMsmqTracing);
        Assert.False(bel.UseSourceJournal);
    }

    [WcfFact]
    public static void MsmqTransportBindingElement_Clone_DeepCopy()
    {
        var original = new MsmqTransportBindingElement
        {
            Durable = false,
            ExactlyOnce = false,
            DeadLetterQueue = DeadLetterQueue.Custom,
            CustomDeadLetterQueue = new Uri("net.msmq://localhost/private/dlq"),
            QueueTransferProtocol = QueueTransferProtocol.Srmp,
            UseActiveDirectory = true,
            TimeToLive = TimeSpan.FromMinutes(30),
        };
        original.MsmqTransportSecurity.MsmqAuthenticationMode = MsmqAuthenticationMode.Certificate;

        var clone = (MsmqTransportBindingElement)original.Clone();
        Assert.NotSame(original, clone);
        Assert.NotSame(original.MsmqTransportSecurity, clone.MsmqTransportSecurity);
        Assert.False(clone.Durable);
        Assert.False(clone.ExactlyOnce);
        Assert.Equal(DeadLetterQueue.Custom, clone.DeadLetterQueue);
        Assert.Equal(original.CustomDeadLetterQueue, clone.CustomDeadLetterQueue);
        Assert.Equal(QueueTransferProtocol.Srmp, clone.QueueTransferProtocol);
        Assert.True(clone.UseActiveDirectory);
        Assert.Equal(TimeSpan.FromMinutes(30), clone.TimeToLive);
        Assert.Equal(MsmqAuthenticationMode.Certificate, clone.MsmqTransportSecurity.MsmqAuthenticationMode);
    }

    [WcfFact]
    public static void MsmqTransportBindingElement_CanBuildChannelFactory()
    {
        var bel = new MsmqTransportBindingElement();
        var binding = new CustomBinding(new BinaryMessageEncodingBindingElement(), bel);
        var context = new BindingContext(binding, new BindingParameterCollection());
        Assert.True(bel.CanBuildChannelFactory<IOutputChannel>(context));
        Assert.True(bel.CanBuildChannelFactory<IOutputSessionChannel>(context));
        Assert.False(bel.CanBuildChannelFactory<IRequestChannel>(context));
        Assert.False(bel.CanBuildChannelFactory<IDuplexChannel>(context));
    }

    [WcfFact]
    public static void MsmqTransportBindingElement_BuildChannelFactory_ReturnsFactory()
    {
        var bel = new MsmqTransportBindingElement();
        var binding = new CustomBinding(new BinaryMessageEncodingBindingElement(), bel);
        var context = new BindingContext(binding, new BindingParameterCollection());
        IChannelFactory<IOutputChannel> factory = bel.BuildChannelFactory<IOutputChannel>(context);
        Assert.NotNull(factory);
    }

    [WcfFact]
    public static void MsmqTransportBindingElement_BuildChannelFactory_UnsupportedType_Throws()
    {
        var bel = new MsmqTransportBindingElement();
        var binding = new CustomBinding(new BinaryMessageEncodingBindingElement(), bel);
        var context = new BindingContext(binding, new BindingParameterCollection());
        Assert.Throws<ArgumentException>(() => bel.BuildChannelFactory<IRequestChannel>(context));
    }

    [WcfTheory]
    [InlineData(-1)]
    public static void MsmqTransportBindingElement_NegativeMaxPoolSize_Throws(int bad)
    {
        var bel = new MsmqTransportBindingElement();
        Assert.Throws<ArgumentOutOfRangeException>(() => bel.MaxPoolSize = bad);
    }

    [WcfFact]
    public static void MsmqTransportBindingElement_NegativeTimeToLive_Throws()
    {
        var bel = new MsmqTransportBindingElement();
        Assert.Throws<ArgumentOutOfRangeException>(() => bel.TimeToLive = TimeSpan.FromSeconds(-1));
    }

    [WcfFact]
    public static void NetMsmqBinding_Defaults()
    {
        var b = new NetMsmqBinding();
        Assert.Equal("net.msmq", b.Scheme);
        Assert.Equal(EnvelopeVersion.Soap12, b.EnvelopeVersion);
        Assert.Equal(NetMsmqSecurityMode.Transport, b.Security.Mode);
        Assert.True(b.Durable);
        Assert.True(b.ExactlyOnce);
        Assert.Equal(QueueTransferProtocol.Native, b.QueueTransferProtocol);
        Assert.False(b.UseActiveDirectory);
    }

    [WcfTheory]
    [InlineData(NetMsmqSecurityMode.None)]
    [InlineData(NetMsmqSecurityMode.Transport)]
    [InlineData(NetMsmqSecurityMode.Message)]
    [InlineData(NetMsmqSecurityMode.Both)]
    public static void NetMsmqBinding_SecurityModeCtor(NetMsmqSecurityMode mode)
    {
        var b = new NetMsmqBinding(mode);
        Assert.Equal(mode, b.Security.Mode);
    }

    [WcfFact]
    public static void NetMsmqBinding_InvalidSecurityMode_Throws()
    {
        Assert.Throws<InvalidEnumArgumentException>(() => new NetMsmqBinding((NetMsmqSecurityMode)42));
    }

    [WcfFact]
    public static void NetMsmqBinding_CreateBindingElements_HasEncodingAndTransport()
    {
        var b = new NetMsmqBinding();
        var elements = b.CreateBindingElements();
        Assert.Contains(elements, e => e is BinaryMessageEncodingBindingElement);
        Assert.Contains(elements, e => e is MsmqTransportBindingElement);
        Assert.Equal(2, elements.Count);
    }

    [WcfFact]
    public static void NetMsmqBinding_TransportSecurityModeAppliesToTransportElement()
    {
        var b = new NetMsmqBinding(NetMsmqSecurityMode.None);
        var elements = b.CreateBindingElements();
        var transport = (MsmqTransportBindingElement)elements.Find<TransportBindingElement>();
        // SecurityMode.None disables transport security
        Assert.Equal(MsmqAuthenticationMode.None, transport.MsmqTransportSecurity.MsmqAuthenticationMode);
        Assert.Equal(ProtectionLevel.None, transport.MsmqTransportSecurity.MsmqProtectionLevel);
    }

    [WcfFact]
    public static void NetMsmqBinding_TransportMode_KeepsTransportSecurityEnabled()
    {
        var b = new NetMsmqBinding(NetMsmqSecurityMode.Transport);
        var elements = b.CreateBindingElements();
        var transport = (MsmqTransportBindingElement)elements.Find<TransportBindingElement>();
        Assert.Equal(MsmqAuthenticationMode.WindowsDomain, transport.MsmqTransportSecurity.MsmqAuthenticationMode);
        Assert.Equal(ProtectionLevel.Sign, transport.MsmqTransportSecurity.MsmqProtectionLevel);
    }

    [WcfFact]
    public static void NetMsmqBinding_ReaderQuotas_NullThrows()
    {
        var b = new NetMsmqBinding();
        Assert.Throws<ArgumentNullException>(() => b.ReaderQuotas = null);
    }
}
