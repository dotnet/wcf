// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ComponentModel;
using System.Net.Security;
using System.Runtime.Versioning;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.MsmqIntegration;
using Infrastructure.Common;
using Xunit;

public static class MsmqIntegrationBindingTest
{
    [WcfFact]
    public static void MsmqIntegrationBindingElement_Defaults()
    {
        var bel = new MsmqIntegrationBindingElement();
        Assert.Equal("msmq.formatname", bel.Scheme);
        Assert.Equal(MsmqMessageSerializationFormat.Xml, bel.SerializationFormat);
        Assert.Null(bel.TargetSerializationTypes);
    }

    [WcfFact]
    public static void MsmqIntegrationBindingElement_TargetTypes_RoundTrip_DefensiveCopy()
    {
        var bel = new MsmqIntegrationBindingElement();
        var input = new[] { typeof(int), typeof(string) };
        bel.TargetSerializationTypes = input;

        var roundTrip = bel.TargetSerializationTypes;
        Assert.Equal(input, roundTrip);
        Assert.NotSame(input, roundTrip);

        // Mutating the input array after assignment must not leak into the element.
        input[0] = typeof(object);
        Assert.Equal(typeof(int), bel.TargetSerializationTypes[0]);
    }

    [WcfFact]
    public static void MsmqIntegrationBindingElement_Clone_DeepCopy()
    {
        var original = new MsmqIntegrationBindingElement
        {
            SerializationFormat = MsmqMessageSerializationFormat.Binary,
            TargetSerializationTypes = new[] { typeof(DateTime) },
            Durable = false,
        };

        var clone = (MsmqIntegrationBindingElement)original.Clone();
        Assert.Equal(MsmqMessageSerializationFormat.Binary, clone.SerializationFormat);
        Assert.Equal(typeof(DateTime), clone.TargetSerializationTypes[0]);
        Assert.False(clone.Durable);
        Assert.NotSame(original.TargetSerializationTypes, clone.TargetSerializationTypes);
    }

    [WcfFact]
    public static void MsmqIntegrationBindingElement_InvalidFormat_Throws()
    {
        var bel = new MsmqIntegrationBindingElement();
        Assert.Throws<ArgumentOutOfRangeException>(() => bel.SerializationFormat = (MsmqMessageSerializationFormat)42);
    }

    [WcfFact]
    public static void MsmqIntegrationBindingElement_CanBuildChannelFactory()
    {
        var bel = new MsmqIntegrationBindingElement();
        var binding = new CustomBinding(bel);
        var context = new BindingContext(binding, new BindingParameterCollection());
        Assert.True(bel.CanBuildChannelFactory<IOutputChannel>(context));
        Assert.False(bel.CanBuildChannelFactory<IOutputSessionChannel>(context));
        Assert.False(bel.CanBuildChannelFactory<IRequestChannel>(context));
    }

    [WcfFact]
    [SupportedOSPlatform("windows")]
    public static void MsmqIntegrationBindingElement_BuildChannelFactory_ReturnsFactory()
    {
        var bel = new MsmqIntegrationBindingElement();
        var binding = new CustomBinding(bel);
        var context = new BindingContext(binding, new BindingParameterCollection());
        IChannelFactory<IOutputChannel> factory = bel.BuildChannelFactory<IOutputChannel>(context);
        Assert.NotNull(factory);
    }

    [WcfFact]
    public static void MsmqIntegrationBindingElement_GetProperty_MessageVersion()
    {
        var bel = new MsmqIntegrationBindingElement();
        var binding = new CustomBinding(bel);
        var context = new BindingContext(binding, new BindingParameterCollection());
        Assert.Same(MessageVersion.None, bel.GetProperty<MessageVersion>(context));
    }

    [WcfFact]
    public static void MsmqIntegrationBinding_Defaults()
    {
        var b = new MsmqIntegrationBinding();
        Assert.Equal("msmq.formatname", b.Scheme);
        Assert.Equal(MsmqIntegrationSecurityMode.Transport, b.Security.Mode);
        Assert.Equal(MsmqMessageSerializationFormat.Xml, b.SerializationFormat);
        Assert.True(b.Durable);
        Assert.True(b.ExactlyOnce);
    }

    [WcfTheory]
    [InlineData(MsmqIntegrationSecurityMode.None)]
    [InlineData(MsmqIntegrationSecurityMode.Transport)]
    public static void MsmqIntegrationBinding_SecurityModeCtor(MsmqIntegrationSecurityMode mode)
    {
        var b = new MsmqIntegrationBinding(mode);
        Assert.Equal(mode, b.Security.Mode);
    }

    [WcfFact]
    public static void MsmqIntegrationBinding_InvalidSecurityMode_Throws()
    {
        Assert.Throws<InvalidEnumArgumentException>(() => new MsmqIntegrationBinding((MsmqIntegrationSecurityMode)42));
    }

    [WcfFact]
    public static void MsmqIntegrationBinding_CreateBindingElements_TransportOnly()
    {
        var b = new MsmqIntegrationBinding();
        var elements = b.CreateBindingElements();
        Assert.Single(elements);
        Assert.IsType<MsmqIntegrationBindingElement>(elements[0]);
    }

    [WcfFact]
    public static void MsmqIntegrationBinding_SecurityNone_DisablesTransport()
    {
        var b = new MsmqIntegrationBinding(MsmqIntegrationSecurityMode.None);
        var elements = b.CreateBindingElements();
        var transport = (MsmqIntegrationBindingElement)elements.Find<TransportBindingElement>();
        Assert.Equal(MsmqAuthenticationMode.None, transport.MsmqTransportSecurity.MsmqAuthenticationMode);
        Assert.Equal(ProtectionLevel.None, transport.MsmqTransportSecurity.MsmqProtectionLevel);
    }
}
