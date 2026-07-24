// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Runtime.Versioning;
using System.ServiceModel.MsmqIntegration;
using Infrastructure.Common;
using Xunit;

[SupportedOSPlatform("windows")]
public static class MsmqMessageTest
{
    [WcfFact]
    public static void MsmqMessage_Ctor_NullBody_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new MsmqMessage<string>(null));
    }

    [WcfFact]
    public static void MsmqMessage_BodyAndProperties_RoundTrip()
    {
        var m = new MsmqMessage<string>("hello")
        {
            Label = "demo-label",
            CorrelationId = "abcd",
            AppSpecific = 42,
            Priority = MessagePriority.High,
            AcknowledgeType = AcknowledgeTypes.FullReceive,
            TimeToReachQueue = TimeSpan.FromMinutes(5),
            ResponseQueue = new Uri("net.msmq://localhost/private/response"),
        };

        Assert.Equal("hello", m.Body);
        Assert.Equal("demo-label", m.Label);
        Assert.Equal("abcd", m.CorrelationId);
        Assert.Equal(42, m.AppSpecific);
        Assert.Equal(MessagePriority.High, m.Priority);
        Assert.Equal(AcknowledgeTypes.FullReceive, m.AcknowledgeType);
        Assert.Equal(TimeSpan.FromMinutes(5), m.TimeToReachQueue);
        Assert.Equal(new Uri("net.msmq://localhost/private/response"), m.ResponseQueue);
    }

    [WcfFact]
    public static void MsmqMessage_BodySetterRejectsNull()
    {
        var m = new MsmqMessage<string>("a");
        Assert.Throws<ArgumentNullException>(() => m.Body = null);
    }

    [WcfFact]
    public static void MsmqMessage_ReadOnlyProperties_AreInternalOnly()
    {
        // These mirror MSMQ's "set by transport" fields — should not have a
        // public setter on MsmqMessage<T>.
        Type t = typeof(MsmqMessage<string>);
        foreach (string p in new[] { "Acknowledgment", "ArrivedTime", "Authenticated", "DestinationQueue", "Id", "MessageType", "SenderId", "SentTime" })
        {
            var prop = t.GetProperty(p);
            Assert.NotNull(prop);
            Assert.Null(prop.SetMethod);
        }
    }

    [WcfFact]
    public static void MsmqIntegrationMessageProperty_NameConstant()
    {
        Assert.Equal("MsmqIntegrationMessageProperty", MsmqIntegrationMessageProperty.Name);
    }
}
